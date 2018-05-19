using System;
using System.Linq;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using Shared;
using Shared.Abstract;
using Shared.Models;
using StockShareRequester.DbAccess;

namespace StockShareRequester.Handlers
{
    public class MessageHandler
    {
        private readonly RequesterContext _dbContext;
        private readonly ILogger _log;

        public MessageHandler(RequesterContext dbContext, ILogger logger)
        {
            _dbContext = dbContext;
            _log = logger;
        }

        public void HandleMessage(BasicDeliverEventArgs ea)
        {
            var body = System.Text.Encoding.Default.GetString(ea.Body);
            var routingKey = ea.RoutingKey;
            switch (routingKey)
            {
                case "BuyOrderFulfilledRoutingKey":
                    HandleBuyOrderFulfilled(body);
                    break;
                default:
                    _log.Error("HandleMessage: Invalid routing key " + routingKey);
                    break;
            }
        }

        private void HandleBuyOrderFulfilled(string stockId)
        {
            _log.Info("SellOrderFulfilledHandler invoked");
            try
            {
                var buyOrderToRemove = _dbContext.BuyOrders.Single(t => t.StockId.Equals(Int32.Parse(stockId)));
                _dbContext.Remove(buyOrderToRemove);
                _dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                _log.Error("HandleBuyOrderFulfilled exception: " + e);
            }
        }
    }
}
