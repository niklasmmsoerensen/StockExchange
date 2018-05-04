namespace StockShareProvider.Handlers.Models
{
    public class ResultModel<T>
    {
        public T Result { get; set; }
        public Result ResultCode { get; set; }
        public string Error { get; set; }
    }

    public enum Result { Ok, Error }
}