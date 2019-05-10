using System;
using System.Fabric;
using System.Threading;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime
{
    public class ServiceRuntime
    {
        private const string APPLICATION_NAME = "fabric:/ApplicationName";
        private const string APPLICATION_TYPE_NAME = "ApplicationTypeName";
        private const string CONTEXT = "366B8CCC-8CC3-4EAA-8B90-938000A5EF52";

        private const string SERVICE_NAME = "ServiceName";

        private static readonly ServiceRuntime instance;

        private readonly Lazy<NodeContext> nodeContext;
        private readonly Lazy<ICodePackageActivationContext> codePackageActivationContext;

        private long serviceIndex;
        private long instanceId;

        public static ServiceRuntime Default
        {
            get
            {
                return instance;
            }
        }

        static ServiceRuntime()
        {
            instance = new ServiceRuntime();
        }

        private ServiceRuntime()
        {
            this.nodeContext = new Lazy<NodeContext>(CreateDefaultNodeContext, true);
            this.codePackageActivationContext = new Lazy<ICodePackageActivationContext>(CreateDefaultCodePackageActivationContext, true);

            this.serviceIndex = 0;
            this.instanceId = 0;
        }

        private static NodeContext CreateDefaultNodeContext()
        {
            return new RuntimeNodeContext();
        }

        private static ICodePackageActivationContext CreateDefaultCodePackageActivationContext()
        {
            return new RuntimeCodePackageActivationContext(APPLICATION_NAME, APPLICATION_TYPE_NAME, CONTEXT);
        }

        public NodeContext GetNodeContext()
        {
            return this.nodeContext.Value;
        }

        public ICodePackageActivationContext GetCodePackageActivationContext()
        {
            return this.codePackageActivationContext.Value;
        }

        public Uri CreateServiceName()
        {
            return new Uri($"{APPLICATION_NAME}/{SERVICE_NAME}{Interlocked.Increment(ref this.serviceIndex)}");
        }

        public long CreateInstanceId()
        {
            return Interlocked.Increment(ref this.instanceId);
        }
    }
}
