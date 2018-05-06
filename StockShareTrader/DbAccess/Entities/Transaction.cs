using System;
using System.ComponentModel.DataAnnotations;

namespace StockShareTrader.DbAccess.Entities
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        public int StockId { get; set; }
        public int UserId { get; set; }
        public decimal Price { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
