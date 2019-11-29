using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.Configurations
{
    public class ServicePackageProvider : IServicePackageProvider
    {
        private const string PACKAGE_DIRECTORY_NAME = "PackageRoot";

        private const string MANIFEST_FILE_NAME = "ServiceManifest.xml";

        private const string SETTINGS_FILE_NAME = "Settings.xml";

        public IServicePackage GetPackage()
        {
            var location = Assembly.GetExecutingAssembly().Location;
            var current = location;

            var br = false;
            for (; !br;)
            {
                current = Path.GetDirectoryName(current);
                if (current is null)
                {
                    current = Path.GetPathRoot(location);
                    br = true;
                }

                var packageRootDirectory = Path.Combine(current, PACKAGE_DIRECTORY_NAME);
                if (!Directory.Exists(packageRootDirectory))
                {
                    continue;
                }

                var serviceManifestXmlFile = Path.Combine(packageRootDirectory, MANIFEST_FILE_NAME);
                if (!File.Exists(serviceManifestXmlFile))
                {
                    continue;
                }

                return new ServicePackage(
                    packageRootDirectory,
                    serviceManifestXmlFile,
                    Directory
                       .EnumerateDirectories(packageRootDirectory)
                       .Select(i => (name: Path.GetFileName(i), settings: Path.Combine(i, SETTINGS_FILE_NAME)))
                       .Where(i => File.Exists(i.settings))
                       .ToDictionary(i => i.name, i => i.settings));
            }

            throw new InvalidOperationException(
                $"Cannot find '{PACKAGE_DIRECTORY_NAME}{Path.DirectorySeparatorChar}{MANIFEST_FILE_NAME}'. "
              + $"Searched paths: {current} -> {Path.GetDirectoryName(location) ?? Path.GetPathRoot(location)}");
        }
    }
}