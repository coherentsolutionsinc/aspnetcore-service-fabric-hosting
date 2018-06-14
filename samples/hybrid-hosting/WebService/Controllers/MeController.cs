using Microsoft.AspNetCore.Mvc;

namespace WebService.Controllers
{
    [Route("api/me")]
    public class MeController : Controller
    {
        private readonly IInformationService _environmentService;

        public MeController(IInformationService environmentService)
        {
            this._environmentService = environmentService;
        }

        [HttpGet]
        public string Get()
        {
            return $"Hello! I am running inside '{this._environmentService.GetExecutingEnvironment()}'.";
        }
    }
}