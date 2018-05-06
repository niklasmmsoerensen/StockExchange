﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PublicShareOwnerControl.DbAccess;
using PublicShareOwnerControl.DbAccess.Entities;
using Shared;
using Shared.Infrastructure;
using Shared.Models;

namespace PublicShareOwnerControl.Handlers
{
    public class StockHandler
    {
        private readonly OwnerControlContext _dbContext;
    
        public StockHandler(OwnerControlContext dbcontext)
        {
            _dbContext = dbcontext;
        }

        public ResultModel UpdateStock(StockModel stockToUpdate)
        {
            try
            {
                var result = _dbContext.Stocks.Single(x => x.StockID.Equals(stockToUpdate.StockID));
                result.UserID = stockToUpdate.UserID;

                 _dbContext.SaveChanges();

                return new ResultModel { ResultCode = Result.Ok };
            }
            catch(Exception e)
            {
                return new ResultModel { ResultCode = Result.Error, Error = e.ToString() };
            }
        }

        public ResultModel<List<StockModel>> GetAllStocks()
        {
            try
            {
                var allStocksInDb = _dbContext.Stocks.OrderBy(T => T.StockID).Select(T => new StockModel { StockID = T.StockID, StockName = T.StockName, UserID = T.UserID }).ToList();

                return new ResultModel<List<StockModel>>{ ResultCode = Result.Ok, Result = allStocksInDb};
            }
            catch(Exception e)
            {
                return new ResultModel<List<StockModel>> { ResultCode = Result.Error, Error = e.ToString()};
            }
        }

    }
}
