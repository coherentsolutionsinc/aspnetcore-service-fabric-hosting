using System;
using System.Collections.Generic;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools
{
    public abstract class ConfigurableObject<TConfigurator> : IConfigurableObject<TConfigurator>
    {
        private readonly Queue<Action<TConfigurator>> configuratorDelegates;

        protected ConfigurableObject()
        {
            this.configuratorDelegates = new Queue<Action<TConfigurator>>();
        }

        public void ConfigureObject(
            Action<TConfigurator> configAction)
        {
            if (configAction == null)
            {
                throw new ArgumentNullException(nameof(configAction));
            }

            this.configuratorDelegates.Enqueue(configAction);
        }

        protected void UpstreamConfiguration(
            TConfigurator configurator)
        {
            if (configurator == null)
            {
                throw new ArgumentNullException(nameof(configurator));
            }

            foreach (var configuratorDelegate in this.configuratorDelegates)
            {
                configuratorDelegate(configurator);
            }
        }
    }
}