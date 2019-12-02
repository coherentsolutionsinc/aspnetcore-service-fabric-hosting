using System;
using System.Fabric;
using System.Fabric.Description;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.Services;

using Microsoft.Extensions.Logging;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime
{
    public class LocalRuntime : IServiceHostRuntime
    {
        private static readonly byte[] initializationData;

        private static int instanceOrReplicaId;

        private readonly NodeContext nodeContext;

        private readonly ICodePackageActivationContext codePackageActivationContext;

        private readonly ILoggerProvider loggerProvider;

        static LocalRuntime()
        {
            initializationData = Array.Empty<byte>();
            instanceOrReplicaId = 0;
        }

        public LocalRuntime(
            NodeContext nodeContext,
            ICodePackageActivationContext codePackageActivationContext,
            ILoggerProvider loggerProvider)
        {
            this.nodeContext = nodeContext ?? throw new ArgumentNullException(nameof(nodeContext));
            this.codePackageActivationContext = codePackageActivationContext ?? throw new ArgumentNullException(nameof(codePackageActivationContext));
            this.loggerProvider = loggerProvider ?? throw new ArgumentNullException(nameof(loggerProvider));
        }

        public Task RegisterServiceAsync(
            string serviceTypeName,
            Func<StatefulServiceContext, StatefulService> serviceFactory,
            TimeSpan timeout = default,
            CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException("Stateful Services aren't supported by local service runtime.");
        }

        public async Task RegisterServiceAsync(
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

            var found = this.codePackageActivationContext
               .GetServiceTypes()
               .Any(i => i.ServiceTypeKind == ServiceDescriptionKind.Stateless && i.ServiceTypeName == serviceTypeName);

            if (!found)
            {
                throw new InvalidOperationException($"Stateless service type: '{serviceTypeName}' isn't found");
            }

            var servicePartitionId = Guid.NewGuid();
            var serviceInstanceId = Interlocked.Increment(ref instanceOrReplicaId);
            var serviceName = $"{this.codePackageActivationContext.ApplicationName}/{serviceTypeName}-{serviceInstanceId}";
            var servicePartition = new LocalRuntimeStatelessServiceSingletonPartition(servicePartitionId);
            var serviceContext = new StatelessServiceContext(
                this.nodeContext,
                this.codePackageActivationContext,
                serviceTypeName,
                new Uri(serviceName),
                initializationData,
                servicePartitionId,
                serviceInstanceId);

            Environment.SetEnvironmentVariable("Fabric_ApplicationName", this.codePackageActivationContext.ApplicationName);
            Environment.SetEnvironmentVariable("Fabric_Folder_App_Log", this.codePackageActivationContext.LogDirectory);
            Environment.SetEnvironmentVariable("Fabric_Folder_App_Temp", this.codePackageActivationContext.TempDirectory);
            Environment.SetEnvironmentVariable("Fabric_Folder_App_Work", this.codePackageActivationContext.WorkDirectory);
            Environment.SetEnvironmentVariable("Fabric_ServicePackageActivationId", this.codePackageActivationContext.ContextId);
            Environment.SetEnvironmentVariable("Fabric_ServiceName", serviceName);
            Environment.SetEnvironmentVariable("Fabric_IsContainerHost", bool.FalseString);

            Environment.SetEnvironmentVariable("Fabric_CodePackageName", this.codePackageActivationContext.CodePackageName);

            foreach (var endpoint in this.codePackageActivationContext.GetEndpoints())
            {
                Environment.SetEnvironmentVariable($"Fabric_Endpoint_{endpoint.Name}", endpoint.Port.ToString());
                Environment.SetEnvironmentVariable($"Fabric_Endpoint_IPOrFQDN_{endpoint.Name}", endpoint.IpAddressOrFqdn);
            }

            Environment.SetEnvironmentVariable("Fabric_NodeId", this.nodeContext.NodeId.ToString());
            Environment.SetEnvironmentVariable("Fabric_NodeIPOrFQDN", this.nodeContext.IPAddressOrFQDN);
            Environment.SetEnvironmentVariable("Fabric_NodeName", this.nodeContext.NodeName);

            var service = serviceFactory(serviceContext);
            if (service is null)
            {
                throw new InvalidOperationException("No stateless service instance was created");
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