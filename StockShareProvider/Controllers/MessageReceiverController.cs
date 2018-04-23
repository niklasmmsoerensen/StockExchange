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

namespace HTTPGateway.Controllers
{
    [Route("api/[controller]")]
    public class MessageReceiverController : Controller
    {
        const string ServiceBusConnectionString = "Endpoint=sb://stockexchange.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=dWgwAxW1GiEaCVCktd5rX2wF4WSRsG7AOIQuGLY7esw=";
        const string QueueName = "testqueue";
        static IQueueClient queueClient;

        [HttpGet]
        public string Get()
        {
            try
            {
                return "ayy lmao message receiver controller";
            }
            catch(Exception e)
            {
                return "error!" + e.ToString();
            }
            
        }

        public static async Task SetupQueueListeners()
        {
            queueClient = new QueueClient(ServiceBusConnectionString, QueueName, ReceiveMode.PeekLock);

            // Register the queue message handler
            registerMessageHandlers();
        }

        private static void registerMessageHandlers()
        {
            try
            {
                var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
                {
                    // Maximum number of Concurrent calls to the callback `ProcessMessagesAsync`, set to 1 for simplicity.
                    // Set it according to how many messages the application wants to process in parallel.
                    MaxConcurrentCalls = 1,

                    // Indicates whether MessagePump should automatically complete the messages after returning from User Callback.
                    // False below indicates the Complete will be handled by the User Callback as in `ProcessMessagesAsync` below.
                    AutoComplete = false
                };
                // Register a OnMessage callback
                queueClient.RegisterMessageHandler(
                    async (message, token) =>
                    {
                        // Process the message
                        Debug.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");

                        // Complete the message so that it is not received again.
                        // This can be done only if the queueClient is opened in ReceiveMode.PeekLock mode.
                        await queueClient.CompleteAsync(message.SystemProperties.LockToken);
                    },
                    messageHandlerOptions);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} > Exception: {exception.Message}");
            }
        }

        static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Debug.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Debug.WriteLine("Exception context for troubleshooting:");
            Debug.WriteLine($"- Endpoint: {context.Endpoint}");
            Debug.WriteLine($"- Entity Path: {context.EntityPath}");
            Debug.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }
    }
}
