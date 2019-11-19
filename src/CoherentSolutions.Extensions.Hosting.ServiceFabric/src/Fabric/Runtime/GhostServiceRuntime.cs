using System;
using System.Fabric;
using System.Threading;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime
{
    public class GhostServiceRuntime
    {
        private const string APPLICATION_NAME = "fabric:/ApplicationName";

        private const string APPLICATION_TYPE_NAME = "ApplicationTypeName";

        private const string CONTEXT = "366B8CCC-8CC3-4EAA-8B90-938000A5EF52";

        private const string SERVICE_NAME = "ServiceName";

        private readonly Lazy<NodeContext> nodeContext;

        private readonly Lazy<ICodePackageActivationContext> codePackageActivationContext;

        private long serviceIndex;

        private long instanceId;

        public static GhostServiceRuntime Default
        {
            get;
        }

        static GhostServiceRuntime()
        {
            Default = new GhostServiceRuntime();
        }

        private GhostServiceRuntime()
        {
            this.nodeContext = new Lazy<NodeContext>(CreateDefaultNodeContext, true);
            this.codePackageActivationContext = new Lazy<ICodePackageActivationContext>(CreateDefaultCodePackageActivationContext, true);

            this.serviceIndex = 0;
            this.instanceId = 0;
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

        private static NodeContext CreateDefaultNodeContext()
        {
            return new GhostNodeContext();
        }

        private static ICodePackageActivationContext CreateDefaultCodePackageActivationContext()
        {
            return new GhostCodePackageActivationContext(APPLICATION_NAME, APPLICATION_TYPE_NAME, CONTEXT);
        }
    }
}