using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Service.Common;

namespace Service.Web.Controllers
{
    [Route("api")]
    public class ApiController : ControllerBase
    {
        private readonly IPersonalService personalService;
        private readonly ISharedService sharedService;

        public ApiController(
            ISharedService sharedService,
            IPersonalService personalService)
        {
            this.sharedService = sharedService;
            this.personalService = personalService;
        }

        [HttpGet]
        public Task<string> GetImportantValue()
        {
            return Task.FromResult(
                $"Shared: {this.sharedService.GetSharedValue()}; Personal: {this.personalService.GetPersonalValue()}");
        }
    }
}