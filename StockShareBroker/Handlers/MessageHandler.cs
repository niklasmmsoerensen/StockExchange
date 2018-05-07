using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared.Abstract;
using Shared.Models;
using StockShareBroker.ServiceRelated;

namespace StockShareBroker.Handlers
{
    public class MessageHandler
    {
        private readonly ILogger _log;
        private readonly HttpClient _httpClient;
        private readonly StatelessServiceContext _serviceContext;


        public MessageHandler(ILogger log, HttpClient httpClient, StatelessServiceContext serviceContext)
        {
            _log = log;
            _httpClient = httpClient;
            _serviceContext = serviceContext;
        }

        public void NewSellOrderHandler(byte[] messageBody)
        {
            try
            {
                var messageBodyString = Encoding.UTF8.GetString(messageBody);
                var newSellOrder = JsonConvert.DeserializeObject<SellOrderModel>(messageBodyString);

                var existingBuyOrdersForStock = GetExistingBuyOrdersForStock(newSellOrder.StockID);

                // TODO muligvis udvid så den først oprettede buyorder går igennem i stedet for bare en random (firstordefault)
                var matchingBuyOrder = existingBuyOrdersForStock.Result.Where(t => t.Price >= newSellOrder.SellPrice).ToList().FirstOrDefault();

                if (matchingBuyOrder != null)
                {
                    // create a purchase
                }
                // else do nuffin
            }
            catch (Exception e)
            {
                _log.Error($"Error on NewSellOrderHandler: {e.Message}");
            }
            
        }

        public void NewSellBuyOrder(byte[] messageBody)
        {
            try
            {

            }
            catch (Exception e)
            {
                _log.Error($"Error on NewSellOrderHandler: {e.Message}");
            }
        }

        private async Task<List<BuyOrderModel>> GetExistingBuyOrdersForStock(int stockId)
        {
            Uri serviceName = ServiceRelated.StockShareBroker.GetStockShareProviderService(_serviceContext);
            Uri proxyAddress = GetProxyAddress(serviceName);

            string proxyUrl = $"{proxyAddress}/api/BuyOrder/GetMatchingBuyOrders/{stockId}";

            using (HttpResponseMessage response = await _httpClient.GetAsync(proxyUrl))
            {
                var jsonStringResult = response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<BuyOrderModel>>(jsonStringResult.ToString());
            }
        }

        private async Task<List<BuyOrderModel>> GetExistingSellOrdersForStock(int stockId)
        {
            Uri serviceName = ServiceRelated.StockShareBroker.GetStockShareProviderService(_serviceContext);
            Uri proxyAddress = GetProxyAddress(serviceName);

            string proxyUrl = $"{proxyAddress}/api/BuyOrder/GetMatchingBuyOrders/{stockId}";

            using (HttpResponseMessage response = await _httpClient.GetAsync(proxyUrl))
            {
                var jsonStringResult = response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<BuyOrderModel>>(jsonStringResult.ToString());
            }
        }

        private async Task<List<BuyOrderModel>> PostPurchase(int stockId)
        {
            Uri serviceName = ServiceRelated.StockShareBroker.GetStockShareProviderService(_serviceContext);
            Uri proxyAddress = GetProxyAddress(serviceName);

            string proxyUrl = $"{proxyAddress}/api/BuyOrder/GetMatchingBuyOrders/{stockId}";

            using (HttpResponseMessage response = await _httpClient.GetAsync(proxyUrl))
            {
                var jsonStringResult = response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<BuyOrderModel>>(jsonStringResult.ToString());
            }
        }


        private Uri GetProxyAddress(Uri serviceName)
        {
            return new Uri($"http://localhost:19081{serviceName.AbsolutePath}");
        }
    }
}