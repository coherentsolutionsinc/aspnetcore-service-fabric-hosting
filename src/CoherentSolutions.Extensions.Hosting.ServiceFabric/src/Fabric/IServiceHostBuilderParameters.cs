using System.Collections.Generic;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostBuilderParameters
        : IConfigurableObjectDependenciesParameters
    {
        string ServiceTypeName { get; }

        IServiceHostEventSourceDescriptor EventSourceDescriptor { get; }

        List<IServiceHostListenerDescriptor> ListenerDescriptors { get; }

        List<IServiceHostDelegateDescriptor> DelegateDescriptors { get; }
    }
}