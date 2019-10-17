using System;
using System.Collections.Generic;
using System.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceDelegateInvocationContextRegistrant : IServiceDelegateInvocationContextRegistrant
    {
        public IEnumerable<(Type t, object o)> GetInvocationContextRegistrations(
            IServiceDelegateInvocationContext invocationContext)
        {
            switch (invocationContext)
            {
                case IStatefulServiceDelegateInvocationContextOnPackageAdded<CodePackage> ctx:
                    {
                        yield return (typeof(IServiceEventPayloadOnPackageAdded<CodePackage>), ctx.Payload);
                    }
                    break;
                case IStatefulServiceDelegateInvocationContextOnPackageModified<CodePackage> ctx:
                    {
                        yield return (typeof(IServiceEventPayloadOnPackageModified<CodePackage>), ctx.Payload);
                    }
                    break;
                case IStatefulServiceDelegateInvocationContextOnPackageRemoved<CodePackage> ctx:
                    {
                        yield return (typeof(IServiceEventPayloadOnPackageRemoved<CodePackage>), ctx.Payload);
                    }
                    break;
                case IStatefulServiceDelegateInvocationContextOnPackageAdded<ConfigurationPackage> ctx:
                    {
                        yield return (typeof(IServiceEventPayloadOnPackageAdded<ConfigurationPackage>), ctx.Payload);
                    }
                    break;
                case IStatefulServiceDelegateInvocationContextOnPackageModified<ConfigurationPackage> ctx:
                    {
                        yield return (typeof(IServiceEventPayloadOnPackageModified<ConfigurationPackage>), ctx.Payload);
                    }
                    break;
                case IStatefulServiceDelegateInvocationContextOnPackageRemoved<ConfigurationPackage> ctx:
                    {
                        yield return (typeof(IServiceEventPayloadOnPackageRemoved<ConfigurationPackage>), ctx.Payload);
                    }
                    break;
                case IStatefulServiceDelegateInvocationContextOnPackageAdded<DataPackage> ctx:
                    {
                        yield return (typeof(IServiceEventPayloadOnPackageAdded<DataPackage>), ctx.Payload);
                    }
                    break;
                case IStatefulServiceDelegateInvocationContextOnPackageModified<DataPackage> ctx:
                    {
                        yield return (typeof(IServiceEventPayloadOnPackageModified<DataPackage>), ctx.Payload);
                    }
                    break;
                case IStatefulServiceDelegateInvocationContextOnPackageRemoved<DataPackage> ctx:
                    {
                        yield return (typeof(IServiceEventPayloadOnPackageRemoved<DataPackage>), ctx.Payload);
                    }
                    break;
                case IStatefulServiceDelegateInvocationContextOnChangeRole ctx:
                    {
                        yield return (typeof(IStatefulServiceEventPayloadOnChangeRole), ctx.Payload);
                    }
                    break;
                case IStatefulServiceDelegateInvocationContextOnShutdown ctx:
                    {
                        yield return (typeof(IStatefulServiceEventPayloadOnShutdown), ctx.Payload);
                    }
                    break;
                case IStatefulServiceDelegateInvocationContextOnDataLoss ctx:
                    {
                        yield return (typeof(IStatefulServiceEventPayloadOnDataLoss), ctx.Payload);
                    }
                    break;
            }
        }
    }
}