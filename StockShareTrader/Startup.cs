using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Shared;
using Shared.Abstract;
using StockShareTrader.DbAccess;
using StockShareTrader.Handlers;
using StockShareTrader.Queue;
using StockShareTrader.Queue.Abstract;
using Swashbuckle.AspNetCore.Swagger;

namespace StockShareTrader
{
    public class Startup
    {
        private Logger myLog = new Logger("StockShareTrader");
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
            SetupMQ(services);

            services.AddScoped(typeof(PurchaseHandler));

            services.AddMvc();

            services.AddScoped<ILogger>(t => myLog);

            //configure swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "StockShareTrader", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            app.UseSwagger();

            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
        }

        private void SetupMQ(IServiceCollection services)
        {
            string hostName = Configuration.GetSection("RabbitMQ")["HostName"];
            string mainExhange = Configuration.GetSection("RabbitMQ")["Exchange"];
            string sellOrderFulfilledRoutingKey = Configuration.GetSection("RabbitMQ")["RoutingKeySellOrderFulfilled"];
            string buyOrderFulfilledRoutingKey = Configuration.GetSection("RabbitMQ")["RoutingKeyBuyOrderFulfilled"];

            var connectionFactory = new ConnectionFactory() { HostName = hostName };

            var rabbitMQConnection = connectionFactory.CreateConnection();

            var rabbitMQChannel = rabbitMQConnection.CreateModel();

            //doesn't make a new exchange if it already exists
            rabbitMQChannel.ExchangeDeclare(mainExhange, ExchangeType.Direct);

            services.AddSingleton<IModel>(rabbitMQChannel);
            services.AddScoped<IQueueGateWay>(t => new QueueGateWay(mainExhange, rabbitMQChannel, sellOrderFulfilledRoutingKey, buyOrderFulfilledRoutingKey));
        }

        private void SetupDb(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("Default");

            services.AddDbContext<TraderContext>(options =>
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
