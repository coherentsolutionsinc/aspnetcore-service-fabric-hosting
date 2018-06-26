using Microsoft.AspNetCore.Mvc;

namespace Services.Controllers
{
    [Route("api")]
    public class StatelessController : Controller
    {
        [HttpGet]
        public string Get()
        {
            return $"I am Stateless Service!";
        }
    }
}