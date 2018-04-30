using System;
using System.Collections.Generic;
using System.Fabric;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace HTTPGateway
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance. 
    /// </summary>
    internal sealed class HTTPGateway : StatelessService
    {
        //RabbitMQ stuff
        private ConnectionFactory _RabbitMqFactory;
        private IConnection _RabbitMqConnection;
        static private IModel _RabbitMqChannel;
        private string _RabbitMqHostName;
        private string _RabbitMqExchange;
        private string _RabbitMqQueue;

        static public IModel RabbitMqChannel { get => _RabbitMqChannel; set => _RabbitMqChannel = value; }
        public string RabbitMqExchange { get => _RabbitMqExchange; set => _RabbitMqExchange = value; }
        public string RabbitMqQueue { get => _RabbitMqQueue; set => _RabbitMqQueue = value; }
        public string RabbitMqHostName { get => _RabbitMqHostName; set => _RabbitMqHostName = value; }

        public HTTPGateway(StatelessServiceContext context)
            : base(context)
        {
            LoadConfiguration();

            _RabbitMqFactory = new ConnectionFactory() { HostName = RabbitMqHostName };

           _RabbitMqConnection = _RabbitMqFactory.CreateConnection();
            _RabbitMqChannel = _RabbitMqConnection.CreateModel();

            //it doesn't create a new one if it already exists
            _RabbitMqChannel.ExchangeDeclare(RabbitMqExchange, ExchangeType.Direct);
            _RabbitMqChannel.QueueDeclare(queue: RabbitMqQueue,
                                durable: false,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);
            //binds queue and exchange
            _RabbitMqChannel.QueueBind(RabbitMqQueue, RabbitMqExchange, "test", null);

            //setup consumer/subscriber for our channel
            var consumer = new EventingBasicConsumer(_RabbitMqChannel);
            consumer.Received += async (ch, ea) =>
                {
                    //Setup Uri to call QueueController, don't know if this is the right way to do it when you want to call a controller within the service itself
                    Uri serviceName = HTTPGateway.GetHttpGatewayServiceName(context);
                    Uri proxyAddress = new Uri($"http://localhost:19081{serviceName.AbsolutePath}");
                    string proxyUrl = $"{proxyAddress}/api/Queue";

                    //the body of the message from the queue
                    var body = ea.Body;

                    //call QueueController
                    HttpClient httpClient = new HttpClient();
                    using (HttpResponseMessage response = await httpClient.GetAsync(proxyUrl))
                    {
                        var result = response.Content.ReadAsStringAsync();
                    }

                    //acknowledge message from queue
                    _RabbitMqChannel.BasicAck(ea.DeliveryTag, false);
                }; //calls QueueController when receiving a message

            //attach consumer to channel
            String consumerTag = _RabbitMqChannel.BasicConsume(RabbitMqQueue, false, consumer);
            
        }

        /// <summary>
        /// Optional override to create listeners (like tcp, http) for this service instance.
        /// </summary>
        /// <returns>The collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[]
            {
                new ServiceInstanceListener(serviceContext =>
                    new KestrelCommunicationListener(serviceContext, "ServiceEndpoint", (url, listener) =>
                    {
                        ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");

                        return new WebHostBuilder()
                                    .UseKestrel()
                                    .ConfigureServices(
                                        services => services
                                            .AddSingleton(serviceContext)
                                            .AddSingleton(new HttpClient())
                                            .AddSingleton(new FabricClient())
                                            .AddSingleton(serviceContext))
                                    .UseContentRoot(Directory.GetCurrentDirectory())
                                    .UseStartup<Startup>()
                                    .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                                    .UseUrls(url)
                                    .Build();
                    }))
            };
        }

        private void LoadConfiguration()
        {   
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            
            RabbitMqHostName = configuration.GetSection("RabbitMQ")["HostName"];
            RabbitMqExchange = configuration.GetSection("RabbitMQ")["Exchange"];
            RabbitMqQueue = configuration.GetSection("RabbitMQ")["Queue"];
        }

        internal static Uri GetStockShareProviderServiceName(ServiceContext context)
        {
            return new Uri($"{context.CodePackageActivationContext.ApplicationName}/StockShareProvider");
        }

        internal static Uri GetHttpGatewayServiceName(ServiceContext context)
        {
            return new Uri($"{context.CodePackageActivationContext.ApplicationName}/HTTPGateway");
        }
    }
}
