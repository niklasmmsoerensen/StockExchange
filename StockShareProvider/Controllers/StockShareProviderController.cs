using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StockShareProvider.DbAccess;
using StockShareProvider.DbAccess.Entities;

namespace StockShareProvider.Controllers
{
    [Route("api/[controller]")]
    public class StockShareProviderController : Controller
    {
        private readonly ProviderContext _context;

        public TestController(ProviderContext context)
        {
            this._context = context;
        }


        [HttpGet]
        public string Get()
        {
            return "ayy lmao";
        }

        [HttpGet("DbTest")]
        public IActionResult DbTest()
        {
            using (var context = _context)
            {
                context.SellOrders.Add(new SellOrder());
                context.SaveChanges();

                var sellorder = context.SellOrders.FirstOrDefault();
                return sellorder != null ? new ObjectResult(sellorder.ID) : new ObjectResult(0);
            }
        }
    }
}
