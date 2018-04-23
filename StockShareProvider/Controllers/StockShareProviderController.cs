using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HTTPGateway.Controllers
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
