using System;
using System.Collections.Generic;
using System.Linq;
using Shared.Abstract;
using Shared.Infrastructure;
using Shared.Models;
using StockShareProvider.DbAccess;
using StockShareProvider.DbAccess.Entities;

namespace StockShareProvider.Handlers
{
    public class SellOrderHandler
    {
        private readonly ProviderContext _dbContext;
        private readonly ILogger _log;

        public SellOrderHandler(ProviderContext dbContext, ILogger log)
        {
            _dbContext = dbContext;
            _log = log;
        }

        public ResultModel InsertSellOrder(SellOrderModel insertModel)
        {
            var usersSellOrders = _dbContext.SellOrders.Where(t => t.UserID.Equals(insertModel.UserID)).ToList();

            if(HasEqualSellOrders(usersSellOrders, insertModel))
            {
                return new ResultModel
                       {
                           ResultCode = Result.Error,
                           Error = "User has existing duplicate sell order"
                       };
            }

            // TODO Sell order added even though there might be existing sell order of differenct price value
            // might be better or overwrite existing sell order?
            _dbContext.SellOrders.Add(new SellOrder()
                                      {
                                          UserID = insertModel.UserID,
                                          SellPrice = insertModel.SellPrice,
                                          StockID = insertModel.StockID,
                                          CreateTime = DateTime.UtcNow
                                      });
            _dbContext.SaveChanges();

            return new ResultModel
                   {
                       ResultCode = Result.Ok
                   };
        }

        public ResultModel<List<SellOrderModel>> MatchingSellOrders(int stockId)
        {
            try
            {
                var matchingBuyOrders = _dbContext.SellOrders.Where(t => t.StockID.Equals(stockId)).Select(t => new SellOrderModel()
                                                                                                                {
                                                                                                                    UserID = t.UserID,
                                                                                                                    StockID = t.StockID,
                                                                                                                    SellPrice = t.SellPrice
                                                                                                                }).ToList();
                return new ResultModel<List<SellOrderModel>>
                       {
                           Result = matchingBuyOrders,
                           ResultCode = Result.Ok
                       };
            }
            catch (Exception e)
            {
                _log.Error($"Error on MatchingSellOrders: {e}");
                return new ResultModel<List<SellOrderModel>>()
                       {
                           Error = e.Message,
                           ResultCode = Result.Error
                       };
            }
        }
        public ResultModel<List<SellOrderModel>> GetSellOrders()
        {
            try
            {
                var matchingBuyOrders = _dbContext.SellOrders.Select(t => new SellOrderModel()
                {
                    UserID = t.UserID,
                    StockID = t.StockID,
                    SellPrice = t.SellPrice
                }).ToList();
                return new ResultModel<List<SellOrderModel>>
                {
                    Result = matchingBuyOrders,
                    ResultCode = Result.Ok
                };
            }
            catch (Exception e)
            {
                _log.Error($"Error on GetSellOrders: {e}");
                return new ResultModel<List<SellOrderModel>>()
                {
                    Error = e.Message,
                    ResultCode = Result.Error
                };
            }
        }

        private bool HasEqualSellOrders(List<SellOrder> usersSellOrders, SellOrderModel insertModel)
        {
            return usersSellOrders.Any(t =>
                t.StockID.Equals(insertModel.StockID) && t.SellPrice.Equals(insertModel.SellPrice));
        }   
    }
}