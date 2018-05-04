namespace Shared.Models
{
    public class SellOrderInsertModel
    {
        public int StockID { get; set; }
        public int UserID { get; set; }
        public decimal SellPrice { get; set; }
    }
}