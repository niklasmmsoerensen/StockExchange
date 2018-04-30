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
using Swashbuckle.AspNetCore.Swagger;

namespace StockShareProvider
{
    public class Startup
    {
        private Logger myLog = new Logger("StockShareProvider");
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
            services.AddScoped<ILogger>(t => myLog);
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
            string HostName = Configuration.GetSection("RabbitMQ")["HostName"];
            string EXCHANGE = Configuration.GetSection("RabbitMQ")["Exchange"];
            string QUEUE = Configuration.GetSection("RabbitMQ")["Queue"];

            var connectionFactory = new ConnectionFactory() { HostName = HostName };

            var rabbitMQConnection = connectionFactory.CreateConnection();

            var rabbitMQChannel = rabbitMQConnection.CreateModel();

            //only run if queue doesn't already exist
            rabbitMQChannel.ExchangeDeclare(EXCHANGE, ExchangeType.Direct);
            rabbitMQChannel.QueueDeclare(queue: QUEUE,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            rabbitMQChannel.QueueBind(QUEUE, EXCHANGE, "test", null);

            rabbitMQChannel.BasicPublish(exchange: EXCHANGE,
                routingKey: "test",
                basicProperties: null,
                body: Encoding.UTF8.GetBytes("Hello from MessageReceiverController!"));

            services.AddSingleton<IModel>(rabbitMQChannel);
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

                    ////Configuring Connection Resiliency:
                    //sqlOptions.
                    //    EnableRetryOnFailure(maxRetryCount: 5,
                    //        maxRetryDelay: TimeSpan.FromSeconds(30),
                    //        errorNumbersToAdd: null);
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
