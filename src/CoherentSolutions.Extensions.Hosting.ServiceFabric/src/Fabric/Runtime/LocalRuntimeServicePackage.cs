using System;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime
{
    public class LocalRuntimeServicePackage
    {
        public string Path
        {
            get;
        }

        public ServiceManifestElement Manifest
        {
            get;
        }

        public LocalRuntimeServicePackage(
            string path,
            ServiceManifestElement manifest)
        {
            this.Path = path ?? throw new ArgumentNullException(nameof(path));
            this.Manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));
        }
    }
}