using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MassTransitTest
{
    public interface IConsumer
    {
    }

    class Consumer : IConsumer<Batch<QueryObject>>
    {
        private readonly ILogger<Consumer> _logger;
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public Consumer(ILogger<Consumer> logger, ISendEndpointProvider sendEndpointProvider)
        {
            _logger = logger;
            _sendEndpointProvider = sendEndpointProvider;
        }

        public async Task Consume(ConsumeContext<Batch<QueryObject>> context)
        {
            _logger.LogError($"received Length: {context.Message.Length}");
            var List = new List<QueryObject>();
            for (int i = 0; i < context.Message.Length; i++)
            {
                _logger.LogError($"received plate: {context.Message[0].Message}");
                List.Add(context.Message[0].Message);
            }
            _logger.LogError($"ProcessPostRequest done count: {List.Count}");
        }
    }

    class ConsumerDefinition : ConsumerDefinition<Consumer>
    {

        public ConsumerDefinition()
        {
            Endpoint(x => x.PrefetchCount = 100);
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<Consumer> consumerConfigurator)
        {
            consumerConfigurator.Options<BatchOptions>(options => options
                .SetMessageLimit(5)
                .SetConcurrencyLimit(5)
                .SetTimeLimit(10));
        }
    }
}
