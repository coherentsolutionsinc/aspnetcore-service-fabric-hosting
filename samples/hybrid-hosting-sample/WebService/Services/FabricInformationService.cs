namespace WebService.Controllers
{
    public class FabricInformationService : IInformationService
    {
        public string GetExecutingEnvironment()
        {
            return "Service Fabric";
        }
    }
}