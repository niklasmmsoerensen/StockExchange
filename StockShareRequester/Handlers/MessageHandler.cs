﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using Shared;
using Shared.Models;
using StockShareRequester.DbAccess;
using StockShareRequester.Models;

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
                BuyOrderModel model = JsonConvert.DeserializeObject<BuyOrderModel>(body);
                
                var toRemove = DbContext.BuyOrders.Where(x => x.StockId == model.StockId).ToList();
                DbContext.BuyOrders.RemoveRange(toRemove);
                DbContext.SaveChanges();
            }
            catch (Exception e)
            {
                Logger.Error("HandleBuyOrderFulfilled exception: " + e);
            }
        }
    }
}
