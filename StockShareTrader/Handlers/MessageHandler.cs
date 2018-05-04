using System;
using RabbitMQ.Client.Events;
using Shared;
using StockShareTrader.DbAccess;

namespace StockShareRequester.Handlers
{
    public class MessageHandler
    {
        private TraderContext DbContext { get; set; }
        private Logger Logger { get; set; }

        public MessageHandler(TraderContext dbContext, Logger logger)
        {
            DbContext = dbContext;
            Logger = logger;
        }

        public void HandleMessage(BasicDeliverEventArgs ea)
        {
            var body = System.Text.Encoding.Default.GetString(ea.Body);
            var routingKey = ea.RoutingKey;
            switch (routingKey)
            {
                case "NewBuyOrder":
                    HandleBuyOrderFulfilled(body);
                    break;
                default:
                    Logger.Error("HandleMessage: Invalid routing key " + routingKey);
                    break;
            }
        }

        private void HandleBuyOrderFulfilled(string body)
        {
            try
            {
                
            }
            catch (Exception e)
            {
                Logger.Error("HandleBuyOrderFulfilled exception: " + e);
            }
        }
    }
}
