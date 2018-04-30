using System;
using System.ComponentModel.DataAnnotations;

namespace StockShareProvider.DbAccess.Entities
{
    public class SellOrder
    {
        [Key]
        public int ID { get; set; }
        public int StockID { get; set; }
        public int UserID { get; set; }
        public decimal SellPrice { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
