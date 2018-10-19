using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceEventSource
    {
        public Func<IServiceEventSource> CreateEventSourceFunc { get; }

        public StatelessServiceEventSource(
            Func<IServiceEventSource> eventSourceFunc)
        {
            this.CreateEventSourceFunc = eventSourceFunc
             ?? throw new ArgumentNullException(nameof(eventSourceFunc));
        }
    }
}