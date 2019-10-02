﻿namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest.Objects.Factories
{
    public interface IPackageFactory<TElement, TPackage>
    {
        TPackage Create(TElement element);
    }
}