using CoherentSolutions.Extensions.Hosting.ServiceFabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

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
                           .DefineDelegate(
                                b =>
                                {
                                    b.UseEvent(
                                            StatefulServiceLifecycleEvent.OnRun)
                                       .UseDelegate(
                                            () =>
                                            {
                                                StatefulServiceEventSource.Current.ServiceReplicaStartupEvent(0);
                                            });
                                })
                           .DefineDelegate(
                                b =>
                                {
                                    b.UseEvent(
                                            StatefulServiceLifecycleEvent.OnCodePackageAdded
                                          | StatefulServiceLifecycleEvent.OnCodePackageModified
                                          | StatefulServiceLifecycleEvent.OnCodePackageRemoved)
                                       .UseDelegate(
                                            (
                                                IStatefulServiceDelegateInvocationContext ctx) =>
                                            {
                                                StatefulServiceEventSource.Current.ServiceReplicaCodePackageEvent(ctx.Event.ToString());
                                            });
                                })
                           .DefineDelegate(
                                b =>
                                {
                                    b.UseEvent(
                                            StatefulServiceLifecycleEvent.OnConfigPackageAdded
                                          | StatefulServiceLifecycleEvent.OnConfigPackageModified
                                          | StatefulServiceLifecycleEvent.OnConfigPackageRemoved)
                                       .UseDelegate(
                                            (
                                                IStatefulServiceDelegateInvocationContext ctx) =>
                                            {
                                                StatefulServiceEventSource.Current.ServiceReplicaConfigPackageEvent(ctx.Event.ToString());
                                            });
                                })
                           .DefineDelegate(
                                b =>
                                {
                                    b.UseEvent(
                                            StatefulServiceLifecycleEvent.OnDataPackageAdded
                                          | StatefulServiceLifecycleEvent.OnDataPackageModified
                                          | StatefulServiceLifecycleEvent.OnDataPackageRemoved)
                                       .UseDelegate(
                                            (
                                                IStatefulServiceDelegateInvocationContext ctx) =>
                                            {
                                                StatefulServiceEventSource.Current.ServiceReplicaDataPackageEvent(ctx.Event.ToString());
                                            });
                                });
                    })
               .DefineStatelessService(
                    serviceBuilder =>
                    {
                        serviceBuilder
                           .UseServiceType("StatelessServiceType")
                           .DefineDelegate(
                                b =>
                                {
                                    b.UseEvent(
                                            StatelessServiceLifecycleEvent.OnRun)
                                       .UseDelegate(
                                            () =>
                                            {
                                                StatelessServiceEventSource.Current.ServiceInstanceRunEvent(0);
                                            });
                                })
                           .DefineDelegate(
                                b =>
                                {
                                    b.UseEvent(
                                            StatelessServiceLifecycleEvent.OnCodePackageAdded
                                          | StatelessServiceLifecycleEvent.OnCodePackageModified
                                          | StatelessServiceLifecycleEvent.OnCodePackageRemoved)
                                       .UseDelegate(
                                            (
                                                IStatelessServiceDelegateInvocationContext ctx) =>
                                            {
                                                StatelessServiceEventSource.Current.ServiceReplicaCodePackageEvent(ctx.Event.ToString());
                                            });
                                })
                           .DefineDelegate(
                                b =>
                                {
                                    b.UseEvent(
                                            StatelessServiceLifecycleEvent.OnConfigPackageAdded
                                          | StatelessServiceLifecycleEvent.OnConfigPackageModified
                                          | StatelessServiceLifecycleEvent.OnConfigPackageRemoved)
                                       .UseDelegate(
                                            (
                                                IStatelessServiceDelegateInvocationContext ctx) =>
                                            {
                                                StatelessServiceEventSource.Current.ServiceReplicaConfigPackageEvent(ctx.Event.ToString());
                                            });
                                })
                           .DefineDelegate(
                                b =>
                                {
                                    b.UseEvent(
                                            StatelessServiceLifecycleEvent.OnDataPackageAdded
                                          | StatelessServiceLifecycleEvent.OnDataPackageModified
                                          | StatelessServiceLifecycleEvent.OnDataPackageRemoved)
                                       .UseDelegate(
                                            (
                                                IStatelessServiceDelegateInvocationContext ctx) =>
                                            {
                                                StatelessServiceEventSource.Current.ServiceReplicaDataPackageEvent(ctx.Event.ToString());
                                            });
                                });
                    })
               .Build()
               .Run();
        }
    }
}