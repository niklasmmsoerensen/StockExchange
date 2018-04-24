using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Query;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace HTTPGateway.Controllers
{
    [Route("api/[controller]")]
    public class QueueController : Controller
    {
        const string ServiceBusConnectionString = "Endpoint=sb://stockexchange.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=dWgwAxW1GiEaCVCktd5rX2wF4WSRsG7AOIQuGLY7esw=";
        const string QueueName = "testqueue";
        static IQueueClient queueClient;

        private readonly HttpClient _httpClient;
        private readonly FabricClient _fabricClient;
        private readonly StatelessServiceContext _serviceContext;

        public QueueController(HttpClient httpClient, FabricClient fabricClient, StatelessServiceContext serviceContext)
        {
            _httpClient = httpClient;
            _fabricClient = fabricClient;
            _serviceContext = serviceContext;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            MainAsync().GetAwaiter().GetResult();
            var jsonResult = this.Json("Queue controller");

            return jsonResult;
        }

        private Uri GetProxyAddress(Uri serviceName)
        {
            return new Uri($"http://localhost:19081{serviceName.AbsolutePath}");
        }

        static async Task MainAsync()
        {
            queueClient = new QueueClient(ServiceBusConnectionString, QueueName);

            // Send Message
            await SendMessagesAsync("test message");

            await queueClient.CloseAsync();
        }

        static async Task SendMessagesAsync(string message)
        {
            try
            {
                    var messageBytes = new Message(Encoding.UTF8.GetBytes(message));

                    // Write the body of the message to the console
                    Console.WriteLine($"Sending message: {message}");

                    // Send the message to the queue
                    await queueClient.SendAsync(messageBytes);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }
        }
    }
}
