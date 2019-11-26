using System;
using System.Collections.Generic;
using System.Fabric;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Extensions;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using MicrosoftStatelessService = Microsoft.ServiceFabric.Services.Runtime.StatelessService;

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
        where TService : MicrosoftStatelessService
    {
        private static readonly Lazy<PropertyInfo> partititon;

        private static readonly Lazy<MethodInfo> createInstanceListeners;

        private static readonly Lazy<MethodInfo> runAsync;

        private static readonly Lazy<MethodInfo> onOpenAsync;

        static StatelessServiceAccessor()
        {
            partititon = typeof(MicrosoftStatelessService).QueryProperty("Partition", @public: false);
            createInstanceListeners = typeof(MicrosoftStatelessService).QueryMethod("CreateServiceInstanceListeners", @public: false);
            onOpenAsync = typeof(MicrosoftStatelessService).QueryMethod("OnOpenAsync", @public: false, @params: typeof(CancellationToken));
            runAsync = typeof(MicrosoftStatelessService).QueryMethod("RunAsync", @public: false, @params: typeof(CancellationToken));
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

        public IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return (IEnumerable<ServiceInstanceListener>)createInstanceListeners.Value.Invoke(this.Instance, null);
        }

        public Task OpenAsync(
            CancellationToken cancellationToken)
        {
            return (Task)onOpenAsync.Value.Invoke(this.Instance, new object[] { cancellationToken });
        }

        public Task RunAsync(
            CancellationToken cancellationToken)
        {
            return (Task)runAsync.Value.Invoke(this.Instance, new object[] { cancellationToken });
        }
    }
}