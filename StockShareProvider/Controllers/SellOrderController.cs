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

        public SellOrderController(SellOrderHandler handler, IQueueGateway mqChannel)
        {
            _handler = handler;
            _queueGateWay = mqChannel;
        }

        [HttpPost]
        public IActionResult Insert([FromBody] SellOrderModel insertModel)
        {
            var resultModel = _handler.InsertSellOrder(insertModel);

            if (resultModel.ResultCode == Result.Error)
            {
                return BadRequest(resultModel.Error);
            }

            _queueGateWay.PublishNewSellOrder(JsonConvert.SerializeObject(insertModel));
            
            return Ok();
        }

        [HttpGet("Matching")]
        public IActionResult MatchingSellOrders(int stockId)
        {
            var resultModel = _handler.Matching(stockId);

            if (resultModel.ResultCode == Result.Error)
            {
                return BadRequest(resultModel.Error);
            }

            return new ObjectResult(resultModel.Result);
        }
    }
}