using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StockShareRequester.Models
{
    public class BuyOrder
    {
        [Key]
        public int Id { get; set; }
        public int StockId { get; set; }
        public int UserId { get; set; }
        public decimal Price { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
