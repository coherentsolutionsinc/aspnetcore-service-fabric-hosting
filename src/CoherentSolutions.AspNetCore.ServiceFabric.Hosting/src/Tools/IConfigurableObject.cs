using System;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tools
{
    public interface IConfigurableObject<out TConfigurator>
    {
        void ConfigureObject(
            Action<TConfigurator> configAction);
    }
}