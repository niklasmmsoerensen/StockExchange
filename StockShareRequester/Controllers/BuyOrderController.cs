using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StockShareRequester.Handlers;
using StockShareRequester.Models;

namespace StockShareRequester.Controllers
{
    [Produces("application/json")]
    [Route("api/BuyOrder")]
    public class BuyOrderController : Controller
    {
        private BuyOrderHandler _handler { get; set; }
        public BuyOrderController(BuyOrderHandler handler)
        {
            _handler = handler;
        }

        [HttpGet]
        public string Get()
        {
            return "ayy lmao";
        }

        [HttpPost("Insert")]
        public IActionResult Insert([FromBody] BuyOrderModel model)
        {
            var result = _handler.InsertBuyOrder(model);
            return CheckResult(result);
        }

        [HttpPost("GetMatchingBuyOrders")]
        public IActionResult GetMatchingBuyOrders([FromBody] BuyOrderModel model)
        {
            var result = _handler.GetMatchingBuyOrders(model);
            return CheckResult(result);
        }

        private IActionResult CheckResult(ResultModel result)
        {
            if (result.Result.Equals(Result.Ok))
            {
                return Ok(result.Error);
            }
            else
            {
                return BadRequest(result.Error);
            }
        }
    }
}