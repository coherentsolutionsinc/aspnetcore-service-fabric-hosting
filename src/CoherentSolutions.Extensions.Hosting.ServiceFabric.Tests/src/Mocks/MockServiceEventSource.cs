using System.Diagnostics.Tracing;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Mocks
{
    public class MockServiceEventSource : IServiceEventSource
    {
        public bool IsEnabled(
            EventLevel eventLevel)
        {
            return false;
        }

        public void Verbose<T>(
            int eventId,
            string eventName,
            string eventCategoryName,
            string eventMessage,
            T eventData)
            where T : ServiceEventSourceData
        {
        }

        public void Information<T>(
            int eventId,
            string eventName,
            string eventCategoryName,
            string eventMessage,
            T eventData)
            where T : ServiceEventSourceData
        {
        }

        public void Warning<T>(
            int eventId,
            string eventName,
            string eventCategoryName,
            string eventMessage,
            T eventData)
            where T : ServiceEventSourceData
        {
        }

        public void Error<T>(
            int eventId,
            string eventName,
            string eventCategoryName,
            string eventMessage,
            T eventData)
            where T : ServiceEventSourceData
        {
        }

        public void Critical<T>(
            int eventId,
            string eventName,
            string eventCategoryName,
            string eventMessage,
            T eventData)
            where T : ServiceEventSourceData
        {
        }
    }
}