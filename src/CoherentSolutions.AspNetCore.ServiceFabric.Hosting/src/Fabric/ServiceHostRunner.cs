using System;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public class ServiceHostRunner : IServiceHostRunner
    {
        private readonly IServiceHost host;

        public ServiceHostRunner(
            IServiceHost host)
        {
            this.host = host
             ?? throw new ArgumentNullException(nameof(host));
        }

        public void Run()
        {
            this.host.Run();
        }
    }
}