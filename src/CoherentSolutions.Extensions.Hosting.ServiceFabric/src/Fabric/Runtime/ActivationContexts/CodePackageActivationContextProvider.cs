using System;
using System.Fabric;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.Configurations;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ActivationContexts
{
    public class CodePackageActivationContextProvider
    {
        private readonly ServiceManifestElement manifest;

        private readonly IServiceActivationContext activationContext;

        private readonly ICodePackageActivationContextReader activationContextReader;

        public CodePackageActivationContextProvider(
            ServiceManifestElement manifest,
            IServiceActivationContext activationContext,
            ICodePackageActivationContextReader activationContextReader)
        {
            this.manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));
            this.activationContext = activationContext ?? throw new ArgumentNullException(nameof(activationContext));
            this.activationContextReader = activationContextReader ?? throw new ArgumentNullException(nameof(activationContextReader));
        }

        public ICodePackageActivationContext GetActivationContext()
        {
            return this.activationContextReader.Read(this.activationContext, this.manifest);
        }
    }
}