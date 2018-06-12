namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Web
{
    public class WebHostKeywords : IWebHostKeywords
    {
        private readonly string[] keywords;

        public WebHostKeywords()
        {
            this.keywords = new[]
            {
                HostKeywords.ENVIRONMENT_ASPNET_CORE
            };
        }

        public string[] GetKeywords()
        {
            return this.keywords;
        }
    }
}