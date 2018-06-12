using System;

using Microsoft.AspNetCore.Hosting;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Common
{
    public class ExtensibleWebHostOnRunAction : IExtensibleWebHostOnRunAction
    {
        private readonly Action<IServiceProvider> onRunDelegate;

        public ExtensibleWebHostOnRunAction(
            Action<IServiceProvider> onRunDelegate)
        {
            this.onRunDelegate = onRunDelegate
             ?? throw new ArgumentNullException(nameof(onRunDelegate));
        }

        public void Execute(
            IWebHost webHost)
        {
            if (webHost == null)
            {
                throw new ArgumentNullException(nameof(webHost));
            }

            this.onRunDelegate(webHost.Services);
        }
    }
}