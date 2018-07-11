using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions.Support;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Items.Base
{
    public abstract class ServiceDelegateTheoryItem<T>
        : TheoryItem,
          IUseDelegateTheoryExtensionSupported,
          IUseDelegateInvokerTheoryExtensionSupported,
          IUseDependenciesTheoryExtensionSupported,
          IPickDependencyTheoryExtensionSupported
        where T : IServiceHostDelegateReplicaTemplateConfigurator
    {
        private class DelegateInvokerProxy : IServiceHostDelegateInvoker
        {
            private readonly IServiceHostDelegateInvoker target;

            private readonly IServiceProvider provider;

            private readonly IEnumerable<Action<IServiceProvider>> actions;

            public DelegateInvokerProxy(
                IServiceHostDelegateInvoker target,
                IServiceProvider provider,
                IEnumerable<Action<IServiceProvider>> actions)
            {
                this.target = target;
                this.provider = provider;
                this.actions = actions;
            }

            public Task InvokeAsync(
                CancellationToken cancellationToken)
            {
                foreach (var action in this.actions)
                {
                    action(this.provider);
                }

                return this.target.InvokeAsync(cancellationToken);
            }
        }

        protected ServiceDelegateTheoryItem(
            string name)
            : base(name)
        {
        }

        protected override void InitializeExtensions()
        {
            this.SetupExtension(new UseDelegateTheoryExtension());
            this.SetupExtension(new UseDelegateInvokerTheoryExtension());
            this.SetupExtension(new UseDependenciesTheoryExtension());
            this.SetupExtension(new PickDependencyTheoryExtension());
        }

        protected virtual void ConfigureExtensions(
            T configurator)
        {
            var useDelegateExtension = this.GetExtension<IUseDelegateTheoryExtension>();
            var useDelegateInvokerExtension = this.GetExtension<IUseDelegateInvokerTheoryExtension>();
            var useDependenciesExtension = this.GetExtension<IUseDependenciesTheoryExtension>();
            var pickDependenciesExtension = this.GetExtension<IPickDependencyTheoryExtension>();

            configurator.UseDependencies(useDependenciesExtension.Factory);
            configurator.UseDelegate(useDelegateExtension.Delegate);
            configurator.UseDelegateInvoker(
                (
                    @delegate,
                    provider) =>
                {
                    return new DelegateInvokerProxy(
                        useDelegateInvokerExtension.Factory(@delegate, provider),
                        provider,
                        pickDependenciesExtension.PickActions);
                });
        }
    }
}