using System;
using System.Linq;
using Shared.Abstract;
using Shared.Models;
using StockShareProvider.DbAccess;

namespace StockShareProvider.Handlers
{
    public class MessageHandler
    {
        private readonly ProviderContext _dbContext;
        private readonly ILogger _log;

        public MessageHandler(ProviderContext dbContext, ILogger log)
        {
            _dbContext = dbContext;
            _log = log;
        }

        public void SellOrderFulfilledHandler(string sellOrderId)
        {
            try
            {
                var sellOrderToRemove = _dbContext.SellOrders.Single(t => t.StockID.Equals(Int32.Parse(sellOrderId)));
                _dbContext.Remove(sellOrderToRemove);
                _dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                _log.Error($"Error on SellOrderFulfilledHandler: {e.Message}");
            }            
        }   
    }
}