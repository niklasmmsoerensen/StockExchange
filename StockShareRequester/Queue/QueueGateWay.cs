using System.Text;
using RabbitMQ.Client;
using StockShareRequester.Queue.Abstract;

namespace StockShareRequester.Queue
{
    public class QueueGateWay : IQueueGateWay
    {
        public string NewOrderRoutingKey => _newOrderRoutingKey;

        public string Exchange => _exchange;

        public IModel Channel => _channel;

        public string NewOrderQueue => _newOrderQueue;

        public string OrderFulfilledQueue => _orderFulfilledQueue;

        private readonly string _exchange;
        private readonly IModel _channel;
        private readonly string _newOrderQueue;
        private readonly string _orderFulfilledQueue;
        private readonly string _newOrderRoutingKey;
        private readonly string _orderFulfilledRoutingKey;

        public QueueGateWay(string exchange, IModel queue, string newBuyOrderQueue, string orderFulfilledQueue, string newOrderRoutingKey, string orderFulfilledRoutingKey)
        {
            _newOrderRoutingKey = newOrderRoutingKey;
            _exchange = exchange;
            _channel = queue;
            _newOrderQueue = newBuyOrderQueue;
            _orderFulfilledQueue = orderFulfilledQueue;
            _orderFulfilledRoutingKey = orderFulfilledRoutingKey;
        }

        public void PublishNewBuyOrder(string messageBody)
        {
            _channel.BasicPublish(exchange: _exchange,
                routingKey: NewOrderRoutingKey,
                basicProperties: null,
                body: Encoding.UTF8.GetBytes(messageBody));
        }
    }
}