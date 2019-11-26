using System;
using System.Fabric;
using System.Fabric.Description;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging.Console;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime
{
    public static class LocalRuntime
    {
        private readonly static byte[] initializationData;

        private static int instanceOrReplicaId;

        static LocalRuntime()
        {
            initializationData = Array.Empty<byte>();
            instanceOrReplicaId = 0;
        }

        public static async Task RegisterServiceAsync(
            string serviceTypeName,
            Func<StatelessServiceContext, StatelessService> serviceFactory,
            TimeSpan timeout = default,
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

            var nodeContext = new LocalRuntimeNodeContext();
            var activationContext = LocalRuntimeActivationContextProvider.GetActivationContext();

            var serviceDescriptions = activationContext.GetServiceTypes();
            if (!serviceDescriptions.Contains(serviceTypeName))
            {
                throw new InvalidOperationException();
            }
            var serviceDescription = serviceDescriptions[serviceTypeName];
            if (serviceDescription.ServiceTypeKind != ServiceDescriptionKind.Stateless)
            {
                throw new InvalidOperationException();
            }

            var servicePartition = new LocalRuntimeStatelessServiceSingletonPartition(Guid.NewGuid());
            var serviceContext = new StatelessServiceContext(
                nodeContext,
                activationContext,
                serviceTypeName,
                new Uri($"{activationContext.ApplicationName}/{Guid.NewGuid().ToString("N")}"),
                initializationData,
                servicePartition.PartitionInfo.Id,
                Interlocked.Increment(ref instanceOrReplicaId));

            var serviceAdapter = new LocalRuntimeStatelessServiceAdapter(
                serviceFactory(serviceContext), 
                servicePartition,
                new ConsoleLogger("asd", null, true));

            await serviceAdapter.OpenAsync();
        }
    }
}