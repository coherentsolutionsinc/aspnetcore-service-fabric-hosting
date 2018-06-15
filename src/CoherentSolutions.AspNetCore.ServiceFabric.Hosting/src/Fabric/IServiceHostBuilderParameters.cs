using System.Collections.Generic;

using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tools;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IServiceHostBuilderParameters
        : IConfigurableObjectDependenciesParameters
    {
        string ServiceTypeName { get; }

        List<IServiceHostListenerDescriptor> ListenerDescriptors { get; }
    }
}