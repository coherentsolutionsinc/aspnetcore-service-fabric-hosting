using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools
{
    public interface IConfigurableObject<out TConfigurator>
    {
        void ConfigureObject(
            Action<TConfigurator> configAction);
    }
}