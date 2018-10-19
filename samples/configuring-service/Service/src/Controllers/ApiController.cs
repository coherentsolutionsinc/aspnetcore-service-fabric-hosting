using Microsoft.AspNetCore.Mvc;

namespace Service.Controllers
{
    [Route("api")]
    public class ApiController : Controller
    {
        [HttpGet]
        [Route("value")]
        public string GetValue()
        {
            return $"Value from {nameof(ApiController)}";
        }
    }
}