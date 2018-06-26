using Microsoft.AspNetCore.Mvc;

namespace Services.Controllers
{
    [Route("api")]
    public class StatelessController : Controller
    {
        [HttpGet]
        [Route("value")]
        public string Get()
        {
            return $"I am Stateless Service!";
        }
    }
}