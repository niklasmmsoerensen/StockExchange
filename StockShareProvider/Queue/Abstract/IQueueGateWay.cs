namespace StockShareProvider.Queue.Abstract
{
    public interface IQueueGateway
    {
        void PublishNewSellOrder(string messageBody);
    }
}