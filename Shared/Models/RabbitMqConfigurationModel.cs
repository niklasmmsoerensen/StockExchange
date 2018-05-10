using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Models
{
    public class RabbitMqConfigurationModel
    {
        public RabbitMqConfigurationModel()
        {
            
        }

        private string _hostName;
        private string _mainExhange;

        //buy related
        private string _buyOrderFulfilledRoutingKey;
        private string _buyOrderFulfilledQueue;
        private string _newBuyOrderRoutingKey;
        private string _newBuyOrderQueue;

        //sell related
        private string _sellOrderFulfilledRoutingKey;
        private string _sellOrderFulfilledQueue;
        private string _newSellOrderRoutingKey;
        private string _newSellOrderQueue;

        public string HostName
        {
            get => _hostName;
            set => _hostName = value;
        }

        public string MainExhange
        {
            get => _mainExhange;
            set => _mainExhange = value;
        }

        public string BuyOrderFulfilledRoutingKey
        {
            get => _buyOrderFulfilledRoutingKey;
            set => _buyOrderFulfilledRoutingKey = value;
        }

        public string BuyOrderFulfilledQueue
        {
            get => _buyOrderFulfilledQueue;
            set => _buyOrderFulfilledQueue = value;
        }

        public string NewBuyOrderRoutingKey
        {
            get => _newBuyOrderRoutingKey;
            set => _newBuyOrderRoutingKey = value;
        }

        public string NewBuyOrderQueue
        {
            get => _newBuyOrderQueue;
            set => _newBuyOrderQueue = value;
        }

        public string SellOrderFulfilledRoutingKey
        {
            get => _sellOrderFulfilledRoutingKey;
            set => _sellOrderFulfilledRoutingKey = value;
        }

        public string SellOrderFulfilledQueue
        {
            get => _sellOrderFulfilledQueue;
            set => _sellOrderFulfilledQueue = value;
        }

        public string NewSellOrderRoutingKey
        {
            get => _newSellOrderRoutingKey;
            set => _newSellOrderRoutingKey = value;
        }

        public string NewSellOrderQueue
        {
            get => _newSellOrderQueue;
            set => _newSellOrderQueue = value;
        }
    }
}
