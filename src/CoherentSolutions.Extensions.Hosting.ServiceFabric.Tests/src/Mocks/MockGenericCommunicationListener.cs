using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Mocks
{
    public class MockGenericCommunicationListener : ICommunicationListener
    {
        public void Abort()
        {
        }

        public Task<string> OpenAsync(
            CancellationToken cancellationToken)
        {
            return Task.FromResult(String.Empty);
        }

        public Task CloseAsync(
            CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}