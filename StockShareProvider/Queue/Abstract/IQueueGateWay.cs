namespace StockShareProvider.Queue.Abstract
{
    public interface IQueueGateWay
    {
        void PublishNewSellOrder(string messageBody);
    }
}