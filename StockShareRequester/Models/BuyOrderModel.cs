using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockShareRequester.Models
{
    public class BuyOrderModel
    {
        public BuyOrderModel()
        {

        }
        public BuyOrderModel(int stockId, int userId, decimal buyPrice)
        {
            StockId = stockId;
            UserId = userId;
            Price = buyPrice;
        }

        public int StockId { get; set; }
        public int UserId { get; set; }
        public decimal Price { get; set; }
    }
}
