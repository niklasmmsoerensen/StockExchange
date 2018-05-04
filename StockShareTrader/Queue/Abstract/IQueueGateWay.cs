namespace StockShareTrader.Queue.Abstract
{
    public interface IQueueGateWay
    {
        void PublishSellOrderFulfilled(string messageBody);
        void PublishBuyOrderFulfilled(string messageBody);
    }
}