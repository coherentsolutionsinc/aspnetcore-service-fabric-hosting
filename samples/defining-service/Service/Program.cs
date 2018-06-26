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
                /*
                    Define stateful service
                */
                .DefineStatefulService(
                    serviceBuilder =>
                    {
                        serviceBuilder
                            /*
                                Set name of service type defined in PackageRoot/ServiceManifest.xml
                            */
                            .UseServiceType("ServiceType")
                            /*
                                Define aspnetcore listener
                            */
                            .DefineAspNetCoreListener(
                                listenerBuilder =>
                                {
                                    listenerBuilder
                                        /*
                                            Set name of the endpoint defined in PackageRoot/ServiceManifest.xml
                                        */
                                        .UseEndpointName("ServiceEndpoint")
                                        /*
                                            Use unique service URL integration
                                        */
                                        .UseUniqueServiceUrlIntegration()
                                        .ConfigureWebHost(webHostBuilder => { webHostBuilder.UseStartup<Startup>(); });
                                });
                    })
                .Build()
                .Run();
        }
    }
}