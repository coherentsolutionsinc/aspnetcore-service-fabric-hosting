using CoherentSolutions.Extensions.Hosting.ServiceFabric;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Service.Common;
using Service.Web;

namespace Service
{
    internal static class Program
    {
        private static void Main()
        {
            new HostBuilder()
                .ConfigureServices(
                    services =>
                    {
                        // Singleton
                        services.AddSingleton<ISharedService, SharedService>();

                        // Transient
                        services.AddTransient<IPersonalService, PersonalService>();
                    })
                .DefineStatefulService(
                    serviceHostBuilder =>
                    {
                        serviceHostBuilder
                            .UseServiceType("ServiceType")
                            .DefineAspNetCoreListener(
                                listenerBuilder =>
                                {
                                    listenerBuilder
                                        .UseEndpointName("FirstEndpoint")
                                        .UseUniqueServiceUrlIntegration()
                                        .ConfigureWebHost(webHostBuilder => { webHostBuilder.UseStartup<Startup>(); });
                                })
                            .DefineAspNetCoreListener(
                                listenerBuilder =>
                                {
                                    listenerBuilder
                                        .UseEndpointName("SecondEndpoint")
                                        .UseUniqueServiceUrlIntegration()
                                        .ConfigureWebHost(webHostBuilder => { webHostBuilder.UseStartup<Startup>(); });
                                });
                    })
                .Build()
                .Run();
        }
    }
}