using CoherentSolutions.Extensions.Hosting.ServiceFabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Service
{
    public class Program
    {
        public static void Main(
            string[] args)
        {
            new HostBuilder()
               .DefineStatefulService(
                    serviceBuilder =>
                    {
                        serviceBuilder
                           .UseServiceType("ServiceType")
                           .SetupEventSource(
                                eventSourceBuilder =>
                                {
                                    eventSourceBuilder
                                       .UseImplementation(() => ServiceEventSource.Current);
                                })
                           .DefineAspNetCoreListener(
                                listenerBuilder =>
                                {
                                    listenerBuilder
                                       .UseEndpoint("ServiceEndpoint")
                                       .UseUniqueServiceUrlIntegration()
                                       .UseLoggerOptions(() => new ServiceHostLoggerOptions())
                                       .ConfigureWebHost(
                                            webHostBuilder =>
                                            {
                                                webHostBuilder.UseStartup<Startup>();
                                            });
                                });
                    })
               .Build()
               .Run();
        }
    }
}