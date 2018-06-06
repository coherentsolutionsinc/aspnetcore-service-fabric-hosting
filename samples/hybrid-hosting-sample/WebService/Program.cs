using CoherentSolutions.AspNetCore.ServiceFabric.Hosting;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

using WebService.Controllers;

namespace WebService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new HybridHostBuilder()
                /*
                    This configuration would flow to both .ConfigureWebHost() method below and
                    to listenerBuilder.ConfigureWebHost() method inside .ConfigureStatefulServiceHost.

                    So we put shared configuration into .UseWebHostBuilder.
                */
                .UseWebHostBuilder(() => WebHost.CreateDefaultBuilder(args).UseStartup<Startup>())
                .ConfigureWebHost(
                    webHostBuilder =>
                    {
                        /*
                            Configuring services to use when hosting as self-hosted app.
                        */
                        webHostBuilder.ConfigureServices(
                            services =>
                            {
                                services.AddTransient<IInformationService, WebInformationService>();
                            });
                    })
                .ConfigureStatefulServiceHost(
                    serviceHostBuilder =>
                    {
                        serviceHostBuilder
                            .UseServiceName("WebServiceType")
                            .DefineAspNetCoreListener(
                                listenerBuilder =>
                                {
                                    listenerBuilder
                                        .UseKestrel()
                                        .UseEndpointName("WebServiceEndpoint")
                                        .UseUniqueServiceUrlIntegration()
                                        .ConfigureWebHost(
                                            webHostBuilder =>
                                            {
                                                /*
                                                    Configuring services to use when hosting as reliable service.
                                                */
                                                webHostBuilder.ConfigureServices(
                                                    services =>
                                                    {
                                                        services.AddTransient<IInformationService, FabricInformationService>();
                                                    });
                                            });
                                });
                    })
                .Build()
                .Run();
        }
    }
}