using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Query;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared.Models;

namespace HTTPGateway.Controllers
{
    [Route("api/[controller]")]
    public class DataController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly FabricClient _fabricClient;
        private readonly StatelessServiceContext _serviceContext;

        public DataController(HttpClient httpClient, FabricClient fabricClient, StatelessServiceContext serviceContext)
        {
            _httpClient = httpClient;
            _fabricClient = fabricClient;
            _serviceContext = serviceContext;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            List<StockModel> stocks = new List<StockModel>();
            List<BuyOrderModel> buyOrders = new List<BuyOrderModel>();
            List<SellOrderModel> sellOrders = new List<SellOrderModel>();
            List<TransactionModel> transactions = new List<TransactionModel>();

            Uri serviceName = HTTPGateway.GetPublicShareOwnerControlServiceName(_serviceContext);
            Uri proxyAddress = this.GetProxyAddress(serviceName);

            string proxyUrl =
                $"{proxyAddress}/api/Stock/GetAllStocks";

            using (HttpResponseMessage response = await this._httpClient.GetAsync(proxyUrl))
            {
                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest("GetAllStocks failed");
                }
                var result = await response.Content.ReadAsStringAsync();

                stocks = JsonConvert.DeserializeObject<List<StockModel>>(result);
            }
            
            //get buy orders
            serviceName = HTTPGateway.GetStockShareRequesterServiceName(_serviceContext);
            proxyAddress = this.GetProxyAddress(serviceName);

            proxyUrl =
                $"{proxyAddress}/api/BuyOrder/GetBuyOrders";

            using (HttpResponseMessage response = await this._httpClient.GetAsync(proxyUrl))
            {
                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest("GetBuyOrders failed");
                }
                var result = await response.Content.ReadAsStringAsync();

                buyOrders = JsonConvert.DeserializeObject<List<BuyOrderModel>>(result);
            }
            
            //get sell orders
            serviceName = HTTPGateway.GetStockShareProviderServiceName(_serviceContext);
            proxyAddress = this.GetProxyAddress(serviceName);

            proxyUrl =
                $"{proxyAddress}/api/SellOrder/GetSellOrders";

            using (HttpResponseMessage response = await this._httpClient.GetAsync(proxyUrl))
            {
                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest("GetSellOrders failed");
                }

                var result = await response.Content.ReadAsStringAsync();

                sellOrders = JsonConvert.DeserializeObject<List<SellOrderModel>>(result);
            }

            //get transactions
            serviceName = HTTPGateway.GetStockShareTraderServiceName(_serviceContext);
            proxyAddress = this.GetProxyAddress(serviceName);

            proxyUrl =
                $"{proxyAddress}/api/Purchase/GetTransactions";

            using (HttpResponseMessage response = await this._httpClient.GetAsync(proxyUrl))
            {
                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest("GetTransactions failed");
                }

                var result = await response.Content.ReadAsStringAsync();

                transactions = JsonConvert.DeserializeObject<List<TransactionModel>>(result);
            }

            StockDataModel stocksData = new StockDataModel()
            {
                Stocks = stocks,
                BuyOrders = buyOrders,
                SellOrders = sellOrders,
                Transactions = transactions
            };
            return Ok(stocksData);
        }

        private Uri GetProxyAddress(Uri serviceName)
        {
            return new Uri($"http://localhost:19081{serviceName.AbsolutePath}");
        }

    }
}
