using System;
using System.Collections.Generic;
using System.Linq;
using Shared.Models;
using TobinTaxControl.DbAccess;

namespace TobinTaxControl.Handlers
{
    //public class SellOrderHandler
    //{
    //    private readonly TaxContext _dbContext;

    //    public SellOrderHandler(TaxContext dbContext)
    //    {
    //        _dbContext = dbContext;
    //    }

    //    public ResultModel<SellOrderModel> InsertSellOrder(SellOrderModel insertModel)
    //    {
    //        var usersSellOrders = _dbContext.SellOrders.Where(t => t.UserID.Equals(insertModel.UserID)).ToList();

    //        if(HasEqualSellOrders(usersSellOrders, insertModel))
    //        {
    //            return new ResultModel<SellOrderModel>
    //                   {
    //                       ResultCode = Result.Error,
    //                       Error = "User has existing duplicate sell order"
    //                   };
    //        }

    //        // Sell order added even though there might be existing sell order of differenct price value
    //        _dbContext.SellOrders.Add(new SellOrder()
    //                                  {
    //                                      UserID = insertModel.UserID,
    //                                      SellPrice = insertModel.SellPrice,
    //                                      StockID = insertModel.StockID,
    //                                      CreateTime = DateTime.UtcNow
    //                                  });
    //        _dbContext.SaveChanges();

    //        return new ResultModel<SellOrderModel>
    //               {
    //                   ResultCode = Result.Ok
    //               };
    //    }

    //    public ResultModel<List<SellOrderModel>> Matching(int stockId)
    //    {
    //        try
    //        {
    //            var matchingBuyOrders = _dbContext.SellOrders.Where(t => t.StockID.Equals(stockId)).Select(t => new SellOrderModel()
    //                                                                                                            {
    //                                                                                                                UserID = t.UserID,
    //                                                                                                                StockID = t.StockID,
    //                                                                                                                SellPrice = t.SellPrice
    //                                                                                                            }).ToList();
    //            return new ResultModel<List<SellOrderModel>>
    //                   {
    //                       Result = matchingBuyOrders,
    //                       ResultCode = Result.Ok
    //                   };
    //        }
    //        catch (Exception e)
    //        {
    //            return new ResultModel<List<SellOrderModel>>()
    //                   {
    //                       Error = e.Message,
    //                       ResultCode = Result.Error
    //                   };
    //        }
    //    }

    //    private bool HasEqualSellOrders(List<SellOrder> usersSellOrders, SellOrderModel insertModel)
    //    {
    //        return usersSellOrders.Any(t =>
    //            t.StockID.Equals(insertModel.StockID) && t.SellPrice.Equals(insertModel.SellPrice));
    //    }   
    //}
}