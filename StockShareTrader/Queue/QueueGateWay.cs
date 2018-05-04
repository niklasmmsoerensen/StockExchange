using System.Text;
using RabbitMQ.Client;
using StockShareTrader.Queue.Abstract;

namespace StockShareTrader.Queue
{
    public class QueueGateWay : IQueueGateWay
    {
        public string SellOrderFulfilledRoutingKey => _sellOrderFulfilledRoutingKey;

        public string BuyOrderFulfilledRoutingKey => _buyOrderFulfilledRoutingKey;

        public string Exchange => _exchange;

        public IModel Channel => _channel;

        private readonly string _exchange;
        private readonly IModel _channel;
        private readonly string _sellOrderFulfilledRoutingKey;
        private readonly string _buyOrderFulfilledRoutingKey;

        public QueueGateWay(string exchange, IModel channel, string newOrderRoutingKey, string orderFulfilledRoutingKey)
        {
            _sellOrderFulfilledRoutingKey = newOrderRoutingKey;
            _exchange = exchange;
            _channel = channel;
            _buyOrderFulfilledRoutingKey = orderFulfilledRoutingKey;
        }

        public void PublishSellOrderFulfilled(string messageBody)
        {
            Channel.BasicPublish(exchange: Exchange,
                routingKey: SellOrderFulfilledRoutingKey,
                basicProperties: null,
                body: Encoding.UTF8.GetBytes(messageBody));
        }
        public void PublishBuyOrderFulfilled(string messageBody)
        {
            Channel.BasicPublish(exchange: Exchange,
                routingKey: BuyOrderFulfilledRoutingKey,
                basicProperties: null,
                body: Encoding.UTF8.GetBytes(messageBody));
        }
    }
}