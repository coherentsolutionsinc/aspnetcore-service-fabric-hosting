namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public class ServiceHostKeywords : IServiceHostKeywords
    {
        private readonly string[] keywords;

        public ServiceHostKeywords()
        {
            this.keywords = new[]
            {
                HostKeywords.ENVIRONMENT_ASPNET_CORE,
                HostKeywords.ENVIRONMENT_SERVICE_FABRIC
            };
        }

        public string[] GetKeywords()
        {
            return this.keywords;
        }
    }
}