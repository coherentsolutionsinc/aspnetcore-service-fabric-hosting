using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceEventSource
    {
        public Func<IServiceEventSource> CreateEventSourceFunc { get; }

        public StatefulServiceEventSource(
            Func<IServiceEventSource> eventSourceFunc)
        {
            if (eventSourceFunc == null)
            {
                throw new ArgumentNullException(nameof(eventSourceFunc));
            }

            var lazy = new Lazy<IServiceEventSource>(eventSourceFunc);

            this.CreateEventSourceFunc = () => lazy.Value;
        }
    }
}