using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tests.Stubs
{
    public class AspNetCoreCommunicationListenerStub : AspNetCoreCommunicationListener
    {
        private readonly Func<string, AspNetCoreCommunicationListener, IWebHost> build;

        public AspNetCoreCommunicationListenerStub(
            ServiceContext serviceContext,
            Func<string, AspNetCoreCommunicationListener, IWebHost> build)
            : base(serviceContext, build)
        {
            this.build = build;
        }

        public override Task<string> OpenAsync(
            CancellationToken cancellationToken)
        {
            this.build(this.GetListenerUrl(), this);

            return Task.FromResult(string.Empty);
        }

        public override Task CloseAsync(
            CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public override void Abort()
        {
        }

        protected override string GetListenerUrl()
        {
            return string.Empty;
        }

        public static AspNetCoreCommunicationListener Func(
            ServiceContext context,
            string name,
            Func<string, AspNetCoreCommunicationListener, IWebHost> build)
        {
            return new AspNetCoreCommunicationListenerStub(context, build);
        }
    }
}