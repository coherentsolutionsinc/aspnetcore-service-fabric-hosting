using System;
using System.IO;
using System.Reflection;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest;
using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime
{
    public static class LocalRuntimeServicePackageProvider
    {
        private struct Locations
        {
            public readonly string SearchRootDirectory;

            public readonly string PackageRootDirectory;
            
            public readonly string ServiceManifestXmlFile;

            public readonly Dictionary<string, string> SettingsXmlFiles;
            
            public Locations(
                string searchRootDirectory,
                string packageRootDirectory,
                string serviceManifestXmlFile,
                Dictionary<string, string> settingsXmlFiles)
            {
                this.SearchRootDirectory = searchRootDirectory;
                this.PackageRootDirectory = packageRootDirectory;
                this.ServiceManifestXmlFile = serviceManifestXmlFile;
                this.SettingsXmlFiles = settingsXmlFiles;
            }
        }

        private const string PACKAGE_DIRECTORY_NAME = "PackageRoot";

        private const string MANIFEST_FILE_NAME = "ServiceManifest.xml";

        private const string SETTINGS_FILE_NAME = "Settings.xml";

        private const string XML_NS = "http://schemas.microsoft.com/2011/01/fabric";

        public static LocalRuntimeServicePackage GetPackage()
        {
            var locations = GetLocations();

            if (locations.PackageRootDirectory is null || locations.ServiceManifestXmlFile is null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        $"Could not locate '{PACKAGE_DIRECTORY_NAME}{{1}}{MANIFEST_FILE_NAME}' " +
                        $"inside {{0}} directory tree.",
                        locations.SearchRootDirectory,
                        Path.DirectorySeparatorChar));
            }

            var manifest = DeserializeElement<ServiceManifestElement>(locations.ServiceManifestXmlFile);
            manifest.PackageRoot = locations.PackageRootDirectory;

            foreach (var configurationPackage in manifest.ConfigurationPackages)
            {
                if (!locations.SettingsXmlFiles.TryGetValue(configurationPackage.Name, out var settings))
                {
                    throw new InvalidOperationException(
                        string.Format(
                            $"Could not locate '{SETTINGS_FILE_NAME}' of " +
                            $"Configuration Package '{configurationPackage.Name}' " +
                            $"inside '{manifest.PackageRoot}{{0}}{configurationPackage.Name}' directory.",
                            Path.DirectorySeparatorChar));
                }
                configurationPackage.Settings = DeserializeElement<ConfigurationSettingsElement>(settings);
            }

            return new LocalRuntimeServicePackage(locations.PackageRootDirectory, manifest);
        }

        private static Locations GetLocations()
        {
            var location = Assembly.GetExecutingAssembly().Location;
            var current = location;

            var searchRootDirectory = Path.GetDirectoryName(location) ?? Path.GetPathRoot(location);
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

                return new Locations(
                    searchRootDirectory,
                    packageRootDirectory,
                    serviceManifestXmlFile,
                    Directory
                        .EnumerateDirectories(packageRootDirectory)
                        .Select(i => (name: Path.GetFileName(i), settings: Path.Combine(i, SETTINGS_FILE_NAME)))
                        .Where(i => File.Exists(i.settings))
                        .ToDictionary(i => i.name, i => i.settings));
            }

            return new Locations(searchRootDirectory, null, null, null);
        }

        private static T DeserializeElement<T>(
            string path)
        {
            using (var stream = File.Open(path, FileMode.Open))
            {
                var slz = new XmlSerializer(typeof(T), XML_NS);
                return (T)slz.Deserialize(stream);
            }
        }
    }
}