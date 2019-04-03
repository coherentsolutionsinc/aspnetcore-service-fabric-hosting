namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class ServiceEventPayloadOnPackageModified<TPackage>
        : IServiceEventPayloadOnPackageModified<TPackage>
    {
        public TPackage OldPackage { get; }

        public TPackage NewPackage { get; }

        public ServiceEventPayloadOnPackageModified(
            TPackage oldPackage,
            TPackage newPackage)
        {
            this.OldPackage = oldPackage;
            this.NewPackage = newPackage;
        }
    }
}