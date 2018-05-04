﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StockShareTrader.Handlers;
using StockShareTrader.Models;
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

        [HttpPost("Insert")]
        public IActionResult Insert([FromBody] TransactionModel model)
        {
            var result = _handler.InsertTransaction(model);

            if (result.Result.Equals(Result.Ok))
            {
                string jsonBuyOrder = JsonConvert.SerializeObject(model);
                //check if order was buy or sell order
                _queueGateWay.PublishSellOrderFulfilled(jsonBuyOrder);
                return Ok(result.Error);
            }
            else
            {
                return BadRequest(result.Error);
            }
        }
    }
}