using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using System.Diagnostics;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace HTTPGateway.Controllers
{
    [Route("api/[controller]")]
    public class MessageReceiverController : Controller
    {
        private IConnection _RabbitMqConnection;
        private IModel _RabbitMqChannel;
        private const string EXCHANGE = "testExchange";
        private const string QUEUE = "test2";

        [HttpGet]
        public string Get()
        {
            using (_RabbitMqConnection = StockShareProvider.Microservice.StockShareProvider.RabbitMqFactory.CreateConnection())
            using (_RabbitMqChannel = _RabbitMqConnection.CreateModel())
            {
                _RabbitMqChannel.ExchangeDeclare(EXCHANGE, ExchangeType.Direct);
                _RabbitMqChannel.QueueDeclare(queue: QUEUE,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
                _RabbitMqChannel.QueueBind(QUEUE, EXCHANGE, "test", null);

                var message = System.Text.Encoding.UTF8.GetBytes("Hello from MessageReceiverController!");

                //publish message to exchange
                _RabbitMqChannel.BasicPublish(exchange: EXCHANGE,
                                     routingKey: "test",
                                     basicProperties: null,
                                     body: message);
            }

            try
            {
                return "ayy lmao message receiver controller";
            }
            catch(Exception e)
            {
                return "error!" + e.ToString();
            }
            
        }
    }
}
