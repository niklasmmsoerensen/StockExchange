using System.IO;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;
using StockShareBroker.Handlers;
using Swashbuckle.AspNetCore.Swagger;
using ILogger = Shared.Abstract.ILogger;

namespace StockShareBroker
{
    public class Startup
    {
        private readonly ILogger _myLog = new Logger("StockShareBroker");
        private string _hostName;
        private string _mainExhange;
        private string _newSellOrderQueue;
        private string _newSellOrderRoutingKey;
        private string _newBuyOrderQueue;
        private string _newBuyOrderRoutingKey;


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
            _hostName = Configuration.GetSection("RabbitMQ")["HostName"];
            _mainExhange = Configuration.GetSection("RabbitMQ")["Exchange"];

            _newSellOrderQueue = Configuration.GetSection("RabbitMQ")["NewSellOrderQueue"];
            _newSellOrderRoutingKey = Configuration.GetSection("RabbitMQ")["NewSellOrderRoutingKey"];

            _newBuyOrderQueue = Configuration.GetSection("RabbitMQ")["NewBuyOrderQueue"];
            _newBuyOrderRoutingKey = Configuration.GetSection("RabbitMQ")["NewBuyOrderRoutingKey"];

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

            rabbitMQChannel.ExchangeDeclare(_mainExhange, ExchangeType.Direct);
            rabbitMQChannel.QueueDeclare(queue: _newBuyOrderQueue,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            rabbitMQChannel.QueueBind(_newBuyOrderQueue, _mainExhange, _newBuyOrderRoutingKey, null);


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
