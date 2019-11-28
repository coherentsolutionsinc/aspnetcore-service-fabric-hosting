using System;
using System.Fabric;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.Configurations;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ActivationContexts
{
    public class CodePackageActivationContextProvider : ICodePackageActivationContextProvider
    {
        private readonly IServiceManifestProvider manifestProvider;

        private readonly ICodePackageActivationContextReader activationContextReader;

        public CodePackageActivationContextProvider(
            IServiceManifestProvider manifestProvider,
            ICodePackageActivationContextReader activationContextReader)
        {
            this.manifestProvider = manifestProvider ?? throw new ArgumentNullException(nameof(manifestProvider));
            this.activationContextReader = activationContextReader ?? throw new ArgumentNullException(nameof(activationContextReader));
        }

        public ICodePackageActivationContext GetActivationContext()
        {
            var manifest = this.manifestProvider.GetManifest();
            if (manifest is null)
            {
                throw new InvalidOperationException();
            }

            return this.activationContextReader.Read(manifest);
        }
    }
}