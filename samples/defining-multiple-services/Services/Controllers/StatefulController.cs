using Microsoft.AspNetCore.Mvc;

namespace Services.Controllers
{
    [Route("api/me")]
    public class StatefulController : Controller
    {
        [HttpGet]
        public string Get()
        {
            return $"I am Stateful Service!";
        }
    }
}