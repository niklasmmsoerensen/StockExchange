using System;
using System.Fabric;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared.Abstract;
using Shared.Infrastructure;
using Shared.Models;
using StockShareRequester.Handlers;
using StockShareRequester.Queue.Abstract;

namespace StockShareRequester.Controllers
{
    [Produces("application/json")]
    [Route("api/BuyOrder")]
    public class BuyOrderController : Controller
    {
        private readonly BuyOrderHandler _handler;
        private readonly IQueueGateWay _queueGateWay;
        private readonly StatelessServiceContext _serviceContext;
        private readonly HttpClient _httpClient;
        private readonly ILogger _log;

        public BuyOrderController(BuyOrderHandler handler, IQueueGateWay queueGateway, StatelessServiceContext serviceContext,
            HttpClient httpClient, ILogger log)
        {
            _handler = handler;
            _queueGateWay = queueGateway;
            _serviceContext = serviceContext;
            _httpClient = httpClient;
            _log = log;
        }
        
        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] BuyOrderModel model)
        {
            _log.Info("Insert called");

            var validatedResult = ValidateOwnerShip(model);
            var validateContent = validatedResult.Result.Content.ReadAsStringAsync();

            if (!validatedResult.Result.IsSuccessStatusCode)
            {
                return BadRequest(validateContent.Result);
            }
            if (validateContent.Result.Equals("true")) //you're not allowed to create buy order of stock you own
            {
                return BadRequest("User " + model.UserId + " already owns stock with ID " + model.StockId);
            }

            var result = _handler.InsertBuyOrder(model);

            if (result.ResultCode.Equals(Result.Ok))
            {
                string jsonBuyOrder = JsonConvert.SerializeObject(model);
                _queueGateWay.PublishNewBuyOrder(jsonBuyOrder);
                return Ok();
            }
            else
            {
                return BadRequest(result.Error);
            }
        }

        [HttpGet("GetMatchingBuyOrders/{stockId}")]
        public IActionResult GetMatchingBuyOrders(int stockId)
        {
            _log.Info("GetMatchingBuyOrders called");

            var result = _handler.GetMatchingBuyOrders(stockId);

            if (result.ResultCode == Result.Ok)
            {
                return Ok(result.Result);
            }
            else
            {
                return BadRequest(result.Error);
            }
        }

        [HttpGet("GetBuyOrders")]
        public IActionResult GetBuyOrders()
        {
            _log.Info("GetBuyOrders called");

            var result = _handler.GetBuyOrders();
            if (result.ResultCode == Result.Ok)
            {
                return Ok(result.Result);
            }
            else
            {
                return BadRequest(result.Error);
            }
        }

        private async Task<HttpResponseMessage> ValidateOwnerShip(BuyOrderModel model)
        {
            Uri serviceName = StockShareRequester.GetPublicShareOwnerControlServiceName(_serviceContext);
            Uri proxyAddress = this.GetProxyAddress(serviceName);

            string requestUrl =
                $"{proxyAddress}/api/Stock/ValidateStockOwnership/{model.StockId}/{model.UserId}";

            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUrl))
            {
                HttpResponseMessage response = await this._httpClient.SendAsync(request);
                return response;
            }
        }

        private Uri GetProxyAddress(Uri serviceName)
        {
            return new Uri($"http://localhost:19081{serviceName.AbsolutePath}");
        }
    }
}