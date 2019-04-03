using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    [Flags]
    public enum StatelessServiceLifecycleEvent
    {
        OnStartup = 1,

        OnRun = 1 << 1,

        OnShutdown = 1 << 2,

        OnCodePackageAdded = 1 << 3,

        OnCodePackageModified = 1 << 4,

        OnCodePackageRemoved = 1 << 5,

        OnConfigPackageAdded = 1 << 6,

        OnConfigPackageModified = 1 << 7,

        OnConfigPackageRemoved = 1 << 8,

        OnDataPackageAdded = 1 << 9,

        OnDataPackageModified = 1 << 10,

        OnDataPackageRemoved = 1 << 11
    }
}