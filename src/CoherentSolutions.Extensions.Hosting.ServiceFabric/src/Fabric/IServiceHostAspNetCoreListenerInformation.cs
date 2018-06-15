namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostAspNetCoreListenerInformation : IServiceHostListenerInformation
    {
        string UrlSuffix { get; }
    }
}