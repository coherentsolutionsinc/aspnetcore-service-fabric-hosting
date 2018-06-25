using Microsoft.AspNetCore.Mvc;

namespace Services.Controllers
{
    [Route("api/me")]
    public class StatelessController : Controller
    {
        [HttpGet]
        public string Get()
        {
            return $"I am Stateless Service!";
        }
    }
}