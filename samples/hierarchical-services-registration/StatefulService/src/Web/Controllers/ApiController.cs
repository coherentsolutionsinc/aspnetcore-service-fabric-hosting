using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Service.Common;

namespace Service.Web.Controllers
{
    public class ApiController : ControllerBase
    {
        private readonly ISharedService sharedService;
        private readonly IPersonalService personalService;

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