using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Models
{
    public class StockDataModel
    {
        public StockDataModel()
        {
            
        }
        public List<StockModel> Stocks { get; set; }
        public List<BuyOrderModel> BuyOrders { get; set; }
        public List<SellOrderModel> SellOrders { get; set; }
    }
}
