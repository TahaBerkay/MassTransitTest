using MassTransit;
using MassTransitTest;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MassTransitTest.Controllers
{
    [ApiController]
    [Route("api/[Controller]/[Action]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public TestController(ILogger<TestController> logger, ISendEndpointProvider sendEndpointProvider)
        {
            _logger = logger;
            _sendEndpointProvider = sendEndpointProvider;
        }

        [HttpPost]
        public async Task Test(QueryObject queryObject)
        {
            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:event-listener"));
            await sendEndpoint.Send<QueryObject>(queryObject);
        }
    }
}
