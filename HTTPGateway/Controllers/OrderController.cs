﻿using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared.Abstract;
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
        private readonly ILogger _log;

        public OrderController(HttpClient httpClient, FabricClient fabricClient, StatelessServiceContext serviceContext, ILogger log)
        {
            _httpClient = httpClient;
            _fabricClient = fabricClient;
            _serviceContext = serviceContext;
            _log = log;
        }

        [HttpPost("Buy")]
        public async Task<IActionResult> Buy([FromBody] BuyOrderModel model)
        {
            _log.Info("Buy called");

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
                    if (!response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        return BadRequest(content);
                    }
                }
            }
            return Ok();
        }

        [HttpPost("Sell")]
        public async Task<IActionResult> Sell([FromBody] SellOrderModel model)
        {
            _log.Info("Sell called");

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
                    if (!response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        return BadRequest(content);
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