using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StockShareProvider.DbAccess;
using StockShareProvider.DbAccess.Entities;

namespace StockShareProvider.Controllers
{
    [Route("api/[controller]")]
    public class StockShareProviderController : Controller
    {
        [HttpGet]
        public string Get()
        {
            return "ayy lmao";
        }
    }
}
