using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceEventSource
    {
        public Func<IServiceEventSource> CreateEventSourceFunc { get; }

        public StatelessServiceEventSource(
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