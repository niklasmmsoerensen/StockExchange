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

        [HttpPost]
        public string Insert([FromBody] BuyOrderModel model)
        {
            _handler.InsertBuyOrder(model);

            return "";
        }

    }
}