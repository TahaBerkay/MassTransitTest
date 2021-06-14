using MassTransit;
using System.Threading.Tasks;

namespace MassTransitTest
{
    public class ReceiveEndpointObserver : IReceiveEndpointObserver
    {

        public ReceiveEndpointObserver()
        {
        }

        public Task Ready(ReceiveEndpointReady ready)
        {
            return Task.CompletedTask;
        }

        public Task Stopping(ReceiveEndpointStopping stopping)
        {
            return Task.CompletedTask;
        }

        public Task Completed(ReceiveEndpointCompleted completed)
        {
            return Task.CompletedTask;
        }

        public Task Faulted(ReceiveEndpointFaulted faulted)
        {
            return Task.CompletedTask;
        }
    }
}