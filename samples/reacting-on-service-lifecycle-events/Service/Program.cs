using System.Fabric;

using CoherentSolutions.Extensions.Hosting.ServiceFabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Service.src;

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
                                       .UseDelegate(
                                            (
                                                StatefulServiceContext svcCtx) =>
                                            {
                                                StatefulServiceEventSource.Current.ServiceReplicaStartupEvent(svcCtx.ReplicaId);
                                            });
                                })
                           .DefineDelegate(
                                delegateBuilder =>
                                {
                                    delegateBuilder
                                       .UseEvent(StatefulServiceLifecycleEvent.OnChangeRole)
                                       .UseDelegate(
                                            (
                                                StatefulServiceContext svcCtx,
                                                IStatefulServiceEventPayloadOnChangeRole payload) =>
                                            {
                                                StatefulServiceEventSource.Current.ServiceReplicaChangeRoleEvent(svcCtx.ReplicaId, payload.NewRole.ToString());
                                            });
                                })
                           .DefineDelegate(
                                delegateBuilder =>
                                {
                                    delegateBuilder
                                       .UseEvent(StatefulServiceLifecycleEvent.OnRun)
                                       .UseDelegate(
                                            (
                                                StatefulServiceContext svcCtx) =>
                                            {
                                                StatefulServiceEventSource.Current.ServiceReplicaRunEvent(svcCtx.ReplicaId);
                                            });
                                })
                           .DefineDelegate(
                                delegateBuilder =>
                                {
                                    delegateBuilder
                                       .UseEvent(StatefulServiceLifecycleEvent.OnShutdown)
                                       .UseDelegate(
                                            (
                                                StatefulServiceContext svcCtx,
                                                IStatefulServiceEventPayloadOnShutdown payload) =>
                                            {
                                                StatefulServiceEventSource.Current.ServiceReplicaShutdownEvent(svcCtx.ReplicaId, payload.IsAborting);
                                            });
                                })
                           .DefineDelegate(
                                delegateBuilder =>
                                {
                                    delegateBuilder
                                       .UseEvent(StatefulServiceLifecycleEvent.OnDataLoss)
                                       .UseDelegate(
                                            (
                                                StatefulServiceContext svcCtx,
                                                IStatefulServiceEventPayloadOnDataLoss payload) =>
                                            {
                                                StatefulServiceEventSource.Current.ServiceReplicaDataLossEvent(svcCtx.ReplicaId);
                                            });
                                })
                           .DefineDelegate(
                                delegateBuilder =>
                                {
                                    delegateBuilder
                                       .UseEvent(StatefulServiceLifecycleEvent.OnRestoreCompleted)
                                       .UseDelegate(
                                            (
                                                StatefulServiceContext svcCtx,
                                                ILogger<object> logger) =>
                                            {
                                                StatefulServiceEventSource.Current.ServiceReplicaRestoreCompletedEvent(svcCtx.ReplicaId);
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
                                       .UseDelegate(
                                            (
                                                StatelessServiceContext svcCtx) =>
                                            {
                                                StatelessServiceEventSource.Current.ServiceInstanceStartupEvent(svcCtx.InstanceId);
                                            });
                                })
                           .DefineDelegate(
                                delegateBuilder =>
                                {
                                    delegateBuilder
                                       .UseEvent(StatelessServiceLifecycleEvent.OnRun)
                                       .UseDelegate(
                                            (
                                                StatelessServiceContext svcCtx) =>
                                            {
                                                StatelessServiceEventSource.Current.ServiceInstanceRunEvent(svcCtx.InstanceId);
                                            });
                                })
                           .DefineDelegate(
                                delegateBuilder =>
                                {
                                    delegateBuilder
                                       .UseEvent(StatelessServiceLifecycleEvent.OnShutdown)
                                       .UseDelegate(
                                            (
                                                StatelessServiceContext svcCtx,
                                                IStatelessServiceEventPayloadOnShutdown payload) =>
                                            {
                                                StatelessServiceEventSource.Current.ServiceInstanceShutdownEvent(svcCtx.InstanceId, payload.IsAborting);
                                            });
                                });
                    })
               .Build()
               .Run();
        }
    }
}