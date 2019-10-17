using System;
using System.Fabric;

using Microsoft.AspNetCore.Mvc;

namespace Service.Controllers
{
    [Route("api")]
    public class ApiController : Controller
    {
        private readonly ServiceContext context;

        public ApiController(
            ServiceContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        [Route("value")]
        public string GetValue()
        {
            var config = this.context.CodePackageActivationContext.GetConfigurationPackageObject("Config");
            var settings = config.Settings;
            
            return $"Value from {nameof(ApiController)}";
        }
    }
}