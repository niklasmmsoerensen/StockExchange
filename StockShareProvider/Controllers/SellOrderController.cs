using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StockShareProvider.Controllers.Models;
using StockShareProvider.Handlers.Models;
using StockShareProvider.Queue.Abstract;
using SellOrderHandler = StockShareProvider.Handlers.SellOrderHandler;

namespace StockShareProvider.Controllers
{
    [Route("api/[controller]")]
    public class SellOrderController : Controller
    {
        private readonly SellOrderHandler _handler;
        private readonly IQueueGateWay _queueGateWay;

        public SellOrderController(SellOrderHandler handler, IQueueGateWay mqChannel)
        {
            _handler = handler;
            _queueGateWay = mqChannel;
        }

        [HttpPost]
        public IActionResult Insert([FromBody] SellOrderInsertModel insertModel)
        {
            var resultModel = _handler.InsertSellOrder(insertModel);

            if (resultModel.Result == Result.Error)
            {
                return BadRequest(resultModel.Error);
            }

            //_queueGateWay.PublishNewSellOrder(JsonConverter<SellOrderInsertModel>());
            
            return Ok();
        }
    }
}