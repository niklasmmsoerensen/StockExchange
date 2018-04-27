using System.Collections.Generic;
using System.Fabric;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using HTTPGateway.Controllers;
using RabbitMQ.Client;

namespace StockShareProvider.Microservice
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance. 
    /// </summary>
    internal sealed class StockShareProvider : StatelessService
    {
        static private ConnectionFactory _RabbitMqFactory;
        private IConnection _RabbitMqConnection;
        private IModel _RabbitMqChannel;
        private const string EXCHANGE = "testExchange";
        private const string QUEUE = "test2";

        public StockShareProvider(StatelessServiceContext context) : base(context)
        {
            RabbitMqFactory = new ConnectionFactory() { HostName = "localhost" };
            using (_RabbitMqConnection = RabbitMqFactory.CreateConnection())
            using (_RabbitMqChannel = _RabbitMqConnection.CreateModel())
            {
                //only run if queue doesn't already exist
                _RabbitMqChannel.ExchangeDeclare(EXCHANGE, ExchangeType.Direct);
                _RabbitMqChannel.QueueDeclare(queue: QUEUE,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
                _RabbitMqChannel.QueueBind(QUEUE, EXCHANGE, "test", null);

                var message = System.Text.Encoding.UTF8.GetBytes("Hello from MessageReceiverController!");

                _RabbitMqChannel.BasicPublish(exchange: EXCHANGE,
                                     routingKey: "test",
                                     basicProperties: null,
                                     body: message);
            }
        }
        static public ConnectionFactory RabbitMqFactory { get => _RabbitMqFactory; set => _RabbitMqFactory = value; }

        /// <summary>
        /// Optional override to create listeners (like tcp, http) for this service instance.
        /// </summary>
        /// <returns>The collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new[]
            {
                new ServiceInstanceListener(serviceContext =>
                    new KestrelCommunicationListener(serviceContext, "ServiceEndpoint", (url, listener) =>
                    {
                        ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");

                        return new WebHostBuilder()
                                    .UseKestrel()
                                    .ConfigureServices(
                                        services => services
                                            .AddSingleton<StatelessServiceContext>(serviceContext))
                                    .UseContentRoot(Directory.GetCurrentDirectory())
                                    .UseStartup<Startup>()
                                    .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                                    .UseUrls(url)
                                    .Build();
                    }))
            };
        }
    }
}
