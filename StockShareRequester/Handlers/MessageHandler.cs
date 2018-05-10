using System;
using System.Linq;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using Shared;
using Shared.Models;
using StockShareRequester.DbAccess;

namespace StockShareRequester.Handlers
{
    public class MessageHandler
    {
        private RequesterContext DbContext { get; set; }
        private Logger Logger { get; set; }

        public MessageHandler(RequesterContext dbContext, Logger logger)
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
                case "BuyOrderFulfilledRoutingKey":
                    HandleBuyOrderFulfilled(body);
                    break;
                default:
                    Logger.Error("HandleMessage: Invalid routing key " + routingKey);
                    break;
            }
        }

        private void HandleBuyOrderFulfilled(string stockId)
        {
            try
            {
                var buyOrderToRemove = DbContext.BuyOrders.Single(t => t.StockId.Equals(Int32.Parse(stockId)));
                DbContext.Remove(buyOrderToRemove);
                DbContext.SaveChanges();
            }
            catch (Exception e)
            {
                Logger.Error("HandleBuyOrderFulfilled exception: " + e);
            }
        }
    }
}
