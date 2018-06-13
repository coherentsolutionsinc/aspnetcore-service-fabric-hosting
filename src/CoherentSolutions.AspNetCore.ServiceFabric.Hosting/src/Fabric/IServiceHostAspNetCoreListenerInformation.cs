namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IServiceHostAspNetCoreListenerInformation : IServiceHostListenerInformation
    {
        string UrlSuffix { get; }
    }
}