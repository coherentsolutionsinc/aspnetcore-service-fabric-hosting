using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest.Objects.Factories
{
    public abstract class Accessor<TInstance>
        where TInstance : class
    {
        public TInstance Instance { get; }

        protected Accessor(
            TInstance instance)
        {
            this.Instance = instance ?? throw new ArgumentNullException(nameof(instance));
        }
    }
}