using CoherentSolutions.Extensions.Hosting.ServiceFabric;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace WebService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new HostBuilder()
                .ConfigureStatefulService(
                    serviceBuilder =>
                    {
                        serviceBuilder
                            /*
                                Set name of service type defined in PackageRoot/ServiceManifest.xml
                            */
                            .UseServiceType("WebServiceType")
                            .DefineAspNetCoreListener(
                                listenerBuilder =>
                                {
                                    listenerBuilder
                                        .UseKestrel()
                                        /*
                                            Set name of the endpoint defined in PackageRoot/ServiceManifest.xml
                                        */
                                        .UseEndpointName("WebServiceEndpoint")
                                        .UseUniqueServiceUrlIntegration()
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