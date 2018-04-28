using Microsoft.AspNetCore.Mvc;

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
