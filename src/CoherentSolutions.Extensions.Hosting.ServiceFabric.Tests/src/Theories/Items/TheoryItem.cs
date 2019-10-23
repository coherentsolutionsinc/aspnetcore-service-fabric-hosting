using System;
using System.Collections.Generic;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Items
{
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