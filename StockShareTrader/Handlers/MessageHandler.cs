using System;
using RabbitMQ.Client.Events;
using Shared;
using Shared.Abstract;
using StockShareTrader.DbAccess;

namespace StockShareTrader.Handlers
{
    public class MessageHandler
    {
        private readonly TraderContext _dbContext;
        private readonly ILogger _logger;

        public MessageHandler(TraderContext dbContext, Logger logger)
        {
            _dbContext = dbContext;
            _logger = logger;
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
                    _logger.Error("HandleMessage: Invalid routing key " + routingKey);
                    break;
            }
        }

        private void HandleBuyOrderFulfilled(string body)
        {
            _logger.Info("HandleBuyOrderFulfilled invoked");
            try
            {
                
            }
            catch (Exception e)
            {
                _logger.Error("HandleBuyOrderFulfilled exception: " + e);
            }
        }
    }
}
