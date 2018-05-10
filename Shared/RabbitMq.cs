using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using Shared.Models;

namespace Shared
{
    public static class RabbitMq
    {
        private static RabbitMqConfigurationModel _configuration;

        private static RabbitMqConfigurationModel Configuration
        {
            get => _configuration;
            set => _configuration = value;
        }
        

        public static RabbitMqConfigurationModel Setup()
        {
            Configuration = new RabbitMqConfigurationModel();

            Console.WriteLine("RabbitMQ setup starting...");

            var settingPath = Path.GetFullPath(Path.Combine(@"../")); // get absolute path

            var builder = new ConfigurationBuilder()
                .SetBasePath(settingPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            Configuration.HostName = configuration.GetSection("RabbitMQ")["HostName"];
            Configuration.MainExhange = configuration.GetSection("RabbitMQ")["Exchange"];

            //buy related
            Configuration.BuyOrderFulfilledRoutingKey = configuration.GetSection("RabbitMQ")["BuyOrderFulfilledRoutingKey"];
            Configuration.BuyOrderFulfilledQueue = configuration.GetSection("RabbitMQ")["BuyOrderFulfilledQueue"];
            Configuration.NewBuyOrderRoutingKey = configuration.GetSection("RabbitMQ")["NewBuyOrderRoutingKey"];
            Configuration.NewBuyOrderQueue = configuration.GetSection("RabbitMQ")["NewBuyOrderQueue"];

            //sell related
            Configuration.SellOrderFulfilledRoutingKey = configuration.GetSection("RabbitMQ")["SellOrderFulfilledRoutingKey"];
            Configuration.SellOrderFulfilledQueue = configuration.GetSection("RabbitMQ")["SellOrderFulfilledQueue"];
            Configuration.NewSellOrderRoutingKey = configuration.GetSection("RabbitMQ")["NewSellOrderRoutingKey"];
            Configuration.NewSellOrderQueue = configuration.GetSection("RabbitMQ")["NewSellOrderQueue"];

            var connectionFactory = new ConnectionFactory() { HostName = Configuration.HostName };

            var rabbitMQConnection = connectionFactory.CreateConnection();

            var rabbitMQChannel = rabbitMQConnection.CreateModel();
            rabbitMQChannel.ExchangeDeclare(Configuration.MainExhange, ExchangeType.Direct);

            #region Buy related queue setup
            rabbitMQChannel.QueueDeclare(queue: Configuration.BuyOrderFulfilledQueue,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            rabbitMQChannel.QueueBind(Configuration.BuyOrderFulfilledQueue, Configuration.MainExhange, Configuration.BuyOrderFulfilledRoutingKey, null);

            rabbitMQChannel.QueueDeclare(queue: Configuration.NewBuyOrderQueue,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            rabbitMQChannel.QueueBind(Configuration.NewBuyOrderQueue, Configuration.MainExhange, Configuration.NewBuyOrderRoutingKey, null);
            #endregion

            #region Sell related queue setup
            rabbitMQChannel.QueueDeclare(queue: Configuration.SellOrderFulfilledQueue,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            rabbitMQChannel.QueueBind(Configuration.SellOrderFulfilledQueue, Configuration.MainExhange, Configuration.SellOrderFulfilledRoutingKey, null);

            rabbitMQChannel.QueueDeclare(queue: Configuration.NewSellOrderQueue,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            rabbitMQChannel.QueueBind(Configuration.NewSellOrderQueue, Configuration.MainExhange, Configuration.NewSellOrderRoutingKey, null);
            #endregion

            Console.WriteLine("RabbitMQ setup done!");

            return Configuration;
        }
    }
}
