using System;
using System.Fabric;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared.Infrastructure;
using Shared.Models;
using StockShareTrader.Handlers;
using StockShareTrader.Queue.Abstract;

namespace StockShareTrader.Controllers
{
    [Produces("application/json")]
    [Route("api/Purchase")]
    public class PurchaseController : Controller
    {
        private PurchaseHandler _handler { get; set; }
        private readonly IQueueGateWay _queueGateWay;
        private readonly HttpClient _httpClient;
        private readonly FabricClient _fabricClient;
        private readonly StatelessServiceContext _serviceContext;

        public PurchaseController(HttpClient httpClient, FabricClient fabricClient, StatelessServiceContext serviceContext, PurchaseHandler handler, IQueueGateWay queueGateway)
        {
            _httpClient = httpClient;
            _fabricClient = fabricClient;
            _serviceContext = serviceContext;
            _handler = handler;
            _queueGateWay = queueGateway;
        }

        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] TransactionModel model)
        {
            //tax order
            Uri serviceName = StockShareTrader.GetTobinTaxControlServiceName(_serviceContext);
            Uri proxyAddress = this.GetProxyAddress(serviceName);

            string requestUrl = $"{proxyAddress}/api/Tax";

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
                            return new ObjectResult(new ResultModel(Result.Error, "TobinTax returned an error"));
                        }
                    }
                }

            //change ownership
            serviceName = StockShareTrader.GetPublicShareOwnerControlServiceName(_serviceContext);
            proxyAddress = this.GetProxyAddress(serviceName);

            requestUrl =
                $"{proxyAddress}/api/Stock/UpdateOwnership";

            using (HttpRequestMessage request =
                new HttpRequestMessage(HttpMethod.Post, requestUrl)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new StockModel(model.StockId, model.BuyerUserId)),
                        System.Text.Encoding.UTF8, "application/json")
                })
            {
                using (HttpResponseMessage response = await _httpClient.SendAsync(request))
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        return new ObjectResult(new ResultModel(Result.Error, "PublicShareOwnerControl returned an error" ));
                    }
                }
            }
            
            //insert transaction
            var result = _handler.InsertTransaction(model);

            if (result.ResultCode.Equals(Result.Ok))
            {
                //Let Provider and Requester know order has been fulfilled
                _queueGateWay.PublishSellOrderFulfilled(model.StockId.ToString());
                _queueGateWay.PublishBuyOrderFulfilled(JsonConvert.SerializeObject(model));
                return Ok(result.Error);
            }
            else
            {
                return BadRequest(result.Error);
            }
        }
        private Uri GetProxyAddress(Uri serviceName)
        {
            return new Uri($"http://localhost:19081{serviceName.AbsolutePath}");
        }
    }
}