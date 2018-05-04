namespace StockShareRequester.Queue.Abstract
{
    public interface IQueueGateWay
    {

        void PublishNewBuyOrder(string messageBody);
    }
}