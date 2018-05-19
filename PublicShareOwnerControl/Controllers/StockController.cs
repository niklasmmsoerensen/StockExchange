using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PublicShareOwnerControl.Handlers;
using Shared;
using Shared.Models;
using Newtonsoft.Json;
using Shared.Abstract;
using Shared.Infrastructure;


namespace PublicShareOwnerControl.Controllers
{
    [Route("api/[controller]")]
    public class StockController : Controller
    {
        private readonly StockHandler _handler;
        private readonly ILogger _log;

        public StockController(StockHandler handler, ILogger log)
        {
            _handler = handler;
            _log = log;
        }

        [HttpGet("ValidateStockOwnership/{stockId}/{userIdToCheck}")]
        public IActionResult ValidateStockOwnership(int stockId, int userIdToCheck)
        {
            _log.Info("ValidateStockOwnership called");

            var model = new StockValidationModel(stockId, userIdToCheck);
            var result = _handler.ValidateStockOwnership(model);

            if (result.ResultCode.Equals(Result.Ok))
            {
                return new ObjectResult(result.Result);
            }
            else
            {
                return BadRequest(result.Error);
            }
        }

        [HttpPost("UpdateOwnership")]
        public IActionResult UpdateOwnership([FromBody]StockModel stockModel)
        {
            _log.Info("UpdateOwnership called");

            var result = _handler.UpdateStock(stockModel);

            if (result.ResultCode == Result.Error)
            {
                return BadRequest(result.Error);
            }
            else
            {
                return Ok();
            }
        }

        [HttpGet("GetAllStocks")]
        public IActionResult GetAllStocks()
        {
            _log.Info("GetAllStocks called");

            var resultModel = _handler.GetAllStocks();

            if(resultModel.ResultCode == Result.Error)
            {
                return BadRequest(resultModel.Error);
            }

            return new ObjectResult(JsonConvert.SerializeObject(resultModel.Result)); 
        }
    }
}
