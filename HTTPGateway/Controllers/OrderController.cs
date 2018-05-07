using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared.Infrastructure;
using Shared.Models;

namespace HTTPGateway.Controllers
{
    [Produces("application/json")]
    [Route("api/Order")]
    public class OrderController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly FabricClient _fabricClient;
        private readonly StatelessServiceContext _serviceContext;

        public OrderController(HttpClient httpClient, FabricClient fabricClient, StatelessServiceContext serviceContext)
        {
            _httpClient = httpClient;
            _fabricClient = fabricClient;
            _serviceContext = serviceContext;
        }

        [HttpPost("Buy")]
        public async Task<IActionResult> Buy([FromBody] BuyOrderModel model)
        {
            //Call requester and insert order
            Uri serviceName = HTTPGateway.GetStockShareRequesterServiceName(_serviceContext);
            Uri proxyAddress = this.GetProxyAddress(serviceName);

            string requestUrl = $"{proxyAddress}/api/BuyOrder";

            using (HttpRequestMessage request =
                new HttpRequestMessage(HttpMethod.Post, requestUrl)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(model),
                        System.Text.Encoding.UTF8, "application/json")
                })
            {
                using (HttpResponseMessage response = await _httpClient.SendAsync(request))
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        return new ObjectResult(new ResultModel(Result.Error, "Creating buy order failed"));
                    }
                }
            }
            return Ok();
        }

        [HttpPost("Sell")]
        public async Task<IActionResult> Sell([FromBody] SellOrderModel model)
        {
            //Call provider and insert order
            Uri serviceName = HTTPGateway.GetStockShareProviderServiceName(_serviceContext);
            Uri proxyAddress = this.GetProxyAddress(serviceName);

            string requestUrl = $"{proxyAddress}/api/SellOrder";

            using (HttpRequestMessage request =
                new HttpRequestMessage(HttpMethod.Post, requestUrl)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(model),
                        System.Text.Encoding.UTF8, "application/json")
                })
            {
                using (HttpResponseMessage response = await _httpClient.SendAsync(request))
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    ResultModel<bool> result = JsonConvert.DeserializeObject<ResultModel<bool>>(content);
                    if (result?.Result == false) //something went wrong creating sell order
                    {
                        return new ObjectResult(result);
                    }
                }
            }
            return Ok();
        }

        private Uri GetProxyAddress(Uri serviceName)
        {
            return new Uri($"http://localhost:19081{serviceName.AbsolutePath}");
        }
    }
}