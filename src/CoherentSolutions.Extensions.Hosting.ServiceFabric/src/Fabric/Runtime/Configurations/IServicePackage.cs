using System.IO;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.Configurations
{
    public interface IServicePackage
    {
        string Path
        {
            get;
        }

        Stream GetManifestStream();

        Stream GetSettingsStream(
            string configurationPackage);
    }
}