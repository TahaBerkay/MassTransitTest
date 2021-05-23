using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using RabbitMQ.Client.Events;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransitTest
{
    public interface IConsumer
    {
    }

    class Consumer : IConsumer<Batch<QueryObject>>
    {
        public async Task Consume(ConsumeContext<Batch<QueryObject>> context)
        {
            Log.Error($"received {context.Message.Length}");
            //context.Defer(TimeSpan.FromMinutes(1));
            throw new Exception("Very bad things happened");
            //Log.Error($"sent {context.Message.Length}");
            for (int i = 0; i < context.Message.Length; i++)
            {
                ConsumeContext<QueryObject> audit = context.Message[i]; // context.Message[0].Message
            }
        }
    }
}
