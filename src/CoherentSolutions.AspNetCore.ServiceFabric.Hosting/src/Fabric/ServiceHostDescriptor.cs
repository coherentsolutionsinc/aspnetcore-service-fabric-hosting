using System;
using System.ComponentModel;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    [Description("Service Fabric Reliable Service implementation.")]
    public class ServiceHostDescriptor : IHostDescriptor
    {
        public IHostKeywords Keywords { get; }

        public IHostRunner Runner { get; }

        public ServiceHostDescriptor(
            IServiceHostKeywords keywords,
            IServiceHostRunner runner)
        {
            this.Keywords = keywords
             ?? throw new ArgumentNullException(nameof(keywords));

            this.Runner = runner
             ?? throw new ArgumentNullException(nameof(runner));
        }
    }
}