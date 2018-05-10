using System;
using System.Collections.Generic;
using System.Linq;
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

        public ResultModel<List<TransactionModel>> GetTransactions()
        {
            try
            {
                var transactions = _dbContext.Transactions.Where(t => t.CreateTime > DateTime.UtcNow.AddHours(-1))
                    .Select(x => new TransactionModel()
                    {
                        StockId = x.StockId,
                        BuyerUserId = x.UserId,
                        Price = x.Price,
                        CreationTime = x.CreateTime
                    }).OrderByDescending(t => t.CreationTime).ToList();
               
                return new ResultModel<List<TransactionModel>>(Result.Ok, transactions);
            }
            catch (Exception e)
            {
                return new ResultModel<List<TransactionModel>>(Result.Error, new List<TransactionModel>(), e.ToString());
            }
        }
    }
}
