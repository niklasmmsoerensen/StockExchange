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

namespace Frontend.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly FabricClient _fabricClient;
        private readonly StatelessServiceContext _serviceContext;

        public HomeController(HttpClient httpClient, FabricClient fabricClient, StatelessServiceContext serviceContext)
        {
            _httpClient = httpClient;
            _fabricClient = fabricClient;
            _serviceContext = serviceContext;
        }


        [HttpPost("SendBuyRequest")]
        public async Task<IActionResult> SendBuyRequest(int stockId, int userId, int price)
        {
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
                        return new ObjectResult(new ResultModel(Result.Error, "HTTPGateway returned an error"));
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
            Uri serviceName = Frontend.GetHTTPGatewayServiceName(_serviceContext);
            Uri proxyAddress = this.GetProxyAddress(serviceName);

            ServicePartitionList partitions = await _fabricClient.QueryManager.GetPartitionListAsync(serviceName);

            List<string> result = new List<string>();


            string proxyUrl =
                $"{proxyAddress}/api/Test";

            using (HttpResponseMessage response = await this._httpClient.GetAsync(proxyUrl))
            {
                var resultTest = response.Content.ReadAsStringAsync();

                try
                {
                    var tempStockModel = JsonConvert.DeserializeObject<List<StockModel>>(resultTest.Result);

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
