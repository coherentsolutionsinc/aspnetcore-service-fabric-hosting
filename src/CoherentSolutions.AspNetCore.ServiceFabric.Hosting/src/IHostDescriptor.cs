namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting
{
    public interface IHostDescriptor
    {
        IHostKeywords Keywords { get; }

        IHostRunner Runner { get; }
    }
}