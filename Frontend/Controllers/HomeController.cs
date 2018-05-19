using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Frontend.Models;
using System.Fabric;
using System.Fabric.Query;
using Shared.Infrastructure;
using Shared;
using Shared.Models;
using Newtonsoft.Json;
using Shared.Abstract;

namespace Frontend.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly FabricClient _fabricClient;
        private readonly StatelessServiceContext _serviceContext;
        private readonly ILogger _log;

        public HomeController(HttpClient httpClient, FabricClient fabricClient, StatelessServiceContext serviceContext, ILogger log)
        {
            _httpClient = httpClient;
            _fabricClient = fabricClient;
            _serviceContext = serviceContext;
            _log = log;
        }


        [HttpPost("SendBuyRequest")]
        public async Task<IActionResult> SendBuyRequest(int stockId, int userId, int price)
        {
            _log.Info("SendBuyRequest called");

            Uri serviceName = Frontend.GetHTTPGatewayServiceName(_serviceContext);
            Uri proxyAddress = this.GetProxyAddress(serviceName);

            string requestUrl =
                $"{proxyAddress}/api/Order/Buy";

            BuyOrderModel test = new BuyOrderModel();
            test.StockId = stockId;
            test.UserId = userId;
            test.Price = price;

            using (HttpRequestMessage request =
                new HttpRequestMessage(HttpMethod.Post, requestUrl)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(test),
                        System.Text.Encoding.UTF8, "application/json")
                })
            {
                using (HttpResponseMessage response = await _httpClient.SendAsync(request))
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        return BadRequest("HTTPGateway returned an error");
                    }

                    else
                    {
                        return Ok();
                    }
                }
            }
        }

        [HttpPost("SendSellRequest")]
        public async Task<IActionResult> SendSellRequest(int stockId, int userId, int price)
        {
            _log.Info("SendSellRequest called");

            Uri serviceName = Frontend.GetHTTPGatewayServiceName(_serviceContext);
            Uri proxyAddress = this.GetProxyAddress(serviceName);

            string requestUrl =
                $"{proxyAddress}/api/Order/Sell";

            SellOrderModel test = new SellOrderModel();
            test.StockID = stockId;
            test.UserID = userId;
            test.SellPrice = price;

            using (HttpRequestMessage request =
                new HttpRequestMessage(HttpMethod.Post, requestUrl)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(test),
                        System.Text.Encoding.UTF8, "application/json")
                })
            {
                using (HttpResponseMessage response = await _httpClient.SendAsync(request))
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        return BadRequest("HTTPGateway returned an error");
                    }

                    else
                    {
                        return Ok();
                    }
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            _log.Info("Index called");

            Uri serviceName = Frontend.GetHTTPGatewayServiceName(_serviceContext);
            Uri proxyAddress = this.GetProxyAddress(serviceName);

            ServicePartitionList partitions = await _fabricClient.QueryManager.GetPartitionListAsync(serviceName);

            List<string> result = new List<string>();

            //get all owned stocks, buy orders and sell orders
            string proxyUrl =
                $"{proxyAddress}/api/Data";

            using (HttpResponseMessage response = await this._httpClient.GetAsync(proxyUrl))
            {
                var resultTest = response.Content.ReadAsStringAsync();

                try
                {
                    var tempStockModel = JsonConvert.DeserializeObject<StockDataModel>(resultTest.Result);

                    return View("Index", tempStockModel);
                }
                catch(Exception e)
                {
                    
                }

                return null;
            }
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private Uri GetProxyAddress(Uri serviceName)
        {
            return new Uri($"http://localhost:19081{serviceName.AbsolutePath}");
        }
    }
}
