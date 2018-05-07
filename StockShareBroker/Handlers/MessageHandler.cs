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

                // TODO mulighed for udvidelse af business logik
                // TODO f.eks. udvid så den først oprettede buyorder går igennem i stedet for bare en random (firstordefault)
                var matchingBuyOrder = existingBuyOrdersForStock.Result.FirstOrDefault(buyOrder => buyOrder.Price >= newSellOrder.SellPrice);

                if (matchingBuyOrder != null)
                {
                    var transactionModel = new TransactionModel
                                           {
                                               StockId = matchingBuyOrder.StockId,
                                               UserId = matchingBuyOrder.UserId,
                                               Price = newSellOrder.SellPrice
                                           };
                    var response = PostPurchase(transactionModel);

                    if (!response.Result.IsSuccessStatusCode)
                    {
                        _log.Error($"NewSellOrderHandler - Error posting data to StockShareTrader - StatusCode = {response.Result.StatusCode}");
                    }
                }
            }
            catch (Exception e)
            {
                _log.Error($"Error on NewSellOrderHandler: {e.Message}");
            }
        }

        public void NewBuyOrderHandler(byte[] messageBody)
        {
            try
            {
                var messageBodyString = Encoding.UTF8.GetString(messageBody);
                var newBuyOrder = JsonConvert.DeserializeObject<BuyOrderModel>(messageBodyString);

                var existingSellOrdersForStock = GetExistingSellOrdersForStock(newBuyOrder.StockId);

                // TODO mulighed for udvidelse af business logik
                var matchingSellOrder = existingSellOrdersForStock.Result.FirstOrDefault(sellOrder => sellOrder.SellPrice <= newBuyOrder.Price);

                if (matchingSellOrder != null)
                {
                    var transactionModel = new TransactionModel
                                           {
                                               StockId = matchingSellOrder.StockID,
                                               UserId = matchingSellOrder.UserID,
                                               Price = matchingSellOrder.SellPrice
                                           };

                    var response = PostPurchase(transactionModel);

                    if (!response.Result.IsSuccessStatusCode)
                    {
                        _log.Error($"NewBuyOrderHandler - Error posting data to StockShareTrader - StatusCode = {response.Result.StatusCode}");
                    }
                }
            }
            catch (Exception e)
            {
                _log.Error($"Error on NewSellOrderHandler: {e.Message}");
            }
        }

        private async Task<List<BuyOrderModel>> GetExistingBuyOrdersForStock(int stockId)
        {
            Uri serviceName = ServiceRelated.StockShareBroker.GetStockShareRequesterService(_serviceContext);
            Uri proxyAddress = GetProxyAddress(serviceName);

            string proxyUrl = $"{proxyAddress}/api/BuyOrder/GetMatchingBuyOrders/{stockId}";

            using (HttpResponseMessage response = await _httpClient.GetAsync(proxyUrl))
            {
                var jsonStringResult = response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<BuyOrderModel>>(jsonStringResult.ToString());
            }
        }

        private async Task<List<SellOrderModel>> GetExistingSellOrdersForStock(int stockId)
        {
            Uri serviceName = ServiceRelated.StockShareBroker.GetStockShareProviderService(_serviceContext);
            Uri proxyAddress = GetProxyAddress(serviceName);

            string proxyUrl = $"{proxyAddress}/api/SellOrder/GetMatchingSellOrders/{stockId}";
            
            using (HttpResponseMessage response = await _httpClient.GetAsync(proxyUrl))
            {
                var jsonStringResult = response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<SellOrderModel>>(jsonStringResult.ToString());
            }
        }

        private async Task<HttpResponseMessage> PostPurchase(TransactionModel transactionModel)
        {
            Uri serviceName = ServiceRelated.StockShareBroker.GetStockShareTraderSerivce(_serviceContext);
            Uri proxyAddress = GetProxyAddress(serviceName);

            string proxyUrl = $"{proxyAddress}/api/Purchase";

            var jsonModel = JsonConvert.SerializeObject(transactionModel);

            HttpRequestMessage request =
                new HttpRequestMessage(HttpMethod.Post, proxyUrl)
                {
                    Content = new StringContent(jsonModel,
                        Encoding.UTF8, "application/json")
                };

            using (HttpResponseMessage response = await _httpClient.SendAsync(request))
            {
                return response;
            }
        }


        private Uri GetProxyAddress(Uri serviceName)
        {
            return new Uri($"http://localhost:19081{serviceName.AbsolutePath}");
        }
    }
}