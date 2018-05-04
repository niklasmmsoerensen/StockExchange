using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Shared;
using Shared.Abstract;
using StockShareProvider.DbAccess;
using StockShareRequester.Handlers;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.IO;
using RabbitMQ.Client;
using StockShareProvider.Queue;
using StockShareProvider.Queue.Abstract;

namespace StockShareRequester
{
    public class Startup
    {
        private Logger myLog = new Logger("StockShareRequester");
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
            SetupDb(services);

            services.AddScoped(typeof(BuyOrderHandler));

            services.AddScoped<ILogger>(t => myLog);
            services.AddMvc();

            //configure swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "StockShareRequester", Version = "v1" });
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

        private void SetupMQ(IServiceCollection services)
        {
            string hostName = Configuration.GetSection("RabbitMQ")["HostName"];
            string mainExhange = Configuration.GetSection("RabbitMQ")["Exchange"];
            string newBuyOrderQueue = Configuration.GetSection("RabbitMQ")["Queue"];
            string routingKey = Configuration.GetSection("RabbitMQ")["RoutingKey"];

            var connectionFactory = new ConnectionFactory() { HostName = hostName };

            var rabbitMQConnection = connectionFactory.CreateConnection();

            var rabbitMQChannel = rabbitMQConnection.CreateModel();

            //only run if queue doesn't already exist
            rabbitMQChannel.ExchangeDeclare(mainExhange, ExchangeType.Direct);
            rabbitMQChannel.QueueDeclare(queue: newBuyOrderQueue,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            rabbitMQChannel.QueueBind(newBuyOrderQueue, mainExhange, routingKey, null);

            services.AddSingleton<IModel>(rabbitMQChannel);
            services.AddScoped<IQueueGateWay>(t => new QueueGateWay(routingKey, mainExhange, rabbitMQChannel));
        }

        private void SetupDb(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("Default");

            services.AddDbContext<RequesterContext>(options =>
            {
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(
                        typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                });
            });
        }
    }
}
