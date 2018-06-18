using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class ServiceCommunicationListenerEventDecorator : ICommunicationListener
    {
        private readonly ICommunicationListener successor;

        private readonly ServiceEventSynchronization eventSynchronization;

        public ServiceCommunicationListenerEventDecorator(
            ServiceEventSynchronization eventSynchronization,
            ICommunicationListener successor)
        {
            this.eventSynchronization = eventSynchronization 
             ?? throw new ArgumentNullException(nameof(eventSynchronization));

            this.successor = successor 
             ?? throw new ArgumentNullException(nameof(successor));
        }

        public async Task<string> OpenAsync(
            CancellationToken cancellationToken)
        {
            try
            {
                return await this.successor.OpenAsync(cancellationToken);
            }
            finally
            {
                this.eventSynchronization.NotifyListenerOpened();
            }
        }

        public Task CloseAsync(
            CancellationToken cancellationToken)
        {
            return this.successor.CloseAsync(cancellationToken);
        }

        public void Abort()
        {
            this.successor.Abort();
        }
    }
}