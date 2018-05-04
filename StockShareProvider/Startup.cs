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
using Shared;
using Shared.Abstract;
using StockShareProvider.DbAccess;
using StockShareProvider.Handlers;
using StockShareProvider.Queue;
using StockShareProvider.Queue.Abstract;
using Swashbuckle.AspNetCore.Swagger;
using SellOrderHandler = StockShareProvider.Handlers.SellOrderHandler;

namespace StockShareProvider
{
    public class Startup
    {
        private Logger _myLog = new Logger("StockShareProvider");
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
            string hostName = Configuration.GetSection("RabbitMQ")["HostName"];
            string mainExhange = Configuration.GetSection("RabbitMQ")["Exchange"];
            string newSellOrderQueue = Configuration.GetSection("RabbitMQ")["Queue"];
            string routingKey = Configuration.GetSection("RabbitMQ")["RoutingKey"];

            var connectionFactory = new ConnectionFactory() { HostName = hostName };

            var rabbitMQConnection = connectionFactory.CreateConnection();

            var rabbitMQChannel = rabbitMQConnection.CreateModel();

            //only run if queue doesn't already exist
            rabbitMQChannel.ExchangeDeclare(mainExhange, ExchangeType.Direct);
            rabbitMQChannel.QueueDeclare(queue: newSellOrderQueue,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            rabbitMQChannel.QueueBind(newSellOrderQueue, mainExhange, routingKey, null);

            services.AddSingleton<IModel>(rabbitMQChannel);
            services.AddScoped<IQueueGateWay>(t => new QueueGateWay(routingKey, mainExhange, rabbitMQChannel));
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
