using System.Collections;
using System.ComponentModel;
using System.Fabric;
using System.Runtime.CompilerServices;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

using Microsoft.AspNetCore.Hosting;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostBuilderConfigurator
        : IConfigurableObjectDependenciesConfigurator
    {
        void UseServiceType(
            string serviceTypeName);
    }
}