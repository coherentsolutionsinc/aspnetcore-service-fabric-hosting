using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    [Flags]
    public enum StatefulServiceLifecycleEvent
    {
        OnStartup = 1,

        OnChangeRole = 1 << 1,

        OnRun = 1 << 2,

        OnShutdown = 1 << 3,

        OnDataLoss = 1 << 4,

        OnRestoreCompleted = 1 << 5,

        OnCodePackageAdded = 1 << 6,

        OnCodePackageModified = 1 << 7,

        OnCodePackageRemoved = 1 << 8,

        OnConfigPackageAdded = 1 << 9,

        OnConfigPackageModified = 1 << 10,

        OnConfigPackageRemoved = 1 << 11,

        OnDataPackageAdded = 1 << 12,

        OnDataPackageModified = 1 << 13,

        OnDataPackageRemoved = 1 << 14
    }
}