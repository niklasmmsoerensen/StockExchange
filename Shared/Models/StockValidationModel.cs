using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Models
{
    public class StockValidationModel
    {
        public StockValidationModel(int stockID, int userIdToCheck)
        {
            StockID = stockID;
            UserIdToCheck = userIdToCheck;
        }
        public int StockID { get; set; }
        public int UserIdToCheck { get; set; }
    }
}
