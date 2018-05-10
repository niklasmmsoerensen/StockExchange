using System;
using System.Fabric;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared.Abstract;
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
        private readonly PurchaseHandler _handler;
        private readonly IQueueGateWay _queueGateWay;
        private readonly HttpClient _httpClient;
        private readonly FabricClient _fabricClient;
        private readonly StatelessServiceContext _serviceContext;
        private readonly ILogger _log;

        public PurchaseController(HttpClient httpClient, FabricClient fabricClient, StatelessServiceContext serviceContext,
            PurchaseHandler handler, IQueueGateWay queueGateway, ILogger log)
        {
            _httpClient = httpClient;
            _fabricClient = fabricClient;
            _serviceContext = serviceContext;
            _handler = handler;
            _queueGateWay = queueGateway;
            _log = log;
        }

        [HttpGet("GetTransactions")]
        public IActionResult GetTransactions()
        {
            var result = _handler.GetTransactions();
            if (result.ResultCode == Result.Ok)
            {
                return Ok(result.Result);
            }
            else
            {
                return BadRequest(result.Error);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] TransactionModel model)
        {
            //tax order
            var taxResult = TaxOrder(model);
            if (!taxResult.Result.IsSuccessStatusCode)
            {
                _log.Error("Insert Purchase - tax order failed");
                return BadRequest(taxResult.Result.Content.ReadAsStringAsync());
            }

            //change ownership - TODO burde det forrige annulleres hvis dette fejler?
            var changeOwnershipResult = ChangeOwnership(model);
            if (!changeOwnershipResult.Result.IsSuccessStatusCode)
            {
                _log.Error("Insert Purchase - ownership change failed");
                return BadRequest(changeOwnershipResult.Result.Content.ReadAsStringAsync());
            }
            
            //insert transaction
            var result = _handler.InsertTransaction(model);

            if (result.ResultCode.Equals(Result.Ok))
            {
                //Let Provider and Requester know order has been fulfilled
                _queueGateWay.PublishSellOrderFulfilled(model.StockId.ToString());
                _queueGateWay.PublishBuyOrderFulfilled(model.StockId.ToString());
                return Ok(result.Error);
            }
            else
            {
                return BadRequest(result.Error);
            }
        }

        private async Task<HttpResponseMessage> TaxOrder(TransactionModel model)
        {
            Uri serviceName = StockShareTrader.GetTobinTaxControlServiceName(_serviceContext);
            Uri proxyAddress = this.GetProxyAddress(serviceName);

            string requestUrl = $"{proxyAddress}/api/Tax";
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestUrl)
                                                {
                                                    Content = new StringContent(JsonConvert.SerializeObject(model),
                                                        System.Text.Encoding.UTF8, "application/json")
                                                })
            {
                HttpResponseMessage response = await _httpClient.SendAsync(request);
                return response;
            }
        }

        private async Task<HttpResponseMessage> ChangeOwnership(TransactionModel model)
        {
            var serviceName = StockShareTrader.GetPublicShareOwnerControlServiceName(_serviceContext);
            var proxyAddress = this.GetProxyAddress(serviceName);

            var requestUrl =
                $"{proxyAddress}/api/Stock/UpdateOwnership";

            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestUrl)
                                                {
                                                    Content = new StringContent(
                                                        JsonConvert.SerializeObject(new StockModel(model.StockId, model.BuyerUserId)),
                                                        System.Text.Encoding.UTF8, "application/json")
                                                })
            {
                HttpResponseMessage response = await _httpClient.SendAsync(request);
                return response;
            }
        }

        private Uri GetProxyAddress(Uri serviceName)
        {
            return new Uri($"http://localhost:19081{serviceName.AbsolutePath}");
        }
    }
}