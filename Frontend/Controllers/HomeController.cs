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

namespace Frontend.Controllers
{
    public class HomeController : Controller
    {
        Logger testlog = new Logger("MarcTest");
        private readonly HttpClient _httpClient;
        private readonly FabricClient _fabricClient;
        private readonly StatelessServiceContext _serviceContext;

        public HomeController(HttpClient httpClient, FabricClient fabricClient, StatelessServiceContext serviceContext)
        {
            _httpClient = httpClient;
            _fabricClient = fabricClient;
            _serviceContext = serviceContext;
        }

        public IActionResult Index()
        {
            return View();
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

        [HttpGet]
        public async Task<IActionResult> GetTest()
        {

            Uri serviceName = Frontend.GetHTTPGatewayServiceName(_serviceContext);
            Uri proxyAddress = this.GetProxyAddress(serviceName);

            ServicePartitionList partitions = await _fabricClient.QueryManager.GetPartitionListAsync(serviceName);

            List<string> result = new List<string>();


            string proxyUrl =
                $"{proxyAddress}/api/Test";

            using (HttpResponseMessage response = await this._httpClient.GetAsync(proxyUrl))
            {
                var testResult = response.Content.ReadAsStringAsync();
                var jsonResult = Json(testResult);
                Debug.WriteLine(jsonResult);
                testlog.Info(jsonResult.Value.ToString());

                return View("Index");
            }
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
