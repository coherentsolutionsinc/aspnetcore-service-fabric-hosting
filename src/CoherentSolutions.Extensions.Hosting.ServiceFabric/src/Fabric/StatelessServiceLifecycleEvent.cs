using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    [Flags]
    public enum StatelessServiceLifecycleEvent
    {
        OnRunBeforeListenersAreOpened = 1,
        OnRunAfterListenersAreOpened = 2,
        OnAbort = 4,
        OnOpen = 8,
        OnClose = 16
    }
}