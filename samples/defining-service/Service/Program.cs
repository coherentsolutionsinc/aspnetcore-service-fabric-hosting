using CoherentSolutions.Extensions.Hosting.ServiceFabric;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new HostBuilder()
                .DefineStatefulService(
                    serviceBuilder =>
                    {
                        serviceBuilder
                            /*
                                Set name of service type defined in PackageRoot/ServiceManifest.xml
                            */
                            .UseServiceType("ServiceType")
                            .DefineAspNetCoreListener(
                                listenerBuilder =>
                                {
                                    listenerBuilder
                                        /*
                                            Set name of the endpoint defined in PackageRoot/ServiceManifest.xml
                                        */
                                        .UseEndpointName("ServiceEndpoint")
                                        .UseUniqueServiceUrlIntegration()
                                        .ConfigureWebHost(
                                            webHostBuilder => { webHostBuilder.UseStartup<Startup>(); });
                                });
                    })
                .Build()
                .Run();
        }
    }
}