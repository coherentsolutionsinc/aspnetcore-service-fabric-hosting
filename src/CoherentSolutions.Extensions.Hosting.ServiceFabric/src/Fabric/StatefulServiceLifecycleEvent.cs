using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    [Flags]
    public enum StatefulServiceLifecycleEvent
    {
        OnStartup = 1,

        OnRun = 2,

        OnChangeRole = 4,

        OnShutdown = 8,

        OnDataLoss = 16,

        OnRestoreCompleted = 32
    }
}