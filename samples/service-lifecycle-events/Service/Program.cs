using System.Fabric;

using CoherentSolutions.Extensions.Hosting.ServiceFabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
                           .DefineDelegate(
                                delegateBuilder =>
                                {
                                    delegateBuilder
                                       .UseEvent(StatefulServiceLifecycleEvent.OnStartup)
                                       .UseLoggerOptions(() => new ServiceHostLoggerOptions())
                                       .UseDelegate(
                                            (StatefulServiceContext svcCtx, ILogger<object> logger) =>
                                            {
                                                logger.LogInformation(
                                                    new EventId(0, "Stateful.OnStartup"),
                                                    $"The replica {svcCtx.ReplicaId} is starting up.");
                                            });
                                })
                           .DefineDelegate(
                                delegateBuilder =>
                                {
                                    delegateBuilder
                                       .UseEvent(StatefulServiceLifecycleEvent.OnChangeRole)
                                       .UseLoggerOptions(() => new ServiceHostLoggerOptions())
                                       .UseDelegate(
                                            (StatefulServiceContext svcCtx, IStatefulServiceEventPayloadOnChangeRole payload, ILogger<object> logger) =>
                                            {
                                                logger.LogInformation(
                                                    new EventId(0, "Stateful.OnChangeRole"),
                                                    $"The replica {svcCtx.ReplicaId} is changing role: {payload.NewRole}.");
                                            });
                                })
                           .DefineDelegate(
                                delegateBuilder =>
                                {
                                    delegateBuilder
                                       .UseEvent(StatefulServiceLifecycleEvent.OnRun)
                                       .UseLoggerOptions(() => new ServiceHostLoggerOptions())
                                       .UseDelegate(
                                            (StatefulServiceContext svcCtx, ILogger<object> logger) =>
                                            {
                                                logger.LogInformation(
                                                    new EventId(0, "Stateful.OnRun"),
                                                    $"The replica {svcCtx.ReplicaId} is running (primary).");
                                            });
                                })
                           .DefineDelegate(
                                delegateBuilder =>
                                {
                                    delegateBuilder
                                       .UseEvent(StatefulServiceLifecycleEvent.OnShutdown)
                                       .UseLoggerOptions(() => new ServiceHostLoggerOptions())
                                       .UseDelegate(
                                            (StatefulServiceContext svcCtx, IStatefulServiceEventPayloadOnShutdown payload, ILogger<object> logger) =>
                                            {
                                                logger.LogInformation(
                                                    new EventId(0, "Stateful.OnShutdown"),
                                                    $"The replica {svcCtx.ReplicaId} is shutting down (aborting: {payload.IsAborting}).");
                                            });
                                })
                           .DefineDelegate(
                                delegateBuilder =>
                                {
                                    delegateBuilder
                                       .UseEvent(StatefulServiceLifecycleEvent.OnDataLoss)
                                       .UseLoggerOptions(() => new ServiceHostLoggerOptions())
                                       .UseDelegate(
                                            (StatefulServiceContext svcCtx, IStatefulServiceEventPayloadOnDataLoss payload, ILogger<object> logger) =>
                                            {
                                                logger.LogInformation(
                                                    new EventId(0, "Stateful.OnDataLoss"),
                                                    $"The replica {svcCtx.ReplicaId} data loss detected.");
                                            });
                                })
                           .DefineDelegate(
                                delegateBuilder =>
                                {
                                    delegateBuilder
                                       .UseEvent(StatefulServiceLifecycleEvent.OnRestoreCompleted)
                                       .UseLoggerOptions(() => new ServiceHostLoggerOptions())
                                       .UseDelegate(
                                            (StatefulServiceContext svcCtx, ILogger<object> logger) =>
                                            {
                                                logger.LogInformation(
                                                    new EventId(0, "Stateful.OnRestoreCompleted"),
                                                    $"The replica {svcCtx.ReplicaId} restore completed.");
                                            });
                                })
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
                           .DefineDelegate(
                                delegateBuilder =>
                                {
                                    delegateBuilder
                                       .UseEvent(StatelessServiceLifecycleEvent.OnStartup)
                                       .UseLoggerOptions(() => new ServiceHostLoggerOptions())
                                       .UseDelegate(
                                            (StatelessServiceContext svcCtx, ILogger<object> logger) =>
                                            {
                                                logger.LogInformation(
                                                    new EventId(0, "Stateful.OnStartup"),
                                                    $"The instance {svcCtx.InstanceId} is starting up.");
                                            });
                                })
                           .DefineDelegate(
                                delegateBuilder =>
                                {
                                    delegateBuilder
                                       .UseEvent(StatelessServiceLifecycleEvent.OnRun)
                                       .UseLoggerOptions(() => new ServiceHostLoggerOptions())
                                       .UseDelegate(
                                            (StatelessServiceContext svcCtx, ILogger<object> logger) =>
                                            {
                                                logger.LogInformation(
                                                    new EventId(0, "Stateful.OnRun"),
                                                    $"The instance {svcCtx.InstanceId} is running.");
                                            });
                                })
                           .DefineDelegate(
                                delegateBuilder =>
                                {
                                    delegateBuilder
                                       .UseEvent(StatelessServiceLifecycleEvent.OnShutdown)
                                       .UseLoggerOptions(() => new ServiceHostLoggerOptions())
                                       .UseDelegate(
                                            (StatelessServiceContext svcCtx, IStatelessServiceEventPayloadOnShutdown payload, ILogger<object> logger) =>
                                            {
                                                logger.LogInformation(
                                                    new EventId(0, "Stateful.OnShutdown"),
                                                    $"The instance {svcCtx.InstanceId} is shutting down (aborting: {payload.IsAborting}).");
                                            });
                                })
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