using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Mocks
{
    public class MockAspNetCoreCommunicationListener : AspNetCoreCommunicationListener
    {
        private readonly Func<string, AspNetCoreCommunicationListener, IWebHost> build;

        public MockAspNetCoreCommunicationListener(
            ServiceContext serviceContext,
            Func<string, AspNetCoreCommunicationListener, IWebHost> build)
            : base(serviceContext, build)
        {
            this.build = build;
        }

        public override void Abort()
        {
        }

        public override Task CloseAsync(
            CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public override Task<string> OpenAsync(
            CancellationToken cancellationToken)
        {
            this.build(string.Empty, this);

            return Task.FromResult(String.Empty);
        }

        protected override string GetListenerUrl()
        {
            return String.Empty;
        }
    }
}