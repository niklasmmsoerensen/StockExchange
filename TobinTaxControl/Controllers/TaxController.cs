using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace TobinTaxControl.Controllers
{
    [Route("api/[controller]")]
    public class TaxController : Controller
    {
        // GET api/values
        [HttpPost]
        public IActionResult Post([FromBody]TransactionModel transactionModel)
        {
            return BadRequest();
        }

    }
}
