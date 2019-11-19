using System;
using System.Collections.Generic;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Items
{
    public interface ITheoryItem
    {
        ITheoryItem SetupExtension(
            ITheoryExtension extension);

        void Verify();
    }

    public interface ITheoryExtension
    {
    }

    public interface ITheoryExtensionsCollection
    {
        T Get<T>()
            where T : class, ITheoryExtension;
    }

    public sealed class TheoryItemEx<T> : ITheoryItem
        where T : class
    {
        private sealed class TheoryItemExtensionsCollection : ITheoryExtensionsCollection
        {
            private readonly Dictionary<Type, ITheoryExtension> extensions;

            public TheoryItemExtensionsCollection()
            {
                this.extensions = new Dictionary<Type, ITheoryExtension>();
            }

            public TExtension Get<TExtension>()
                where TExtension : class, ITheoryExtension
            {
                return this.extensions.TryGetValue(typeof(TExtension), out var extension)
                    ? (TExtension)extension
                    : null;
            }

            public void Set(
                ITheoryExtension extension)
            {
                if (extension is null)
                {
                    throw new ArgumentNullException(nameof(extension));
                }

                this.extensions[extension.GetType()] = extension;
            }
        }

        private readonly TheoryItemExtensionsCollection extensions;
        private readonly LinkedList<Action<T>> extensionConfigActions;

        private Action verify;

        public TheoryItemEx()
        {
            this.extensions = new TheoryItemExtensionsCollection();
            this.extensionConfigActions = new LinkedList<Action<T>>();
        }

        public TheoryItemEx<T> UseExtension<TExtension>(
            TExtension extension,
            Action<T, TExtension> extensionConfigAction)
            where TExtension : class, ITheoryExtension
        {
            this.extensions.Set(extension);
            this.extensionConfigActions.AddLast(
                target =>
                {
                    var ext = this.extensions.Get<TExtension>();
                    extensionConfigAction(target, ext);
                });

            return this;
        }

        public TheoryItemEx<T> UseTarget(
            T target,
            Action<T> targetActivateAction)
        {
            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (targetActivateAction is null)
            {
                throw new ArgumentNullException(nameof(targetActivateAction));
            }

            this.verify = new Action(
                () =>
                {
                    foreach (var extensionConfigAction in this.extensionConfigActions)
                    {
                        extensionConfigAction(target);
                    }

                    targetActivateAction(target);
                });

            return this;
        }

        public ITheoryItem SetupExtension(
            ITheoryExtension extension)
        {
            this.extensions.Set(extension);

            return this;
        }

        public void Verify()
        {
            this.verify();
        }
    }

    public static class A
    {
        public static void BB()
        {
            var item = new TheoryItemEx<StatelessServiceHostDelegateReplicaTemplate>()
                .UseExtension(
                    new UseDependenciesTheoryExtension(),
                    (host, extension) =>
                    {
                        host.ConfigureObject(
                            c =>
                            {
                                c.UseDependencies(extension.Factory);
                            });
                    })
                .UseExtension(
                    new PickDependencyTheoryExtension(),
                    (host, extension) =>
                    {
                        host.ConfigureObject(
                            c =>
                            {
                                c.UseDelegateInvoker(provider =>
                                {
                                    foreach (var pick in extension.PickActions)
                                    {
                                        pick(provider);
                                    }
                                    return null;
                                });
                            });
                    })
                .UseTarget(
                    new StatelessServiceHostDelegateReplicaTemplate(),
                    replicaTemplate =>
                    {
                        var instance = replicaTemplate.Activate(null);
                        instance.CreateDelegateInvoker().InvokeAsync(instance.Delegate, null, default);
                    });
            item.Verify();
        }
    }

    public class TheoryItem
    {
        public sealed class TheoryItemExtensionProvider
        {
            private readonly TheoryItem item;

            public TheoryItemExtensionProvider(
                TheoryItem item)
            {
                this.item = item;
            }

            public T GetExtension<T>()
                where T : class
            {
                return this.item.extensions.TryGetValue(typeof(T), out var extension)
                    ? (T) extension
                    : null;
            }
        }

        private readonly string name;

        private readonly Dictionary<Type, object> extensions;

        private readonly LinkedList<Action<HostBuilder, TheoryItemExtensionProvider>> configActions;

        private readonly LinkedList<Action<IHost>> checkActions;

        public TheoryItem(
            string name)
        {
            this.name = name
                ?? throw new ArgumentNullException(nameof(name));

            this.extensions = new Dictionary<Type, object>();
            this.configActions = new LinkedList<Action<HostBuilder, TheoryItemExtensionProvider>>();
            this.checkActions = new LinkedList<Action<IHost>>();
        }

        public override string ToString()
        {
            return this.name;
        }

        public override int GetHashCode()
        {
            return this.name.GetHashCode();
        }

        public override bool Equals(
            object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj is TheoryItem other && this.name.Equals(other.name);
        }

        public TheoryItem SetupExtension<T>(
            T extension)
            where T : class
        {
            foreach (var @interface in typeof(T).GetInterfaces())
            {
                this.extensions[@interface] = extension;
            }

            return this;
        }

        public TheoryItem SetupConfig(
            Action<HostBuilder, TheoryItemExtensionProvider> configAction)
        {
            if (configAction is null)
            {
                throw new ArgumentNullException(nameof(configAction));
            }

            this.configActions.AddLast(configAction);

            return this;
        }

        public TheoryItem SetupCheck(
            Action<IHost> checkAction)
        {
            if (checkAction is null)
            {
                throw new ArgumentNullException(nameof(checkAction));
            }

            this.checkActions.AddLast(checkAction);

            return this;
        }

        public void Try()
        {
            var provider = new TheoryItemExtensionProvider(this);

            var builder = new HostBuilder();
            builder.ConfigureServices(
                services =>
                {
                    services.Configure<ConsoleLifetimeOptions>(
                        options => options.SuppressStatusMessages = true);
                });

            foreach (var configAction in this.configActions)
            {
                configAction(builder, provider);
            }

            var host = builder.Build();

            foreach (var checkAction in this.checkActions)
            {
                checkAction(host);
            }

            host.StartAsync().GetAwaiter().GetResult();
            host.StopAsync().GetAwaiter().GetResult();
        }
    }
}