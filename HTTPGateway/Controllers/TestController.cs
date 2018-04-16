using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Query;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HTTPGateway.Controllers
{
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly FabricClient _fabricClient;
        private readonly StatelessServiceContext _serviceContext;

        public TestController(HttpClient httpClient, FabricClient fabricClient, StatelessServiceContext serviceContext)
        {
            _httpClient = httpClient;
            _fabricClient = fabricClient;
            _serviceContext = serviceContext;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            Uri serviceName = HTTPGateway.GetStockShareProviderServiceName(_serviceContext);
            Uri proxyAddress = this.GetProxyAddress(serviceName);

            ServicePartitionList partitions = await _fabricClient.QueryManager.GetPartitionListAsync(serviceName);

            List<string> result = new List<string>();
         

            string proxyUrl =
                $"{proxyAddress}/api/Test";

            using (HttpResponseMessage response = await this._httpClient.GetAsync(proxyUrl))
            {
                var fsfs = response;
            }
            

            return this.Json(result);
        }

        private Uri GetProxyAddress(Uri serviceName)
        {
            return new Uri($"http://localhost:19081{serviceName.AbsolutePath}");
        }

    }
}
