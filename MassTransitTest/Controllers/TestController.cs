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
        private readonly IBus _bus;

        public TestController(ILogger<TestController> logger, IBus bus)
        {
            _logger = logger;
            _bus = bus;
        }

        [HttpPost]
        public async Task Test(QueryObject queryObject)
        {
            var endpoint = await _bus.GetSendEndpoint(new Uri("queue:event-listener"));

            // Set CorrelationId using SendContext<T>
            await endpoint.Send(queryObject);
        }
    }
}
