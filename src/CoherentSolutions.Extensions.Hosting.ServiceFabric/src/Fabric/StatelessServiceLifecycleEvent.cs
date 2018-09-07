using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    [Flags]
    public enum StatelessServiceLifecycleEvent
    {
        OnStartup = 1,

        OnRun = 2,

        OnShutdown = 4
    }
}