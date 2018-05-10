using System;
using Shared.Infrastructure;
using Shared.Models;
using StockShareTrader.DbAccess;
using StockShareTrader.DbAccess.Entities;

namespace StockShareTrader.Handlers
{
    public class PurchaseHandler
    {
        private TraderContext _dbContext { get; set; }
        public PurchaseHandler(TraderContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ResultModel InsertTransaction(TransactionModel transaction)
        {
            try
            {
                _dbContext.Transactions.Add(new Transaction()
                {
                    StockId = transaction.StockId,
                    UserId = transaction.BuyerUserId,
                    Price = transaction.Price,
                    CreateTime = DateTime.UtcNow
                });
                _dbContext.SaveChanges();
                return new ResultModel(Result.Ok);
            }
            catch(Exception e)
            {
                return new ResultModel(Result.Error, e.ToString());
            }
        }
    }
}
