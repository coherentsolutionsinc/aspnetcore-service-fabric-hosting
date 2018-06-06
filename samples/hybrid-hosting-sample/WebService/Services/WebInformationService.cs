namespace WebService.Controllers
{
    public class WebInformationService : IInformationService
    {
        public string GetExecutingEnvironment()
        {
            return "Web Host";
        }
    }
}