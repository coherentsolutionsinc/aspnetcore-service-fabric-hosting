using System;
using System.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceEventSource
    {
        public Func<StatefulServiceContext, IServiceEventSource> CreateEventSourceFunc { get; }

        public StatefulServiceEventSource(
            Func<StatefulServiceContext, IServiceEventSource> eventSourceFunc)
        {
            this.CreateEventSourceFunc = eventSourceFunc
             ?? throw new ArgumentNullException(nameof(eventSourceFunc));
        }
    }
}