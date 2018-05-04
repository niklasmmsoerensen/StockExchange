using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Shared;
using ILogger = Shared.Abstract.ILogger;

namespace StockShareBroker
{
    public class Startup
    {
        private readonly ILogger _myLog = new Logger("StockShareProvider");
        private string _hostName;
        private string _mainExhange;
        private string _newSellOrderQueue;
        private string _newSellOrderRoutingKey;


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

            SetupMQ(services);
            services.AddMvc();
        }

        private void SetupMQ(IServiceCollection services)
        {
            _hostName = Configuration.GetSection("RabbitMQ")["HostName"];
            _mainExhange = Configuration.GetSection("RabbitMQ")["Exchange"];

            _newSellOrderQueue = Configuration.GetSection("RabbitMQ")["NewSellOrderQueue"];
            _newSellOrderRoutingKey = Configuration.GetSection("RabbitMQ")["NewSellOrderRoutingKey"];


            var connectionFactory = new ConnectionFactory() { HostName = _hostName };

            var rabbitMQConnection = connectionFactory.CreateConnection();

            var rabbitMQChannel = rabbitMQConnection.CreateModel();

            //only run if queue doesn't already exist
            rabbitMQChannel.ExchangeDeclare(_mainExhange, ExchangeType.Direct);


            rabbitMQChannel.QueueDeclare(queue: _newSellOrderQueue,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            rabbitMQChannel.QueueBind(_newSellOrderQueue, _mainExhange, _newSellOrderRoutingKey, null);

            services.AddSingleton<IModel>(rabbitMQChannel);

            // TODO tilføj handler til queue events 
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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
