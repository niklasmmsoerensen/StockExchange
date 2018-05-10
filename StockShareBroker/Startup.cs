using System.IO;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;
using Shared.Models;
using StockShareBroker.Handlers;
using Swashbuckle.AspNetCore.Swagger;
using ILogger = Shared.Abstract.ILogger;

namespace StockShareBroker
{
    public class Startup
    {
        private readonly ILogger _myLog = new Logger("StockShareBroker");
        private string _hostName;
        private string _newSellOrderQueue;
        private string _newBuyOrderQueue;


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

            //configure swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "StockShareBroker", Version = "v1" });
            });
        }

        private void SetupMQ(IServiceCollection services)
        {
            RabbitMqConfigurationModel configuration = RabbitMq.Setup();

            _hostName = configuration.HostName;

            _newSellOrderQueue = configuration.NewSellOrderQueue;

            _newBuyOrderQueue = configuration.NewBuyOrderQueue;

            var connectionFactory = new ConnectionFactory() { HostName = _hostName };
            var rabbitMQConnection = connectionFactory.CreateConnection();
            var rabbitMQChannel = rabbitMQConnection.CreateModel();

            services.AddSingleton<IModel>(rabbitMQChannel);

            services.AddScoped(typeof(MessageHandler));
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            var rabbitMQchannel = (IModel)app.ApplicationServices.GetService(typeof(IModel));
            var messageHandler = (MessageHandler)app.ApplicationServices.GetService(typeof(MessageHandler));

            // new buy order event setup
            var consumerBuyorder = new EventingBasicConsumer(rabbitMQchannel);
            consumerBuyorder.Received += (ch, ea) => { messageHandler.NewBuyOrderHandler(ea.Body); };
            rabbitMQchannel.BasicConsume(_newBuyOrderQueue, true, consumerBuyorder);

            // new sell order event setup
            var consumerSellOrder = new EventingBasicConsumer(rabbitMQchannel);
            consumerSellOrder.Received += (ch, ea) => { messageHandler.NewSellOrderHandler(ea.Body); };
            rabbitMQchannel.BasicConsume(_newSellOrderQueue, true, consumerSellOrder);

            app.UseSwagger();

            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "StockShareBroker"); });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
