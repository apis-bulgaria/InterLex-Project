using Microsoft.AspNetCore.Mvc;

namespace Interlex.Controllers
{
    using ClassificationService;
    using Microsoft.AspNetCore.Authorization;
    using Services;

    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly DataService service;

        public DataController(DataService service)
        {
            this.service = service;
        }

        [HttpGet("GetEuLanguages")]
        public IActionResult GetEuLanguages()
        {
            var suggestions = this.service.GetEuLanguages();
            return this.Ok(suggestions);
        }
    }
}