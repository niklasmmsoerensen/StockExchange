﻿using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.Infrastructure;
using Shared.Models;
using StockShareRequester.DbAccess;
using StockShareRequester.DbAccess.Entities;

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

        public List<BuyOrder> GetMatchingBuyOrders(int stockId)
        {
            var result = _dbContext.BuyOrders.Where(x => x.StockId.Equals(stockId)).Select(x => x).ToList();
            if (result.Count > 0)
            {
                return result;
            }
            else
            {
                return new List<BuyOrder>();
            }
        }
    }
}
