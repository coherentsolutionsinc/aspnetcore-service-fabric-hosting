using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    [Flags]
    public enum StatelessServiceLifecycleEvent
    {
        OnRunBeforeListenersOpened = 1,

        OnRunAfterListenersOpened = 2,

        OnAbort = 4,

        OnOpen = 8,

        OnClose = 16
    }
}