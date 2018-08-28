using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    [Flags]
    public enum StatefulServiceLifecycleEvent
    {
        OnRunBeforeListenersOpened = 1,

        OnRunAfterListenersOpened = 2,

        OnRunBeforeRoleChanged = 4,

        OnRunAfterRoleChanged = 8,

        OnAbort = 16,

        OnOpen = 32,

        OnClose = 64,

        OnDataLoss = 128,

        OnRestoreCompleted = 256
    }
}