﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PublicShareOwnerControl.Handlers;
using Shared;
using Shared.Models;
using Newtonsoft.Json;
using Shared.Infrastructure;


namespace PublicShareOwnerControl.Controllers
{
    [Route("api/[controller]")]
    public class StockController : Controller
    {
        private StockHandler _handler { get; set; }

        public StockController(StockHandler handler)
        {
            _handler = handler;
        }

        /*
        [HttpGet]
        public string Get()
        {
            return "LetHjemmeside123";
        }*/

        [HttpPost]
        public IActionResult Insert([FromBody]StockModel stockModel)
        {
            var result = _handler.UpdateStock(stockModel);

            if (result.ResultCode == Result.Error)
            {
                return BadRequest(result.Error);
            }

            return Ok();
        }

        [HttpGet]
        public IActionResult GetAllStocks()
        {
            var resultModel = _handler.GetAllStocks();

            if(resultModel.ResultCode == Result.Error)
            {
                return BadRequest(resultModel.Error);
            }

            return new ObjectResult(JsonConvert.SerializeObject(resultModel.Result)); 
        }
    }
}
