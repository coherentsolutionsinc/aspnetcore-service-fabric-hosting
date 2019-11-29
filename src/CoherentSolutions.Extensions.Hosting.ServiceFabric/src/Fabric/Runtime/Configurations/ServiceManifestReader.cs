using System;
using System.IO;
using System.Xml.Serialization;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.Configurations
{
    public class ServiceManifestReader : IServiceManifestReader
    {
        private const string XML_NS = "http://schemas.microsoft.com/2011/01/fabric";

        public ServiceManifestElement Read(
            IServicePackage package)
        {
            if (package is null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            using var manifestStream = package.GetManifestStream();

            var manifest = DeserializeElement<ServiceManifestElement>(manifestStream);
            manifest.PackageRoot = package.Path;

            foreach (var configurationPackage in manifest.ConfigurationPackages)
            {
                using var settingsStream = package.GetSettingsStream(configurationPackage.Name);
                if (settingsStream is null)
                {
                    continue;
                }

                configurationPackage.Settings = DeserializeElement<ConfigurationSettingsElement>(settingsStream);
            }

            return manifest;
        }

        private static T DeserializeElement<T>(
            Stream stream)
        {
            var slz = new XmlSerializer(typeof(T), XML_NS);
            return (T) slz.Deserialize(stream);
        }
    }
}