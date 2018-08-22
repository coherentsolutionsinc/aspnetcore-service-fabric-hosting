using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    [Flags]
    public enum StatefulServiceLifecycleEvent
    {
        OnRunBeforeListenersAreOpened = 1,

        OnRunAfterListenersAreOpened = 2,

        OnRunBeforeRoleChanged = 4,

        OnRunAfterRoleChanged = 8,

        OnAbort = 16,

        OnOpen = 32,

        OnClose = 64,

        OnDataLoss = 128,

        OnRestoreCompleted = 256
    }
}