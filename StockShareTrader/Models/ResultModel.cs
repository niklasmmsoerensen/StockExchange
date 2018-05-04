using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockShareTrader.Models
{
    public class ResultModel
    {
        public ResultModel(Result result, string error = "")
        {
            Result = result;
            Error = error;
        }

        public Result Result { get; set; }
        public string Error { get; set; }
    }

    public enum Result { Ok, Error }
}
