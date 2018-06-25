using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Service.Common;

namespace Service.Web.Controllers
{
    public class ManagementApiController : ControllerBase
    {
        private readonly IManagementService managementService;

        public ManagementApiController(
            IManagementService managementService)
        {
            this.managementService = managementService;
        }

        [HttpGet]
        public Task<string> GetImportantValue()
        {
            return Task.FromResult(this.managementService.GetImportantValue());
        }
    }
}