using System;
using System.Fabric;
using System.Reflection;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Extensions;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest.Objects.Factories
{
    public class StatelessServiceAccessor : StatelessServiceAccessor<StatelessService>
    {
        public StatelessServiceAccessor(
            StatelessService instance)
            : base(instance)
        {
        }
    }

    public abstract class StatelessServiceAccessor<TService>
        where TService : Microsoft.ServiceFabric.Services.Runtime.StatelessService
    {
        private static readonly Lazy<PropertyInfo> partititon;

        static StatelessServiceAccessor()
        {
            partititon = typeof(Microsoft.ServiceFabric.Services.Runtime.StatelessService).QueryProperty("Partition", @public: false);
        }

        public TService Instance { get; }

        protected StatelessServiceAccessor(
            TService instance)
        {
            this.Instance = instance ?? throw new ArgumentNullException(nameof(instance));
        }

        public IStatelessServicePartition Partition
        {
            get => (IStatelessServicePartition)partititon.Value.GetValue(this.Instance);
            set => partititon.Value.SetValue(this.Instance, value);
        }
    }
}