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
               .DefineStatelessService(
                    serviceBuilder =>
                    {
                        serviceBuilder
                           .UseServiceType("ServiceType")
                           .DefineAspNetCoreListener(
                                listenerBuilder =>
                                {
                                    listenerBuilder
                                       .UseEndpoint("ServiceEndpoint")
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