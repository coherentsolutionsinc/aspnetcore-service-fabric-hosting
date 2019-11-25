using System;
using System.IO;
using System.Xml.Serialization;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest.Objects
{
    public class ServicePackage
    {
        private const string MANIFEST_FILE_NAME = "ServiceManifest.xml";
        private const string SETTINGS_FILE_NAME = "Settings.xml";

        private const string XML_NS = "http://schemas.microsoft.com/2011/01/fabric";

        public string Path
        {
            get;
        }

        public ServiceManifestElement Manifest
        {
            get;
        }

        public ServicePackage(
            string path,
            ServiceManifestElement manifest)
        {
            this.Path = path ?? throw new ArgumentNullException(nameof(path));
            this.Manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));
        }

        public static bool TryLoad(
            string path,
            out ServicePackage package)
        {
            if (!Directory.Exists(path))
            {
                package = null;
                return false;
            }

            var manifestPath = System.IO.Path.Combine(path, MANIFEST_FILE_NAME);
            if (!File.Exists(manifestPath))
            {
                package = null;
                return false;
            }

            using (var stream = File.Open(manifestPath, FileMode.Open))
            {
                var serializer = new XmlSerializer(typeof(ServiceManifestElement), XML_NS);

                var manifest = (ServiceManifestElement)serializer.Deserialize(stream);
                manifest.PackageRoot = path;

                var settingsserializer = new XmlSerializer(typeof(ConfigurationSettingsElement), XML_NS);
                foreach (var p in manifest.ConfigPackages)
                {
                    var configPath = System.IO.Path.Combine(path, p.Name, SETTINGS_FILE_NAME);
                    if (File.Exists(configPath))
                    {
                        using (var cccc = File.Open(configPath, FileMode.Open))
                        {
                            p.Settings = (ConfigurationSettingsElement)settingsserializer.Deserialize(cccc);
                        }
                    }
                }

                package = new ServicePackage(path, manifest);
                return true;
            }
        }
    }
}