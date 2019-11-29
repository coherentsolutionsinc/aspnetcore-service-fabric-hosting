using System;
using System.Collections.Generic;
using System.IO;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.Configurations
{
    public class ServicePackage : IServicePackage
    {
        private readonly string manifestXml;

        private readonly IReadOnlyDictionary<string, string> settingsXmls;

        public string Path { get; }

        public ServicePackage(
            string packageRoot,
            string manifestXml,
            IReadOnlyDictionary<string, string> settingsXmls)
        {
            this.Path = packageRoot ?? throw new ArgumentNullException(nameof(packageRoot));
            this.manifestXml = manifestXml ?? throw new ArgumentNullException(nameof(manifestXml));
            this.settingsXmls = settingsXmls ?? throw new ArgumentNullException(nameof(settingsXmls));
        }

        public Stream GetManifestStream()
        {
            return File.OpenRead(this.manifestXml);
        }

        public Stream GetSettingsStream(
            string configurationPackage)
        {
            return this.settingsXmls.TryGetValue(configurationPackage, out var path)
                ? File.OpenRead(path)
                : null;
        }
    }
}