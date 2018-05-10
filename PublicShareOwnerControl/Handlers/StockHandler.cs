using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PublicShareOwnerControl.DbAccess;
using PublicShareOwnerControl.DbAccess.Entities;
using Shared;
using Shared.Abstract;
using Shared.Infrastructure;
using Shared.Models;

namespace PublicShareOwnerControl.Handlers
{
    public class StockHandler
    {
        private readonly OwnerControlContext _dbContext;
        private readonly ILogger _log;

        public StockHandler(OwnerControlContext dbcontext, ILogger log)
        {
            _dbContext = dbcontext;
            _log = log;
        }

        public ResultModel UpdateStock(StockModel stockToUpdate)
        {
            try
            {
                var result = _dbContext.Stocks.Single(x => x.StockID.Equals(stockToUpdate.StockID));
                result.UserID = stockToUpdate.NewOwnerID;

                 _dbContext.SaveChanges();

                return new ResultModel { ResultCode = Result.Ok };
            }
            catch(Exception e)
            {
                _log.Error($"Error on UpdateStock: {e}");
                return new ResultModel { ResultCode = Result.Error, Error = e.ToString() };
            }
        }

        public ResultModel<bool> ValidateStockOwnership(StockValidationModel stockValidationModel)
        {
            try
            {
                var matchingStockInDb = _dbContext.Stocks
                    .Where(T => T.StockID == stockValidationModel.StockID && T.UserID == stockValidationModel.UserIdToCheck )
                    .Select(T => new StockModel {StockID = T.StockID, UserID = T.UserID})
                    .ToList();

                if(matchingStockInDb.Count > 0)
                {
                    return new ResultModel<bool> { ResultCode = Result.Ok, Result = true};
                }

                return new ResultModel<bool>{ ResultCode = Result.Error, Error = "User does not have the stock", Result = false};
            }
            catch (Exception e)
            {
                _log.Error($"Error on ValidateStockOwnership: {e}");
                return new ResultModel<bool> { ResultCode = Result.Error, Error = e.ToString(), Result = false };
            }
        }

        public ResultModel<List<StockModel>> GetAllStocks()
        {
            try
            {
                var allStocksInDb = _dbContext.Stocks
                    .OrderBy(T => T.StockID)
                    .Select(T => new StockModel {StockID = T.StockID, StockName = T.StockName, UserID = T.UserID})
                    .ToList();

                return new ResultModel<List<StockModel>>{ ResultCode = Result.Ok, Result = allStocksInDb};
            }
            catch(Exception e)
            {
                _log.Error($"Error on GetAllStocks: {e}");
                return new ResultModel<List<StockModel>> { ResultCode = Result.Error, Error = e.ToString()};
            }
        }
    }
}
