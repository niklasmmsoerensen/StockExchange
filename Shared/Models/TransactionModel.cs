using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class TransactionModel
    {
        public TransactionModel(int stockId, int userId, decimal price)
        {
            StockId = stockId;
            UserId = userId;
            Price = price;
        }

        public TransactionModel()
        {
            
        }
        
        public int StockId { get; set; }
        public int UserId { get; set; }
        public decimal Price { get; set; }
    }
}
