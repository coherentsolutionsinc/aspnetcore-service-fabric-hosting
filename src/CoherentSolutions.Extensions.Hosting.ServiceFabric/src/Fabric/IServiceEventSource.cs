using System.Diagnostics.Tracing;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceEventSource
    {
        bool IsEnabled(
            EventLevel eventLevel);

        void Verbose<T>(
            int eventId,
            string eventName,
            string eventCategoryName,
            string eventMessage,
            T eventData)
            where T : ServiceEventSourceData;

        void Information<T>(
            int eventId,
            string eventName,
            string eventCategoryName,
            string eventMessage,
            T eventData)
            where T : ServiceEventSourceData;

        void Warning<T>(
            int eventId,
            string eventName,
            string eventCategoryName,
            string eventMessage,
            T eventData)
            where T : ServiceEventSourceData;

        void Error<T>(
            int eventId,
            string eventName,
            string eventCategoryName,
            string eventMessage,
            T eventData)
            where T : ServiceEventSourceData;

        void Critical<T>(
            int eventId,
            string eventName,
            string eventCategoryName,
            string eventMessage,
            T eventData)
            where T : ServiceEventSourceData;
    }
}