using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Health;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime;
using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceSingletonPartition : IStatelessServicePartition
    {
        public ServicePartitionInformation PartitionInfo
        {
            get
            {
                return new SingletonPartitionInformation();
            }
        }

        public void ReportFault(FaultType faultType)
        {
        }

        public void ReportInstanceHealth(HealthInformation healthInfo)
        {
        }

        public void ReportInstanceHealth(HealthInformation healthInfo, HealthReportSendOptions sendOptions)
        {
        }

        public void ReportLoad(IEnumerable<LoadMetric> metrics)
        {
        }

        public void ReportMoveCost(MoveCost moveCost)
        {
        }

        public void ReportPartitionHealth(HealthInformation healthInfo)
        {
        }

        public void ReportPartitionHealth(HealthInformation healthInfo, HealthReportSendOptions sendOptions)
        {
        }
    }
    public class StatelessServiceInstance
    {
        private static readonly Lazy<Action<StatelessService, IStatelessServicePartition>> setPartition;
        private static readonly Lazy<Func<StatelessService, IEnumerable<ServiceInstanceListener>>> createInstanceListeners;

        private readonly StatelessService service;
        private readonly IStatelessServicePartition partition;

        static StatelessServiceInstance()
        {
            setPartition = new Lazy<Action<StatelessService, IStatelessServicePartition>>(() =>
            {
                const string NAME = "Partition";

                var p = typeof(StatelessService).GetProperty(NAME, BindingFlags.NonPublic | BindingFlags.Instance);
                if (p is null)
                {
                    throw new MissingMemberException(nameof(StatelessService), NAME);
                }

                if (p.SetMethod is null)
                {
                    p = p.DeclaringType.GetProperty(NAME, BindingFlags.NonPublic | BindingFlags.Instance);
                }

                return (service, partition) =>
                {
                    p.SetValue(service, partition);
                };
            }, true);
            createInstanceListeners = new Lazy<Func<StatelessService, IEnumerable<ServiceInstanceListener>>>(() =>
            {
                const string NAME = "CreateServiceInstanceListeners";

                var m = typeof(StatelessService).GetMethod(NAME, BindingFlags.NonPublic | BindingFlags.Instance);
                if (m is null)
                {
                    throw new MissingMethodException(nameof(StatelessService), NAME);
                }

                return service =>
                {
                    return (IEnumerable<ServiceInstanceListener>)m.Invoke(service, null);
                };
            }, true);
        }

        public StatelessServiceInstance(
            StatelessService service,
            IStatelessServicePartition partition)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
            this.partition = partition ?? throw new ArgumentNullException(nameof(partition));

            setPartition.Value(this.service, this.partition);
        }

        public async Task StartupAsync()
        {
            var instanceListeners = createInstanceListeners.Value(this.service);
            foreach (var instanceListener in instanceListeners)
            {
                var communicationListener = instanceListener.CreateCommunicationListener(this.service.Context);

                await communicationListener.OpenAsync(default);
            }
        }
    }

    public class StatelessServiceHostedRuntimeRegistrant : IStatelessServiceRuntimeRegistrant
    {
        public async Task RegisterAsync(
            string serviceTypeName,
            Func<StatelessServiceContext, StatelessService> serviceFactory,
            CancellationToken cancellationToken)
        {
            var runtime = ServiceRuntime.Default;
            var context = new StatelessServiceContext(
                runtime.GetNodeContext(),
                runtime.GetCodePackageActivationContext(),
                serviceTypeName,
                runtime.CreateServiceName(),
                null,
                Guid.NewGuid(),
                runtime.CreateInstanceId());

            var service = serviceFactory(context);
            var partition = new StatelessServiceSingletonPartition();

            var instance = new StatelessServiceInstance(service, partition);

            await instance.StartupAsync();
        }

        public Task UnregisterAsync(
            string serviceTypeName,
            CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
