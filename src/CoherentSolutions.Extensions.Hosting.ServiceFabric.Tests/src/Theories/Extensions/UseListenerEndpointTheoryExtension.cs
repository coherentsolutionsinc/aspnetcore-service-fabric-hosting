namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public class UseListenerEndpointTheoryExtension : IUseListenerEndpointTheoryExtension
    {
        public string Endpoint { get; private set; }

        public UseListenerEndpointTheoryExtension()
        {
            this.Endpoint = string.Empty;
        }

        public UseListenerEndpointTheoryExtension Setup(
            string endpoint)
        {
            this.Endpoint = endpoint;

            return this;
        }
    }
}