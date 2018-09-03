using CoherentSolutions.Extensions.Hosting.ServiceFabric;

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
                           .UseServiceType("StatefulServiceType")
                           .DefineAspNetCoreListener(
                                listenerBuilder =>
                                {
                                    listenerBuilder
                                       .UseEndpoint("StatefulServiceEndpoint")
                                       .UseUniqueServiceUrlIntegration()
                                       .ConfigureWebHost(
                                            webHostBuilder =>
                                            {
                                                webHostBuilder.UseStartup<Startup>();
                                            });
                                });
                    })
               .DefineStatelessService(
                    serviceBuilder =>
                    {
                        serviceBuilder
                           .UseServiceType("StatelessServiceType")
                           .DefineAspNetCoreListener(
                                listenerBuilder =>
                                {
                                    listenerBuilder
                                       .UseEndpoint("StatelessServiceEndpoint")
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