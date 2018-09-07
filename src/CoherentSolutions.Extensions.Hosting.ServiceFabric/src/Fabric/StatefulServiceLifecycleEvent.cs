using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    [Flags]
    public enum StatefulServiceLifecycleEvent
    {
        OnStartup = 1,

        OnChangeRole = 2,

        OnRun = 4,

        OnShutdown = 8,

        OnDataLoss = 16,

        OnRestoreCompleted = 32
    }
}