﻿using System;
using System.Collections.Generic;
using System.Fabric;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared.Abstract;
using Shared.Infrastructure;
using Shared.Models;
using StockShareProvider.Queue.Abstract;
using SellOrderHandler = StockShareProvider.Handlers.SellOrderHandler;

namespace StockShareProvider.Controllers
{
    [Route("api/[controller]")]
    public class SellOrderController : Controller
    {
        private readonly SellOrderHandler _handler;
        private readonly IQueueGateway _queueGateWay;
        private readonly HttpClient _httpClient;
        private readonly FabricClient _fabricClient;
        private readonly StatelessServiceContext _serviceContext;
        private readonly ILogger _log;

        public SellOrderController(HttpClient httpClient, FabricClient fabricClient, StatelessServiceContext serviceContext,
            SellOrderHandler handler, IQueueGateway mqChannel, ILogger log)
        {
            _httpClient = httpClient;
            _fabricClient = fabricClient;
            _serviceContext = serviceContext;
            _handler = handler;
            _queueGateWay = mqChannel;
            _log = log;
        }

        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] SellOrderModel insertModel)
        {
            try
            {
                //call validate on StockShareOwnerControl
                Uri serviceName = ServiceRelated.StockShareProvider.GetPublicShareOwnerControlServiceName(_serviceContext);
                Uri proxyAddress = this.GetProxyAddress(serviceName);

                string proxyUrl =
                    $"{proxyAddress}/api/Purchase/Insert";

                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, proxyUrl))
                {
                    string json = JsonConvert.SerializeObject(insertModel);

                    request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                    using (HttpResponseMessage response = await this._httpClient.SendAsync(request))
                    {
                        //check if validation was OK
                        var resultTest = response.Content.ReadAsStringAsync();

                    }
                }

                var resultModel = _handler.InsertSellOrder(insertModel);

                if (resultModel.ResultCode == Result.Error)
                {
                    return BadRequest(resultModel.Error);
                }

                _queueGateWay.PublishNewSellOrder(JsonConvert.SerializeObject(insertModel));
                
                return Ok();
            }
            catch (Exception e)
            {
                return Ok(new ResultModel(Result.Error, e.ToString()));
            }
        }

        [HttpGet("Matching")]
        public IActionResult MatchingSellOrders(int stockId)
        {
            var result = _handler.Matching(stockId);

            if (result.ResultCode == Result.Error)
            {
                _log.Error($"Error matching sell order: {result.Error}");
                return BadRequest(result.Error);
            }

            return new ObjectResult(result.Result);
        }

        private Uri GetProxyAddress(Uri serviceName)
        {
            return new Uri($"http://localhost:19081{serviceName.AbsolutePath}");
        }
    }
}