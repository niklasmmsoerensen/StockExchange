using System;
using System.Linq;
using Shared.Models;
using StockShareProvider.DbAccess;

namespace StockShareProvider.Handlers
{
    public class MessageHandler
    {
        private readonly ProviderContext _dbContext;

        public MessageHandler(ProviderContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void SellOrderFulfilled(string sellOrderId)
        {
            try
            {
                // TODO muligvis noget casting her
                var sellOrderToRemove = _dbContext.SellOrders.Single(t => t.StockID.Equals(Int32.Parse(sellOrderId)));
                _dbContext.Remove(sellOrderToRemove);
                _dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                //log exception
            }
            
        }   
    }
}