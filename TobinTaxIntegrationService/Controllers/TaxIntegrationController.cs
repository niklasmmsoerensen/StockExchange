using Microsoft.AspNetCore.Mvc;
using Shared.Abstract;
using Shared.Models;

namespace TobinTaxIntegrationService.Controllers
{
    [Route("api/[controller]")]
    public class TaxIntegrationController : Controller
    {
        private readonly ILogger _log;

        public TaxIntegrationController(ILogger log)
        {
            _log = log;
        }

        [HttpPost]
        public IActionResult Post([FromBody] TaxationModel taxationModel)
        {
            _log.Info($"TobinTaxIntegration service called with model: UserToTaxID = {taxationModel.UserToTaxID}, TaxSum = {taxationModel.TaxSum}");
            return Ok();
        }
    }
}
