using System.Text;
using RabbitMQ.Client;
using StockShareProvider.Queue.Abstract;

namespace StockShareProvider.Queue
{
    public class QueueGateWay : IQueueGateWay
    {
        private readonly IModel _channel;
        private readonly string _exchange;

        private readonly string _newSellOrderRoutingKey;

        private readonly string _sellOrderFulfilledRoutingKey;
        private readonly string _sellOrderFulfilledQueue;

        
        public QueueGateWay(IModel channel, string exchange, string newSellOrderRoutingKey, 
            string sellOrderFulfilledRoutingKey, string sellOrderFulfilledQueue)
        {
            _channel = channel;
            _exchange = exchange;
            _newSellOrderRoutingKey = newSellOrderRoutingKey;
            _sellOrderFulfilledRoutingKey = sellOrderFulfilledRoutingKey;
            _sellOrderFulfilledQueue = sellOrderFulfilledQueue;
        }

        public void PublishNewSellOrder(string messageBody)
        {
            _channel.BasicPublish(exchange: _exchange,
                routingKey: _newSellOrderRoutingKey,
                basicProperties: null,
                body: Encoding.UTF8.GetBytes(messageBody));
        }
    }
}