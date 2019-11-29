using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.Configurations
{
    public class ServiceManifestProvider : IServiceManifestProvider
    {
        private readonly IServicePackageProvider packageProvider;

        private readonly IServiceManifestReader manifestReader;

        public ServiceManifestProvider(
            IServicePackageProvider packageProvider,
            IServiceManifestReader manifestReader)
        {
            this.packageProvider = packageProvider ?? throw new ArgumentNullException(nameof(packageProvider));
            this.manifestReader = manifestReader ?? throw new ArgumentNullException(nameof(manifestReader));
        }

        public ServiceManifestElement GetManifest()
        {
            return this.manifestReader.Read(
                this.packageProvider.GetPackage());
        }
    }
}