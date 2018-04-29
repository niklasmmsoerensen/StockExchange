using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;

namespace StockShareProvider.Controllers
{
    [Route("api/[controller]")]
    public class MessageReceiverController : Controller
    {
        private readonly IModel _mqChannel;
        private readonly string _exchangeName = "testExchange";

        public MessageReceiverController(IModel mqChannel)
        {
            _mqChannel = mqChannel;
        }

        [HttpGet]
        public IActionResult Get()
        {
            //publish message to exchange
            _mqChannel.BasicPublish(exchange: _exchangeName,
                                 routingKey: "test",
                                 basicProperties: null,
                                 body: Encoding.UTF8.GetBytes("Message from Controller - now with dependency injection"));
            try
            {
                return new ObjectResult("ayy lmao message receiver controller");
            }
            catch(Exception e)
            {
                return new ObjectResult("error!" + e);
            }
        }
    }
}
