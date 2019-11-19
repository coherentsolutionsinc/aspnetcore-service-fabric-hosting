using System;
using System.IO;
using System.Xml.Serialization;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest.Objects
{
    public class ServicePackage
    {
        private const string SERVICE_MANIFEST_FILE = "ServiceManifest.xml";

        private const string SERVICE_MANIFEST_XML_NS = "http://schemas.microsoft.com/2011/01/fabric";

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

        public static ServicePackage Load(
            string path)
        {
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException($"{path} doesn't exist.");
            }

            var manifestPath = System.IO.Path.Combine(path, SERVICE_MANIFEST_FILE);
            if (!File.Exists(manifestPath))
            {
                throw new FileNotFoundException($"{SERVICE_MANIFEST_FILE} doesn't exist.", manifestPath);
            }

            using (var stream = File.Open(manifestPath, FileMode.Open))
            {
                var serializer = new XmlSerializer(typeof(ServiceManifestElement), SERVICE_MANIFEST_XML_NS);

                var manifest = (ServiceManifestElement)serializer.Deserialize(stream);
                manifest.PackageRoot = path;

                return new ServicePackage(path, manifest);
            }
        }
    }
}