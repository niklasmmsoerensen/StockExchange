using System;
using System.Fabric;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HTTPGateway.Controllers
{
    [Route("api/[controller]")]
    public class QueueController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly FabricClient _fabricClient;
        private readonly StatelessServiceContext _serviceContext;

        public QueueController(HttpClient httpClient, FabricClient fabricClient, StatelessServiceContext serviceContext)
        {
            _httpClient = httpClient;
            _fabricClient = fabricClient;
            _serviceContext = serviceContext;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var jsonResult = this.Json("Queue controller");

            return jsonResult;
        }

        private Uri GetProxyAddress(Uri serviceName)
        {
            return new Uri($"http://localhost:19081{serviceName.AbsolutePath}");
        }
    }
}
