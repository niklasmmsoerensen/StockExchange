using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.FileExtensions;
using Microsoft.Extensions.Configuration.Json;
using RabbitMQ.Client;

namespace RabbitMqSetup
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("RabbitMQ setup starting...");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            string hostName = configuration.GetSection("RabbitMQ")["HostName"];
            string mainExhange = configuration.GetSection("RabbitMQ")["Exchange"];

            //buy related
            string buyOrderFulfilledRoutingKey = configuration.GetSection("RabbitMQ")["BuyOrderFulfilledRoutingKey"];
            string buyOrderFulfilledQueue = configuration.GetSection("RabbitMQ")["BuyOrderFulfilledQueue"];
            string newBuyOrderRoutingKey = configuration.GetSection("RabbitMQ")["NewBuyOrderRoutingKey"];
            string newBuyOrderQueue = configuration.GetSection("RabbitMQ")["NewBuyOrderQueue"];

            //sell related
            string sellOrderFulfilledRoutingKey = configuration.GetSection("RabbitMQ")["SellOrderFulfilledRoutingKey"];
            string sellOrderFulfilledQueue = configuration.GetSection("RabbitMQ")["SellOrderFulfilledQueue"];
            string newSellOrderRoutingKey = configuration.GetSection("RabbitMQ")["NewSellOrderRoutingKey"];
            string newSellOrderQueue = configuration.GetSection("RabbitMQ")["NewSellOrderQueue"];

            var connectionFactory = new ConnectionFactory() { HostName = hostName };

            var rabbitMQConnection = connectionFactory.CreateConnection();

            var rabbitMQChannel = rabbitMQConnection.CreateModel();
            rabbitMQChannel.ExchangeDeclare(mainExhange, ExchangeType.Direct);

            #region Buy related queue setup
            rabbitMQChannel.QueueDeclare(queue: buyOrderFulfilledQueue,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            rabbitMQChannel.QueueBind(buyOrderFulfilledQueue, mainExhange, buyOrderFulfilledRoutingKey, null);

            rabbitMQChannel.QueueDeclare(queue: newBuyOrderQueue,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            rabbitMQChannel.QueueBind(newBuyOrderQueue, mainExhange, newBuyOrderRoutingKey, null);
            #endregion

            #region Sell related queue setup
            rabbitMQChannel.QueueDeclare(queue: sellOrderFulfilledQueue,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            rabbitMQChannel.QueueBind(sellOrderFulfilledQueue, mainExhange, sellOrderFulfilledRoutingKey, null);

            rabbitMQChannel.QueueDeclare(queue: newSellOrderQueue,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            rabbitMQChannel.QueueBind(newSellOrderQueue, mainExhange, newSellOrderRoutingKey, null);
            #endregion

            Console.WriteLine("RabbitMQ setup done!");
        }
    }
}
