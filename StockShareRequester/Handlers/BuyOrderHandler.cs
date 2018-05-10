﻿using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.Abstract;
using Shared.Infrastructure;
using Shared.Models;
using StockShareRequester.DbAccess;
using StockShareRequester.DbAccess.Entities;

namespace StockShareRequester.Handlers
{
    public class BuyOrderHandler
    {
        private readonly RequesterContext _dbContext;
        private readonly ILogger _logger;
        public BuyOrderHandler(RequesterContext dbContext, ILogger logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public ResultModel InsertBuyOrder(BuyOrderModel buyOrder)
        {
            try
            {
                var orders = _dbContext.BuyOrders.Where(x => x.StockId == buyOrder.StockId && x.UserId == buyOrder.UserId).ToList();
                _dbContext.BuyOrders.RemoveRange(orders);
                
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
                _logger.Error($"Error on InsertBuyOrder: {e}");
                return new ResultModel(Result.Error, e.Message);
            }
        }

        public ResultModel<List<BuyOrderModel>> GetMatchingBuyOrders(int stockId)
        {
            try
            {
                var result = _dbContext.BuyOrders.Where(x => x.StockId.Equals(stockId)).Select(x => new BuyOrderModel()
                                                                                                    {
                                                                                                        StockId = x.StockId,
                                                                                                        UserId = x.UserId,
                                                                                                        Price = x.Price
                                                                                                    }).ToList();

                return new ResultModel<List<BuyOrderModel>>
                       {
                           Result = result,
                           ResultCode = Result.Ok
                       };
            }
            catch (Exception e)
            {
                _logger.Error($"Error on GetMatchingBuyOrders: {e}");
                return new ResultModel<List<BuyOrderModel>>
                       {
                           ResultCode = Result.Error,
                           Error = e.Message
                       };
            }
        }

        public ResultModel<List<BuyOrderModel>> GetBuyOrders()
        {
            try
            {
                var result = _dbContext.BuyOrders.Select(x => new BuyOrderModel()
                {
                    StockId = x.StockId,
                    UserId = x.UserId,
                    Price = x.Price
                }).ToList();

                return new ResultModel<List<BuyOrderModel>>
                {
                    Result = result,
                    ResultCode = Result.Ok
                };
            }
            catch (Exception e)
            {
                _logger.Error($"Error on GetBuyOrders: {e}");
                return new ResultModel<List<BuyOrderModel>>
                {
                    ResultCode = Result.Error,
                    Error = e.Message
                };
            }
        }
    }
}
