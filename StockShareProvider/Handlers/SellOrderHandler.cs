using System;
using System.Collections.Generic;
using System.Linq;
using StockShareProvider.Controllers;
using StockShareProvider.Controllers.Models;
using StockShareProvider.DbAccess;
using StockShareProvider.DbAccess.Entities;

namespace StockShareProvider.Handlers
{
    public class SellOrderHandler
    {
        private readonly ProviderContext _dbContext;

        public SellOrderHandler(ProviderContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ResultModel InsertSellOrder(SellOrderInsertModel insertModel)
        {
            var usersSellOrders = _dbContext.SellOrders.Where(t => t.UserID.Equals(insertModel.UserID)).ToList();

            if(HasEqualSellOrders(usersSellOrders, insertModel))
            {
                return new ResultModel()
                       {
                           Result = Result.Error,
                           Error = "User has existing duplicate sell order"
                       };
            }

            // Sell order added even though there might be existing sell order of differenct price value
            _dbContext.SellOrders.Add(new SellOrder()
                                      {
                                          UserID = insertModel.UserID,
                                          SellPrice = insertModel.SellPrice,
                                          StockID = insertModel.StockID,
                                          CreateTime = DateTime.UtcNow
                                      });
            _dbContext.SaveChanges();

            return new ResultModel()
                   {
                       Result = Result.Ok
                   };
        }

        private bool HasEqualSellOrders(List<SellOrder> usersSellOrders, SellOrderInsertModel insertModel)
        {
            return usersSellOrders.Any(t =>
                t.StockID.Equals(insertModel.StockID) && t.SellPrice.Equals(insertModel.SellPrice));
        }

        public class ResultModel
        {
            public Result Result { get; set; }
            public string Error { get; set; }
        }

        public enum Result { Ok, Error }
    }

    
}