using System;
using System.Fabric;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client;

using Newtonsoft.Json.Linq;

using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace StatefulService
{
    public interface IManagementService
    {
        string GetImportantValue();
    }

    public interface IMessageProvider
    {
        string GetMessage();
    }

    public class MessageProvider : IMessageProvider
    {
        public string GetMessage()
        {
            return "Value";
        }
    }

    public class ManagementService : IManagementService
    {
        private readonly IMessageProvider provider;

        public ManagementService(
            IMessageProvider provider)
        {
            this.provider = provider;
        }

        public string GetImportantValue()
        {
            // Same instances should have the same hash
            return this.provider.GetMessage() + $"Hash: {this.GetHashCode()}";
        }
    }

    public interface IRemotingImplementation : IService
    {
        Task<string> RemotingGetImportantValue();
    }

    public class RemotingImplementation : IRemotingImplementation
    {
        private readonly IConfiguration configuration;

        private readonly Microsoft.Extensions.Hosting.IHostingEnvironment hostingEnvironment;

        private readonly ServiceContext context;

        private readonly IReliableStateManager manager;

        private readonly IManagementService managementService;

        // Dependency injection demonstration
        public RemotingImplementation(
            IConfiguration configuration,
            Microsoft.Extensions.Hosting.IHostingEnvironment hostingEnvironment,
            ServiceContext context,
            IReliableStateManager manager,
            IManagementService managementService)
        {
            this.configuration = configuration;
            this.hostingEnvironment = hostingEnvironment;
            this.context = context;
            this.manager = manager;
            this.managementService = managementService;
        }

        public Task<string> RemotingGetImportantValue()
        {
            return Task.FromResult(this.managementService.GetImportantValue());
        }
    }

    public class WebApiImplementationController : ControllerBase
    {
        private readonly IConfiguration configuration;

        private readonly IHostingEnvironment hostingEnvironment;

        private readonly ServiceContext context;

        private readonly IReliableStateManager manager;

        private readonly IManagementService managementService;

        // Dependency injection demonstration
        public WebApiImplementationController(
            IConfiguration configuration,
            IHostingEnvironment hostingEnvironment,
            ServiceContext context,
            IReliableStateManager manager,
            IManagementService managementService)
        {
            this.configuration = configuration;
            this.hostingEnvironment = hostingEnvironment;
            this.context = context;
            this.manager = manager;
            this.managementService = managementService;
        }

        [HttpGet]
        public Task<string> WebApiGetImportantValue()
        {
            return Task.FromResult(this.managementService.GetImportantValue());
        }
    }

    public class WebApiStartup
    {
        private readonly IConfiguration configuration;

        public WebApiStartup(
            IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(
            IServiceCollection services)
        {
            services.AddMvc();
        }

        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory)
        {
            app.UseMvcWithDefaultRoute();
        }
    }

    internal static class Program
    {
        /// <summary>
        ///     This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            var host = new HostBuilder()
               .ConfigureServices(
                    services =>
                    {
                        services.AddTransient<IMessageProvider, MessageProvider>();
                        services.AddSingleton<IManagementService, ManagementService>();
                    })
               .ConfigureStatefulService(
                    serviceHostBuilder =>
                    {
                        serviceHostBuilder
                           .UseServiceType("StatefulServiceType")
                           .DefineAspNetCoreListener(
                                listenerBuilder =>
                                {
                                    listenerBuilder
                                       .UseEndpointName("ServiceEndpoint")
                                       .UseKestrel()
                                       .UseUniqueServiceUrlIntegration()
                                       .ConfigureWebHost(
                                            webHostBuilder =>
                                            {
                                                webHostBuilder.UseStartup<WebApiStartup>();
                                            });
                                })
                           .DefineRemotingListener(
                                listenerBuilder =>
                                {
                                    listenerBuilder
                                       .UseEndpointName("ServiceEndpoint2")
                                       .UseImplementation<RemotingImplementation>();
                                });
                    })
               .Build();

            Task.Run(() => host.Run());

            var httpClient = new HttpClient();
            var proxyFactory = new ServiceProxyFactory(c => new FabricTransportServiceRemotingClientFactory());

            for (;;)
            {
                Thread.Sleep(10000);
                try
                {
                    var partitionAddress = new ServicePartitionResolver()
                       .ResolveAsync(new Uri("fabric:/App/StatefulService"), new ServicePartitionKey(0), CancellationToken.None)
                       .GetAwaiter()
                       .GetResult()
                       .GetEndpoint()
                       .Address;

                    var ip = JObject.Parse(partitionAddress)["Endpoints"]["ServiceEndpoint"].Value<string>();

                    var proxy = proxyFactory.CreateServiceProxy<IRemotingImplementation>(
                        new Uri("fabric:/App/StatefulService"),
                        new ServicePartitionKey(0),
                        TargetReplicaSelector.Default,
                        "ServiceEndpoint2");

                    // The 'hash' values should be the same because we are receiving response from the same instance.
                    var webApiValue = httpClient.GetStringAsync($"{ip}/WebApiImplementation/WebApiGetImportantValue").GetAwaiter().GetResult();
                    var remotingValue = proxy.RemotingGetImportantValue().GetAwaiter().GetResult();
                }
                catch
                {
                }
            }
        }
    }
}