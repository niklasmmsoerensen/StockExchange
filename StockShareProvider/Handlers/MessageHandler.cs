using System;
using System.Linq;
using Shared.Models;
using StockShareProvider.DbAccess;
using StockShareProvider.Handlers.Models;

namespace StockShareProvider.Handlers
{
    public class MessageHandler
    {
        private readonly ProviderContext _dbContext;

        public MessageHandler(ProviderContext dbContext)
        {
            _dbContext = dbContext;

            _dbContext.SellOrders.Where(t => t.StockID == 5)
        }

        public void SellOrderFulfilled(int sellOrderId)
        {
            var sellOrderToRemove = _dbContext.SellOrders.Single(t => t.StockID.Equals(sellOrderId));
            _dbContext.Remove(sellOrderToRemove);
            _dbContext.SaveChanges();
        }   
    }
}