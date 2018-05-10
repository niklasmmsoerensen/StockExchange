using System;
using System.Fabric;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared.Infrastructure;
using Shared.Models;
using StockShareProvider.Queue.Abstract;
using SellOrderHandler = StockShareProvider.Handlers.SellOrderHandler;

namespace StockShareProvider.Controllers
{
    [Route("api/[controller]")]
    public class SellOrderController : Controller
    {
        private readonly SellOrderHandler _handler;
        private readonly IQueueGateway _queueGateWay;
        private readonly HttpClient _httpClient;
        private readonly FabricClient _fabricClient;
        private readonly StatelessServiceContext _serviceContext;

        public SellOrderController(HttpClient httpClient, FabricClient fabricClient, StatelessServiceContext serviceContext,
            SellOrderHandler handler, IQueueGateway mqChannel)
        {
            _httpClient = httpClient;
            _fabricClient = fabricClient;
            _serviceContext = serviceContext;
            _handler = handler;
            _queueGateWay = mqChannel;
        }

        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] SellOrderModel insertModel)
        {
            try
            {
                var validatedResult = ValidateOwnerShip(insertModel);
                var validateContent = validatedResult.Result.Content.ReadAsStringAsync();

                if (!validatedResult.Result.IsSuccessStatusCode)
                {
                    return BadRequest(validateContent.Result);
                }
                if(validateContent.Result.Equals("false"))
                {
                    return BadRequest("User " + insertModel.UserID + " does not own stock with ID " + insertModel.StockID);
                }

                var resultModel = _handler.InsertSellOrder(insertModel);

                if (resultModel.ResultCode == Result.Error)
                {
                    return BadRequest(resultModel.Error);
                }

                _queueGateWay.PublishNewSellOrder(JsonConvert.SerializeObject(insertModel));
                
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(new ResultModel(Result.Error, e.ToString()));
            }
        }

        [HttpGet("GetMatchingSellOrders/{stockId}")]
        public IActionResult MatchingSellOrders(int stockId)
        {
            var result = _handler.MatchingSellOrders(stockId);

            if (result.ResultCode == Result.Error)
            {
                return BadRequest(result.Error);
            }

            return new ObjectResult(result.Result);
        }

        private async Task<HttpResponseMessage> ValidateOwnerShip(SellOrderModel insertModel)
        {
            Uri serviceName = ServiceRelated.StockShareProvider.GetPublicShareOwnerControlServiceName(_serviceContext);
            Uri proxyAddress = this.GetProxyAddress(serviceName);

            string requestUrl =
                $"{proxyAddress}/api/Stock/ValidateStockOwnership/{insertModel.StockID}/{insertModel.UserID}";

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