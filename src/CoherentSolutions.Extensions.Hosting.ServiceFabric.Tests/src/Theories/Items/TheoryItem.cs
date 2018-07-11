using System;
using System.Collections.Generic;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions;

using Microsoft.Extensions.Hosting;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Items
{
    public abstract class TheoryItem
    {
        private readonly string name;

        private readonly Dictionary<Type, ITheoryExtension> extensions;

        private readonly LinkedList<Action<HostBuilder>> configActions;

        private readonly LinkedList<Action<IHost>> checkActions;

        private bool initializingExtensions;

        protected TheoryItem(
            string name)
        {
            this.name = name
             ?? throw new ArgumentNullException(nameof(name));

            this.extensions = new Dictionary<Type, ITheoryExtension>();
            this.configActions = new LinkedList<Action<HostBuilder>>();
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

        protected abstract void InitializeExtensions();

        public void SetupExtension<T>(
            T extension)
            where T : class, ITheoryExtension
        {
            foreach (var @interface in typeof(T).GetInterfaces())
            {
                if (typeof(ITheoryExtension) == @interface)
                {
                    continue;
                }

                if (typeof(ITheoryExtension).IsAssignableFrom(@interface))
                {
                    if (!this.initializingExtensions && !this.extensions.ContainsKey(@interface))
                    {
                        throw new InvalidOperationException($"{@interface.Name} isn't supported by current {nameof(TheoryItem)}.");
                    }

                    this.extensions[@interface] = extension;
                }
            }
        }

        public void SetupConfig(
            Action<HostBuilder> configAction)
        {
            if (configAction == null)
            {
                throw new ArgumentNullException(nameof(configAction));
            }

            this.configActions.AddLast(configAction);
        }

        public void SetupCheck(
            Action<IHost> checkAction)
        {
            if (checkAction == null)
            {
                throw new ArgumentNullException(nameof(checkAction));
            }

            this.checkActions.AddLast(checkAction);
        }

        public void Try()
        {
            var builder = new HostBuilder();

            foreach (var configAction in this.configActions)
            {
                configAction(builder);
            }

            var host = builder.Build();

            foreach (var checkAction in this.checkActions)
            {
                checkAction(host);
            }

            host.StartAsync().GetAwaiter().GetResult();
            host.StopAsync().GetAwaiter().GetResult();
        }

        public TheoryItem Initialize()
        {
            this.initializingExtensions = true;
            try
            {
                this.InitializeExtensions();
            }
            finally
            {
                this.initializingExtensions = false;
            }

            return this;
        }

        protected T GetExtension<T>()
            where T : class, ITheoryExtension
        {
            return this.extensions.TryGetValue(typeof(T), out var extension)
                ? (T) extension
                : null;
        }
    }
}