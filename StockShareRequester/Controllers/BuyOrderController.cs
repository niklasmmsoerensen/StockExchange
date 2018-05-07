using System;
using System.Fabric;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
        private BuyOrderHandler _handler { get; set; }
        private readonly IQueueGateWay _queueGateWay;

        public BuyOrderController(BuyOrderHandler handler, IQueueGateWay queueGateway)
        {
            _handler = handler;
            _queueGateWay = queueGateway;
        }
        
        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] BuyOrderModel model)
        {
            var result = _handler.InsertBuyOrder(model);

            if (result.ResultCode.Equals(Result.Ok))
            {
                string jsonBuyOrder = JsonConvert.SerializeObject(model);
                _queueGateWay.PublishNewBuyOrder(jsonBuyOrder);
                return Ok(result.Error);
            }
            else
            {
                return BadRequest(result.Error);
            }
        }

        [HttpGet("GetMatchingBuyOrders")]
        public IActionResult GetMatchingBuyOrders(int stockId)
        {
            try
            {
                var result = _handler.GetMatchingBuyOrders(stockId);
                return new ObjectResult(result);
            }
            catch (Exception e)
            {
                return new ObjectResult("GetMatchingBuyOrders exception: " + e);
            }
            
        }

        private Uri GetProxyAddress(Uri serviceName)
        {
            return new Uri($"http://localhost:19081{serviceName.AbsolutePath}");
        }
    }
}