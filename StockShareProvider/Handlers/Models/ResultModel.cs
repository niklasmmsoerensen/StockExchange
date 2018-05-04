namespace StockShareProvider.Handlers.Models
{
    public class ResultModel
    {
        public Result Result { get; set; }
        public string Error { get; set; }
    }

    public enum Result { Ok, Error }
}