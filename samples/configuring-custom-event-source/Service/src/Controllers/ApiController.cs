using System;

using Microsoft.AspNetCore.Mvc;

namespace Service.Controllers
{
    [Route("api")]
    public class ApiController : Controller
    {
        private readonly IApiServiceEventSource eventSource;

        public ApiController(
            IApiServiceEventSource eventSource)
        {
            this.eventSource = eventSource
             ?? throw new ArgumentNullException(nameof(eventSource));
        }

        [HttpGet]
        [Route("value")]
        public string GetValue()
        {
            this.eventSource.GetValueMethodInvoked();
            return $"Value from {nameof(ApiController)}";
        }
    }
}