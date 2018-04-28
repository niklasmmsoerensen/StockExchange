using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StockShareProvider.DbAccess;
using StockShareProvider.DbAccess.Entities;

namespace StockShareProvider.Controllers
{
    [Route("api/[controller]")]
    public class DbTestController : Controller
    {
        private readonly ProviderContext _context;

        public DbTestController(ProviderContext context)
        {
            _context = context;
        }

        [HttpGet("DoTest")]
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
