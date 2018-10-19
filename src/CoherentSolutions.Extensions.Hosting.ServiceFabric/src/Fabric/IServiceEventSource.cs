namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceEventSource
    {
        void WriteEvent<T>(
            ref T eventData)
            where T : ServiceEventSourceData;
    }
}