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
                    Calling .UseWebHostBuilder() sets the factory function used to create 
                    instances of `IWebHostBuilder` during the build for both Web App and Service Fabric. 
                    
                    In general this allows to separate shared configuration by providing partially preconfigured
                    IWebHostBuilder from specific configuration done .ConfigureWebHost() methods. 
                */
                .UseWebHostBuilder(() => WebHost.CreateDefaultBuilder(args).UseStartup<Startup>())
                .ConfigureWebHost(
                    webHostBuilder => // Here we are receiving and instance of IWebHostBuilder from .UseWebHostBuilder()
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
                            /*
                                Set name of service type defined in PackageRoot/ServiceManifest.xml
                            */
                            .UseServiceName("WebServiceType")
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
                                            webHostBuilder => // Here we are receiving and instance of IWebHostBuilder from .UseWebHostBuilder()
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