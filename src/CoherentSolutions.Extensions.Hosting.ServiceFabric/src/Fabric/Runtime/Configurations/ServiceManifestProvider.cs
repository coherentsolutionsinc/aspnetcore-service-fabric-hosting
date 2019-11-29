using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.Configurations
{
    public class ServiceManifestProvider : IServiceManifestProvider
    {
        private readonly IServicePackage package;

        private readonly IServiceManifestReader manifestReader;

        public ServiceManifestProvider(
            IServicePackage package,
            IServiceManifestReader manifestReader)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            this.manifestReader = manifestReader ?? throw new ArgumentNullException(nameof(manifestReader));
        }

        public ServiceManifestElement GetManifest()
        {
            return this.manifestReader.Read(this.package);
        }
    }
}