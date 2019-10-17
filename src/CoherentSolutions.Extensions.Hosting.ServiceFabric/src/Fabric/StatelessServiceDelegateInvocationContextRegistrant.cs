using System;
using System.Collections.Generic;
using System.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceDelegateInvocationContextRegistrant : IServiceDelegateInvocationContextRegistrant
    {
        public IEnumerable<(Type t, object o)> GetInvocationContextRegistrations(
            IServiceDelegateInvocationContext invocationContext)
        {
            switch (invocationContext)
            {
                case IStatelessServiceDelegateInvocationContextOnPackageAdded<CodePackage> ctx:
                    {
                        yield return (typeof(IServiceEventPayloadOnPackageAdded<CodePackage>), ctx.Payload);
                    }
                    break;
                case IStatelessServiceDelegateInvocationContextOnPackageModified<CodePackage> ctx:
                    {
                        yield return (typeof(IServiceEventPayloadOnPackageModified<CodePackage>), ctx.Payload);
                    }
                    break;
                case IStatelessServiceDelegateInvocationContextOnPackageRemoved<CodePackage> ctx:
                    {
                        yield return (typeof(IServiceEventPayloadOnPackageRemoved<CodePackage>), ctx.Payload);
                    }
                    break;
                case IStatelessServiceDelegateInvocationContextOnPackageAdded<ConfigurationPackage> ctx:
                    {
                        yield return (typeof(IServiceEventPayloadOnPackageAdded<ConfigurationPackage>), ctx.Payload);
                    }
                    break;
                case IStatelessServiceDelegateInvocationContextOnPackageModified<ConfigurationPackage> ctx:
                    {
                        yield return (typeof(IServiceEventPayloadOnPackageModified<ConfigurationPackage>), ctx.Payload);
                    }
                    break;
                case IStatelessServiceDelegateInvocationContextOnPackageRemoved<ConfigurationPackage> ctx:
                    {
                        yield return (typeof(IServiceEventPayloadOnPackageRemoved<ConfigurationPackage>), ctx.Payload);
                    }
                    break;
                case IStatelessServiceDelegateInvocationContextOnPackageAdded<DataPackage> ctx:
                    {
                        yield return (typeof(IServiceEventPayloadOnPackageAdded<DataPackage>), ctx.Payload);
                    }
                    break;
                case IStatelessServiceDelegateInvocationContextOnPackageModified<DataPackage> ctx:
                    {
                        yield return (typeof(IServiceEventPayloadOnPackageModified<DataPackage>), ctx.Payload);
                    }
                    break;
                case IStatelessServiceDelegateInvocationContextOnPackageRemoved<DataPackage> ctx:
                    {
                        yield return (typeof(IServiceEventPayloadOnPackageRemoved<DataPackage>), ctx.Payload);
                    }
                    break;
                case IStatelessServiceDelegateInvocationContextOnShutdown ctx:
                    {
                        yield return (typeof(IStatelessServiceEventPayloadOnShutdown), ctx.Payload);
                    }
                    break;
            }
        }
    }
}