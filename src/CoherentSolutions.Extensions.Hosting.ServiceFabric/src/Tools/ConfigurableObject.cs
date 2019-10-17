using System;
using System.Collections.Generic;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools
{
    public abstract class ConfigurableObject<TConfigurator> : IConfigurableObject<TConfigurator>
    {
        private readonly Queue<Action<TConfigurator>> configActions;

        protected ConfigurableObject()
        {
            this.configActions = new Queue<Action<TConfigurator>>();
        }

        public void ConfigureObject(
            Action<TConfigurator> configAction)
        {
            if (configAction is null)
            {
                throw new ArgumentNullException(nameof(configAction));
            }

            this.configActions.Enqueue(configAction);
        }

        protected void UpstreamConfiguration(
            TConfigurator configurator)
        {
            if (configurator is null)
            {
                throw new ArgumentNullException(nameof(configurator));
            }

            foreach (var configAction in this.configActions)
            {
                configAction(configurator);
            }
        }
    }
}