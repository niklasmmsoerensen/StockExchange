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
            string newBuyOrderRoutingKey = configuration.GetSection("RabbitMQ")["SellOrderFulfilledRoutingKey"];
            string newBuyOrderQueue = configuration.GetSection("RabbitMQ")["SellOrderFulfilledQueue"];

            //sell related
            string sellOrderFulfilledRoutingKey = configuration.GetSection("RabbitMQ")["SellOrderFulfilledRoutingKey"];
            string sellOrderFulfilledQueue = configuration.GetSection("RabbitMQ")["SellOrderFulfilledQueue"];
            string newSellOrderRoutingKey = configuration.GetSection("RabbitMQ")["SellOrderFulfilledRoutingKey"];
            string newSellOrderQueue = configuration.GetSection("RabbitMQ")["SellOrderFulfilledQueue"];

            var connectionFactory = new ConnectionFactory() { HostName = hostName };

            var rabbitMQConnection = connectionFactory.CreateConnection();

            var rabbitMQChannel = rabbitMQConnection.CreateModel();

            rabbitMQChannel.ExchangeDeclare(mainExhange, ExchangeType.Direct);

            //declare order fulfilled queue
            rabbitMQChannel.QueueDeclare(queue: buyOrderFulfilledQueue,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            rabbitMQChannel.QueueBind(buyOrderFulfilledQueue, mainExhange, buyOrderFulfilledRoutingKey, null);

            rabbitMQChannel.QueueDeclare(queue: sellOrderFulfilledQueue,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            rabbitMQChannel.QueueBind(sellOrderFulfilledQueue, mainExhange, sellOrderFulfilledRoutingKey, null);

            rabbitMQChannel.QueueDeclare(queue: sellOrderFulfilledQueue,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            rabbitMQChannel.QueueBind(sellOrderFulfilledQueue, mainExhange, sellOrderFulfilledRoutingKey, null);

            Console.WriteLine("RabbitMQ setup done!");
        }
    }
}
