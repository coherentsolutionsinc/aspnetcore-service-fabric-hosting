using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceEventSource
    {
        public Func<IServiceEventSource> CreateEventSource { get; }

        public StatelessServiceEventSource(
            Func<IServiceEventSource> eventSourceFunc)
        {
            this.CreateEventSource = eventSourceFunc 
                ?? throw new ArgumentNullException(nameof(eventSourceFunc));
        }
    }
}