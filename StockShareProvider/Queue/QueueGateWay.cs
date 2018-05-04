using System.Text;
using RabbitMQ.Client;
using StockShareProvider.Queue.Abstract;

namespace StockShareProvider.Queue
{
    public class QueueGateway : IQueueGateway
    {
        private readonly IModel _channel;
        private readonly string _exchange;
        private readonly string _newSellOrderRoutingKey;
        
        public QueueGateway(IModel channel, string exchange, string newSellOrderRoutingKey)
        {
            _channel = channel;
            _exchange = exchange;
            _newSellOrderRoutingKey = newSellOrderRoutingKey;
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