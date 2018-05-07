using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared.Abstract;
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
        private readonly ILogger _log;

        public SellOrderController(SellOrderHandler handler, IQueueGateway mqChannel, ILogger log)
        {
            _handler = handler;
            _queueGateWay = mqChannel;
            _log = log;
        }

        [HttpPost]
        public IActionResult Insert([FromBody] SellOrderModel insertModel)
        {
            var result = _handler.InsertSellOrder(insertModel);

            if (result.ResultCode == Result.Error)
            {
                _log.Error($"Error inserting sell order: {result.Error}");
                return BadRequest(result.Error);
            }

            _queueGateWay.PublishNewSellOrder(JsonConvert.SerializeObject(insertModel));
            
            return Ok();
        }

        [HttpGet("Matching")]
        public IActionResult MatchingSellOrders(int stockId)
        {
            var result = _handler.Matching(stockId);

            if (result.ResultCode == Result.Error)
            {
                _log.Error($"Error matching sell order: {result.Error}");
                return BadRequest(result.Error);
            }

            return new ObjectResult(result.Result);
        }
    }
}