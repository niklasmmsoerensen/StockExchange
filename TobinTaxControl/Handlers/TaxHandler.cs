using System;
using System.Collections.Generic;
using System.Linq;
using Shared.Infrastructure;
using Shared.Models;
using TobinTaxControl.DbAccess;
using TobinTaxControl.DbAccess.Entities;

namespace TobinTaxControl.Handlers
{
    public class TaxHandler
    {
        private readonly TaxContext _dbContext;
        private readonly decimal _taxPercentage = 1;

        public TaxHandler(TaxContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ResultModel<TaxationModel> TaxTransaction(TransactionModel insertModel)
        {
            try
            {
                decimal taxSum = insertModel.Price * _taxPercentage / 100;
                _dbContext.Taxes.Add(new Taxation
                                     {
                                         StockID = insertModel.StockId,
                                         TaxPercentage = _taxPercentage,
                                         TaxSum = taxSum,
                                         UserToTaxID = insertModel.UserId
                                     });

                _dbContext.SaveChanges();

                return new ResultModel<TaxationModel>
                       {
                            ResultCode = Result.Ok,
                            Result = new TaxationModel()
                                     {
                                         UserToTaxID = insertModel.UserId,
                                         TaxSum = taxSum
                            }
                       };
            }
            catch (Exception e)
            {
                return new ResultModel<TaxationModel>
                       {
                           ResultCode = Result.Error,
                           Error = e.Message
                       };
            }
        }
    }
}