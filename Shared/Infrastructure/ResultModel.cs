namespace Shared.Infrastructure
{
    public class ResultModel
    {
        public ResultModel(Result resultCode, string error = "")
        {
            ResultCode = resultCode;
            Error = error;
        }

        public Result ResultCode { get; set; }
        public string Error { get; set; }
    }


    public class ResultModel<T> : ResultModel
    {
        public ResultModel(Result resultCode, T result, string error = "") : base(resultCode, error)
        {
            Result = result;
        }

        public T Result { get; set; }   
    }
}