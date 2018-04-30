using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockShareProvider.Controllers.Models;
using StockShareProvider.Handlers;


namespace StockShareProvider.Controllers
{
    [Route("api/[controller]")]
    public class SellOrderController : Controller
    {
        private readonly SellOrderHandler _handler;

        public SellOrderController(SellOrderHandler handler)
        {
            _handler = handler;
        }

        [HttpPost]
        public ActionResult Insert([FromBody] SellOrderInsertModel insertModel)
        {
            var resultModel = _handler.InsertSellOrder(insertModel);

            if (resultModel.Result == SellOrderHandler.Result.Error)
            {
                return BadRequest(resultModel.Error);
            }

            // TODO add event to queue
            return Ok();
        }
    }

    public interface IServiceGateWay
    {

    }
}
