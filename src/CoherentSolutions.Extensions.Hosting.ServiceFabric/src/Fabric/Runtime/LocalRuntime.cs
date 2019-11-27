using System;
using System.Fabric;
using System.Fabric.Description;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime
{
    public static class LocalRuntime
    {
        private readonly static byte[] initializationData;

        private static int instanceOrReplicaId;
        private static Guid partitionId;

        static LocalRuntime()
        {
            initializationData = Array.Empty<byte>();
            instanceOrReplicaId = 0;
            partitionId = new Guid(0x30fb9439, 0x4624, 0x492d, new byte[] { 0xa9, 0x8, 0x9b, 0x16, 0x9f, 0x93, 0xef, 0x28 });
        }

        public static async Task RegisterServiceAsync(
            string serviceTypeName,
            Func<StatelessServiceContext, StatelessService> serviceFactory,
            ILogger logger,
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

            var servicePartition = new LocalRuntimeStatelessServiceSingletonPartition(partitionId);
            var serviceContext = new StatelessServiceContext(
                nodeContext,
                activationContext,
                serviceTypeName,
                new Uri($"{activationContext.ApplicationName}/{Guid.NewGuid().ToString("N")}"),
                initializationData,
                partitionId,
                Interlocked.Increment(ref instanceOrReplicaId));

            var serviceAdapter = new LocalRuntimeStatelessServiceAdapter(
                serviceFactory(serviceContext), 
                servicePartition,
                logger);

            await serviceAdapter.OpenAsync();
        }
    }
}