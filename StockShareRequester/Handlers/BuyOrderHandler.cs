using Microsoft.EntityFrameworkCore;
using StockShareProvider.DbAccess;
using StockShareRequester.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockShareRequester.Handlers
{
    public class BuyOrderHandler
    {
        private RequesterContext _dbContext { get; set; }
        public BuyOrderHandler(RequesterContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ResultModel InsertBuyOrder(BuyOrderModel buyOrder)
        {
            try
            {
                _dbContext.BuyOrders.Add(new BuyOrder {
                    StockId = buyOrder.StockId,
                    UserId = buyOrder.UserId,
                    Price = buyOrder.Price,
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

        public ResultModel GetMatchingBuyOrders(BuyOrderModel buyOrder)
        {
            try
            {
                var result = _dbContext.BuyOrders.Where(x => x.StockId.Equals(buyOrder.StockId)).Select(x => x).ToList();
                if (result.Count > 0)
                {
                    return new ResultModel(Result.Ok);
                }
                else
                {
                    return new ResultModel(Result.Ok, "No matching buy orders found");
                }
            }
            catch (Exception e)
            {
                return new ResultModel(Result.Error, e.ToString());
            }
        }
    }
}
