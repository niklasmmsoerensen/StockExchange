using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;
using Shared.Abstract;
using StockShareProvider.DbAccess;
using StockShareProvider.Handlers;
using StockShareProvider.Queue;
using StockShareProvider.Queue.Abstract;
using StockShareProvider.ServiceRelated;
using Swashbuckle.AspNetCore.Swagger;
using SellOrderHandler = StockShareProvider.Handlers.SellOrderHandler;

namespace StockShareProvider
{
    public class Startup
    {
        private readonly ILogger _myLog = new Logger("StockShareProvider");
        private MessageHandler _messageHandler;
        private string _hostName;
        private string _mainExhange;
        private string _sellOrderFulfilledQueue;
        private string _newSellOrderRoutingKey;
        private string _sellOrderFulfilledRoutingKey;


        public Startup(IConfiguration configuration)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ILogger>(t => _myLog);

            services.AddScoped(typeof(SellOrderHandler));

            services.AddMvc();
            
            SetupDb(services);

            SetupMQ(services);
            

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "StockShareProvider", Version = "v1" });
            });
        }

        private void SetupMQ(IServiceCollection services)
        {
            _hostName = Configuration.GetSection("RabbitMQ")["HostName"];
            _mainExhange = Configuration.GetSection("RabbitMQ")["Exchange"];

            _newSellOrderRoutingKey = Configuration.GetSection("RabbitMQ")["NewSellOrderRoutingKey"];

            _sellOrderFulfilledRoutingKey = Configuration.GetSection("RabbitMQ")["SellOrderFulfilledKey"];
            _sellOrderFulfilledQueue = Configuration.GetSection("RabbitMQ")["SellOrderFulfilledQueue"];
            

            var connectionFactory = new ConnectionFactory() { HostName = _hostName };

            var rabbitMQConnection = connectionFactory.CreateConnection();

            var rabbitMQChannel = rabbitMQConnection.CreateModel();

            //only run if queue doesn't already exist
            rabbitMQChannel.ExchangeDeclare(_mainExhange, ExchangeType.Direct);
            

            rabbitMQChannel.QueueDeclare(queue: _sellOrderFulfilledQueue,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            rabbitMQChannel.QueueBind(_sellOrderFulfilledQueue, _mainExhange, _sellOrderFulfilledRoutingKey, null);
            
            services.AddSingleton<IModel>(rabbitMQChannel);

            services.AddScoped<IQueueGateway>(t => new QueueGateway(rabbitMQChannel, _mainExhange, _newSellOrderRoutingKey));
        }

        private void SetupDb(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("Default");

            services.AddDbContext<ProviderContext>(options =>
            {
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(
                        typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            var dbContext = (ProviderContext)app.ApplicationServices.GetService(typeof(ProviderContext));
            var channel = (IModel)app.ApplicationServices.GetService(typeof(IModel));
            
            _messageHandler = new MessageHandler(dbContext);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (ch, ea) => { _messageHandler.SellOrderFulfilled(Encoding.UTF8.GetString(ea.Body)); };
            channel.BasicConsume(_sellOrderFulfilledQueue, true, consumer);

            app.UseSwagger();

            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
