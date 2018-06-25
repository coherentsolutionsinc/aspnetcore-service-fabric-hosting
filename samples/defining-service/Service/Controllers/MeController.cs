using Microsoft.AspNetCore.Mvc;

namespace Service.Controllers
{
    [Route("api/me")]
    public class MeController : Controller
    {
        [HttpGet]
        public string Get()
        {
            return $"Hello! I am running inside Service Fabric!";
        }
    }
}