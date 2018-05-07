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
        public PurchaseController(PurchaseHandler handler, IQueueGateWay queueGateway)
        {
            _handler = handler;
            _queueGateWay = queueGateway;
        }

        [HttpPost("New")]
        public IActionResult Insert([FromBody] TransactionModel model)
        {
            var result = _handler.InsertTransaction(model);

            if (result.ResultCode.Equals(Result.Ok))
            {
                //check if order was buy or sell order
                _queueGateWay.PublishSellOrderFulfilled(model.StockId.ToString());
                return Ok(result.Error);
            }
            else
            {
                return BadRequest(result.Error);
            }
        }
    }
}