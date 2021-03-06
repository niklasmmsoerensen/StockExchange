﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Shared;
using Shared.Abstract;
using StockShareRequester.Handlers;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.IO;
using System.Net.Http;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Models;
using StockShareRequester.DbAccess;
using StockShareRequester.Queue;
using StockShareRequester.Queue.Abstract;

namespace StockShareRequester
{
    public class Startup
    {
        private Logger myLog = new Logger("StockShareRequester");
        private MessageHandler _messageHandler;
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

            var dbContext = (RequesterContext) app.ApplicationServices.GetService(typeof(RequesterContext));
            var queueGateway = (QueueGateWay) app.ApplicationServices.GetService(typeof(IQueueGateWay));
            _messageHandler = new MessageHandler(dbContext, myLog);

            //setup buyorder fulfilled subscriber
            var consumer = new EventingBasicConsumer(queueGateway.Channel);
            consumer.Received += (ch, ea) =>
            {
                _messageHandler.HandleMessage(ea);
            };
            queueGateway.Channel.BasicConsume(queue: queueGateway.OrderFulfilledQueue,
                autoAck: true,
                consumer: consumer);

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
            RabbitMqConfigurationModel configuration = RabbitMq.Setup();

            string hostName = configuration.HostName;
            string mainExhange = configuration.MainExhange;
            string newBuyOrderRoutingKey = configuration.NewBuyOrderRoutingKey;
            string buyOrderFulfilledQueue = configuration.BuyOrderFulfilledQueue;
            string buyOrderFulfilledRoutingKey = configuration.BuyOrderFulfilledRoutingKey;

            var connectionFactory = new ConnectionFactory() { HostName = hostName };

            var rabbitMQConnection = connectionFactory.CreateConnection();

            var rabbitMQChannel = rabbitMQConnection.CreateModel();
            
            services.AddSingleton<IModel>(rabbitMQChannel);
            services.AddScoped<IQueueGateWay>(t => new QueueGateWay(mainExhange, rabbitMQChannel, buyOrderFulfilledQueue, buyOrderFulfilledRoutingKey, newBuyOrderRoutingKey));
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
