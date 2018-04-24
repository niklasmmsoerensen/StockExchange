using System.ComponentModel.DataAnnotations;

namespace StockShareProvider.DbAccess.Entities
{
    public class SellOrder
    {
        [Key]
        public int ID { get; set; }
    }
}
