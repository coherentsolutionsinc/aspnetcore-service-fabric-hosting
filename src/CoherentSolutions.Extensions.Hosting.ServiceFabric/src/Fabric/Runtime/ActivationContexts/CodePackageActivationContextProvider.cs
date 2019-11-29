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
            return this.activationContextReader.Read(
                this.manifestProvider.GetManifest());
        }
    }
}