using System;
using System.Collections.Generic;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Common;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Common.Exceptions;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tools;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting
{
    public class HostBuilder
        : ConfigurableObject<IHostBuilderConfigurator>,
          IHostBuilder
    {
        private enum ServiceHostKind
        {
            None,

            Stateful,

            Stateless
        }

        private class Parameters
            : IHostBuilderParameters,
              IHostBuilderConfigurator
        {
            public ServiceHostKind ServiceHostKind { get; private set; }

            public Func<IWebHostBuilderExtensionsImpl> WebHostBuilderExtensionsImplFunc { get; private set; }

            public Func<IWebHostExtensionsImpl> WebHostExtensionsImplFunc { get; private set; }

            public Func<IWebHostBuilder> WebHostBuilderFunc { get; private set; }

            public Func<IServiceHostBuilder<IServiceHost, IServiceHostBuilderConfigurator>> ServiceHostBuilderFunc { get; private set; }

            public Func<IHostSelector> HostSelectorFunc { get; private set; }

            public Func<IHostRunner, IHost> HostFunc { get; private set; }

            public Action<IWebHostBuilder> WebHostConfigAction { get; private set; }

            public Action<IServiceHostBuilder<IServiceHost, IServiceHostBuilderConfigurator>> ServiceHostConfigAction { get; private set; }

            public Parameters()
            {
                this.ServiceHostKind = ServiceHostKind.None;

                this.WebHostBuilderExtensionsImplFunc = DefaultWebHostBuilderExtensionsImplFunc;
                this.WebHostExtensionsImplFunc = DefaultWebHostExtensionsImplFunc;
                this.WebHostBuilderFunc = DefaultWebHostBuilderFuncImpl;
                this.ServiceHostBuilderFunc = () =>
                {
                    switch (this.ServiceHostKind)
                    {
                        case ServiceHostKind.Stateful:
                            return DefaultStatefulServiceHostBuilderFuncImpl();
                        case ServiceHostKind.Stateless:
                            return DefaultStatelessServiceHostBuilderFuncImpl();
                        default:
                            return null;
                    }
                };
                this.HostSelectorFunc = DefaultHostSelectorFuncImpl;
                this.HostFunc = DefaultHostFuncImpl;
                this.WebHostConfigAction = null;
                this.ServiceHostConfigAction = null;
            }

            public void UseWebHostBuilderExtensionsImpl(
                Func<IWebHostBuilderExtensionsImpl> factoryFunc)
            {
                this.WebHostBuilderExtensionsImplFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }

            public void UseWebHostExtensionsImpl(
                Func<IWebHostExtensionsImpl> factoryFunc)
            {
                this.WebHostExtensionsImplFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }

            public void UseWebHostBuilder(
                Func<IWebHostBuilder> factoryFunc)
            {
                this.WebHostBuilderFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }

            public void UseStatefulServiceHostBuilder(
                Func<IStatefulServiceHostBuilder> factoryFunc)
            {
                if (this.ServiceHostKind == ServiceHostKind.Stateless)
                {
                    throw new InvalidOperationException("The host is already being configured as stateless service");
                }

                this.ServiceHostKind = ServiceHostKind.Stateful;

                this.ServiceHostBuilderFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }

            public void UseStatelessServiceHostBuilder(
                Func<IStatelessServiceHostBuilder> factoryFunc)
            {
                if (this.ServiceHostKind == ServiceHostKind.Stateful)
                {
                    throw new InvalidOperationException("The host is already being configured as stateful service");
                }

                this.ServiceHostKind = ServiceHostKind.Stateless;

                this.ServiceHostBuilderFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }

            public void UseHostSelector(
                Func<IHostSelector> factoryFunc)
            {
                this.HostSelectorFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }

            public void UseHost(
                Func<IHostRunner, IHost> factoryFunc)
            {
                this.HostFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }

            public void ConfigureWebHost(
                Action<IWebHostBuilder> configAction)
            {
                if (configAction == null)
                {
                    throw new ArgumentNullException(nameof(configAction));
                }

                this.WebHostConfigAction = this.WebHostConfigAction.Chain(configAction);
            }

            public void ConfigureStatefulServiceHost(
                Action<IStatefulServiceHostBuilder> configAction)
            {
                if (configAction == null)
                {
                    throw new ArgumentNullException(nameof(configAction));
                }

                if (this.ServiceHostKind == ServiceHostKind.Stateless)
                {
                    throw new InvalidOperationException("The host is already being configured as stateless service");
                }

                this.ServiceHostKind = ServiceHostKind.Stateful;

                this.ServiceHostConfigAction = this.ServiceHostConfigAction.Chain(c => configAction((IStatefulServiceHostBuilder) c));
            }

            public void ConfigureStatelessServiceHost(
                Action<IStatelessServiceHostBuilder> configAction)
            {
                if (configAction == null)
                {
                    throw new ArgumentNullException(nameof(configAction));
                }

                if (this.ServiceHostKind == ServiceHostKind.Stateful)
                {
                    throw new InvalidOperationException("The host is already being configured as stateful service");
                }

                this.ServiceHostKind = ServiceHostKind.Stateless;

                this.ServiceHostConfigAction = this.ServiceHostConfigAction.Chain(c => configAction((IStatelessServiceHostBuilder) c));
            }

            private static IWebHostBuilderExtensionsImpl DefaultWebHostBuilderExtensionsImplFunc()
            {
                return new WebHostBuilderExtensionsImpl();
            }

            private static IWebHostExtensionsImpl DefaultWebHostExtensionsImplFunc()
            {
                return new WebHostExtensionsImpl();
            }

            private static IWebHostBuilder DefaultWebHostBuilderFuncImpl()
            {
                return new WebHostBuilder();
            }

            private static IStatefulServiceHostBuilder DefaultStatefulServiceHostBuilderFuncImpl()
            {
                return new StatefulServiceHostBuilder();
            }

            private static IStatelessServiceHostBuilder DefaultStatelessServiceHostBuilderFuncImpl()
            {
                return new StatelessServiceHostBuilder();
            }

            private static IHostSelector DefaultHostSelectorFuncImpl()
            {
                return new HostSelector();
            }

            private static IHost DefaultHostFuncImpl(
                IHostRunner runner)
            {
                return new Host(runner);
            }
        }

        public IHost Build()
        {
            var hostDescriptors = new List<IHostDescriptor>();
            var hostKeywordsProviders = new List<IHostKeywordsProvider>
                { new HostKeywordsProvider(new ConfigurationBuilder().AddEnvironmentVariables().Build()) };

            var parameters = new Parameters();

            this.UpstreamConfiguration(parameters);

            if (parameters.WebHostConfigAction != null)
            {
                var builder = parameters.WebHostBuilderFunc();
                if (builder == null)
                {
                    throw new FactoryProducesNullInstanceException<IWebHostBuilder>();
                }

                var extensionsImpl = parameters.WebHostExtensionsImplFunc();
                if (extensionsImpl == null)
                {
                    throw new FactoryProducesNullInstanceException<IWebHostExtensionsImpl>();
                }

                builder = new ExtensibleWebHostBuilder(builder);

                parameters.WebHostConfigAction(builder);

                hostDescriptors.Add(
                    new WebHostDescriptor(
                        new WebHostKeywords(),
                        new WebHostRunner(builder.Build(), extensionsImpl)));
            }

            if (parameters.ServiceHostConfigAction != null)
            {
                var builder = parameters.ServiceHostBuilderFunc();
                if (builder == null)
                {
                    switch (parameters.ServiceHostKind)
                    {
                        case ServiceHostKind.Stateful:
                            throw new FactoryProducesNullInstanceException<IStatefulServiceHostBuilder>();
                        case ServiceHostKind.Stateless:
                            throw new FactoryProducesNullInstanceException<IStatelessServiceHostBuilder>();
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                builder.ConfigureObject(
                    c =>
                    {
                        c.UseWebHostBuilderExtensionsImpl(parameters.WebHostBuilderExtensionsImplFunc);
                        c.UseWebHostBuilder(parameters.WebHostBuilderFunc);
                    });

                parameters.ServiceHostConfigAction(builder);

                hostDescriptors.Add(
                    new ServiceHostDescriptor(
                        new ServiceHostKeywords(),
                        new ServiceHostRunner(builder.Build())));
            }

            var hostSelector = parameters.HostSelectorFunc();
            if (hostSelector == null)
            {
                throw new FactoryProducesNullInstanceException<IHostSelector>();
            }

            var hostDescriptor = hostSelector.Select(hostKeywordsProviders, hostDescriptors);
            if (hostDescriptor == null)
            {
                throw new InvalidOperationException(
                    $"Cannot continue build process when {nameof(IHostSelector)} returned null");
            }

            var host = parameters.HostFunc(hostDescriptor.Runner);
            if (host == null)
            {
                throw new FactoryProducesNullInstanceException<IHost>();
            }

            return host;
        }
    }
}