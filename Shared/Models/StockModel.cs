using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class StockModel
    {
        public StockModel()
        {
        }

        public StockModel(int stockID, int newOwnerID)
        {
            StockID = stockID;
            NewOwnerID = newOwnerID;
        }

        public StockModel(int stockID, string stockName, int userID, int newOwnerID)
        {
            StockID = stockID;
            StockName = stockName;
            UserID = userID;
            NewOwnerID = newOwnerID;
        }

        public int StockID { get; set; }
        public string StockName { get; set; }
        public int UserID { get; set; }
        public int NewOwnerID { get; set; }
    }
}
