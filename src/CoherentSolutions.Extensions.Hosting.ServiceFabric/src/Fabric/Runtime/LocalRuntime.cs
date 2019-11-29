using System;
using System.Fabric;
using System.Fabric.Description;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ActivationContexts;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.NodeContexts;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.Services;

using Microsoft.Extensions.Logging;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime
{
    public class LocalRuntime : ILocalRuntime
    {
        private readonly INodeContextProvider nodeContextProvider;

        private readonly ICodePackageActivationContextProvider activationContextProvider;

        private readonly ILoggerProvider loggerProvider;

        private static readonly byte[] initializationData;

        private static int instanceOrReplicaId;

        static LocalRuntime()
        {
            initializationData = Array.Empty<byte>();
            instanceOrReplicaId = 0;
        }

        public LocalRuntime(
            INodeContextProvider nodeContextProvider,
            ICodePackageActivationContextProvider activationContextProvider,
            ILoggerProvider loggerProvider)
        {
            this.nodeContextProvider = nodeContextProvider ?? throw new ArgumentNullException(nameof(nodeContextProvider));
            this.activationContextProvider = activationContextProvider ?? throw new ArgumentNullException(nameof(activationContextProvider));
            this.loggerProvider = loggerProvider ?? throw new ArgumentNullException(nameof(loggerProvider));
        }

        public async Task RegisterServiceAsync(
            string serviceTypeName,
            Func<StatelessServiceContext, StatelessService> serviceFactory,
            CancellationToken cancellationToken = default)
        {
            if (serviceTypeName is null)
            {
                throw new ArgumentNullException(nameof(serviceTypeName));
            }

            if (serviceFactory is null)
            {
                throw new ArgumentNullException(nameof(serviceFactory));
            }

            var nodeContext = this.nodeContextProvider.GetNodeContext();
            var activationContext = this.activationContextProvider.GetActivationContext();

            var found = activationContext
               .GetServiceTypes()
               .Any(i => i.ServiceTypeKind == ServiceDescriptionKind.Stateless && i.ServiceTypeName == serviceTypeName);

            if (!found)
            {
                throw new InvalidOperationException($"Stateless service type: '{serviceTypeName}' isn't found");
            }

            var servicePartitionId = Guid.NewGuid();
            var serviceInstanceId = Interlocked.Increment(ref instanceOrReplicaId);
            var serviceName = $"{serviceTypeName}-{serviceInstanceId}";
            var servicePartition = new LocalRuntimeStatelessServiceSingletonPartition(servicePartitionId);
            var serviceContext = new StatelessServiceContext(
                nodeContext,
                activationContext,
                serviceTypeName,
                new Uri($"{activationContext.ApplicationName}/{serviceName}"),
                initializationData,
                servicePartitionId,
                serviceInstanceId);

            var service = serviceFactory(serviceContext);
            if (service is null)
            {
                throw new InvalidOperationException($"No stateless service instance was created");
            }
            var serviceAdapter = new LocalRuntimeStatelessServiceAdapter(
                new StatelessServiceAccessor<StatelessService>(service)
                {
                    Partition = servicePartition
                },
                this.loggerProvider.CreateLogger(serviceName));

            await serviceAdapter.OpenAsync();
        }
    }
}