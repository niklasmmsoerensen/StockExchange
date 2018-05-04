using System.Text;
using RabbitMQ.Client;
using StockShareProvider.Queue.Abstract;

namespace StockShareProvider.Queue
{
    public class QueueGateWay : IQueueGateWay
    {
        private readonly string _routingKey;
        private readonly string _exchange;
        private readonly IModel _channel;

        public QueueGateWay(string routingKey, string exchange, IModel queue)
        {
            _routingKey = routingKey;
            _exchange = exchange;
            _channel = queue;
            
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