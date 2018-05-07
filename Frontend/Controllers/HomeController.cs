using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Frontend.Models;
using System.Fabric;
using System.Fabric.Query;
using System.Net.Http;
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

        /*
        [HttpPost]
        public async SendBuyRequest([FromBody]StockModel stockModel)
        {
            Uri serviceName = Frontend.GetHTTPGatewayServiceName(_serviceContext);
            Uri proxyAddress = this.GetProxyAddress(serviceName);

            ServicePartitionList partitions = await _fabricClient.QueryManager.GetPartitionListAsync(serviceName);

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
                catch (Exception e)
                {

                }

                return null;
            }
        }*/

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
