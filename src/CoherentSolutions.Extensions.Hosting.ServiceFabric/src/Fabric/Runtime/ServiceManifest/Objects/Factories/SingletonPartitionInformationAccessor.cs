using System.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest.Objects.Factories
{
    public class SingletonPartitionInformationAccessor
        : ServicePartitionInformationAccessor<SingletonPartitionInformation>
    {
        public SingletonPartitionInformationAccessor(
            SingletonPartitionInformation instance) 
            : base(instance)
        {
        }
    }
}