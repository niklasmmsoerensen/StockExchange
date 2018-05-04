using System.Text;
using RabbitMQ.Client;
using StockShareRequester.Queue.Abstract;

namespace StockShareRequester.Queue
{
    public class QueueGateWay : IQueueGateWay
    {
        private readonly string _routingKey;

        public string RoutingKey => _routingKey;

        public string Exchange => _exchange;

        public IModel Channel => _channel;

        public string Queue => _queue;

        private readonly string _exchange;
        private readonly IModel _channel;
        private readonly string _queue;

        public QueueGateWay(string routingKey, string exchange, IModel queue, string newBuyOrderQueue)
        {
            _routingKey = routingKey;
            _exchange = exchange;
            _channel = queue;
            _queue = newBuyOrderQueue;

        }

        public void PublishNewBuyOrder(string messageBody)
        {
            _channel.BasicPublish(exchange: _exchange,
                routingKey: _routingKey,
                basicProperties: null,
                body: Encoding.UTF8.GetBytes(messageBody));
        }
    }
}