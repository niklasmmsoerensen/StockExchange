using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Query;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared.Abstract;
using Shared.Infrastructure;
using Shared.Models;
using TobinTaxControl.Handlers;

namespace TobinTaxControl.Controllers
{
    [Route("api/[controller]")]
    public class TaxController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly FabricClient _fabricClient;
        private readonly StatelessServiceContext _serviceContext;
        private readonly TaxHandler _taxHandler;
        private readonly ILogger _logger;

        public TaxController(HttpClient httpClient, FabricClient fabricClient, StatelessServiceContext serviceContext, TaxHandler taxHandler, ILogger logger)
        {
            _httpClient = httpClient;
            _fabricClient = fabricClient;
            _serviceContext = serviceContext;
            _taxHandler = taxHandler;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Post([FromBody]TransactionModel transactionModel)
        {
            _logger.Info("Post called");

            var result = _taxHandler.TaxTransaction(transactionModel);

            if (result.ResultCode == Result.Ok)
            {
                var httpResponse = PostToIntegrationService(JsonConvert.SerializeObject(result.Result));

                if (httpResponse.Result.IsSuccessStatusCode)
                {
                    // TODO mark taxation in db as accomplished (needs expanded entity)
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                _logger.Error($"Error on transaction taxing: {result.Error}");
                return BadRequest();
            }            
        }

        private async Task<HttpResponseMessage> PostToIntegrationService(string jsonModel)
        {
            Uri serviceName = ServiceRelated.TobinTaxControl.GetTobinTaxIntegrationService(_serviceContext);
            Uri proxyAddress = GetProxyAddress(serviceName);

            string proxyUrl = $"{proxyAddress}/api/TaxIntegration";

            HttpRequestMessage request =
                new HttpRequestMessage(HttpMethod.Post, proxyUrl)
                {
                    Content = new StringContent(jsonModel,
                        System.Text.Encoding.UTF8, "application/json")
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
