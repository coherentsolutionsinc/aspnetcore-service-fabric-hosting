using Microsoft.AspNetCore.Mvc;

namespace Services.Controllers
{
    [Route("api")]
    public class StatefulController : Controller
    {
        [HttpGet]
        public string Get()
        {
            return $"I am Stateful Service!";
        }
    }
}