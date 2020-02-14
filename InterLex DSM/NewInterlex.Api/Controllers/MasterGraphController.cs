namespace NewInterlex.Api.Controllers
{
    using System;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using Core.Dto.UseCaseRequests;
    using Core.Interfaces.UseCases;
    using Infrastructure.Helpers;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.Request;

    [ApiController]
    [Route("api/[controller]")]
    public class MasterGraphController : ControllerBase
    {
        private readonly ICreateMasterGraphUseCase masterGraphUseCase;
        private readonly IMasterGraphDetailsUseCase masterGraphDetailsUseCase;
        private readonly IMasterGraphGetAll masterGraphGetAll;

        public MasterGraphController(ICreateMasterGraphUseCase masterGraphUseCase, IMasterGraphDetailsUseCase masterGraphDetailsUseCase,
            IMasterGraphGetAll masterGraphGetAll)
        {
            this.masterGraphUseCase = masterGraphUseCase;
            this.masterGraphDetailsUseCase = masterGraphDetailsUseCase;
            this.masterGraphGetAll = masterGraphGetAll;
        }


        [HttpPost("Create")]
        public async Task<ActionResult> Create([FromBody] CreateMasterGraphRequest request)
        {
            var req = new UcCreateMasterGraphRequest(request.Title, request.Order, request.MasterGraphCategory);
            var result = await this.masterGraphUseCase.Handle(req);
            var response = new JsonContentResult
            {
                StatusCode = (int) (result.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest),
                Content = JsonSerializer.SerializeObject(result)
            };
            return response;
        }

        [HttpGet("Details/{id}")]
        public async Task<ActionResult> Details([FromRoute] string id)
        {
            var req = new UcMasterGraphDetailsRequest(id);
            var res = await this.masterGraphDetailsUseCase.Handle(req);
            var response = new JsonContentResult
            {
                StatusCode = (int) (res.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest),
                Content = JsonSerializer.SerializeObject(res)
            };

            return response;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult> GetAll()
        {
            var res = await this.masterGraphGetAll.Handle();
            var response = new JsonContentResult
            {
                StatusCode = (int) (res.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest),
                Content = JsonSerializer.SerializeObject(res)
            };
            return response;
        }

        
        [HttpPost("CreateNational")]
        public async Task<ActionResult> CreateNational()
        {
            return this.Ok();
        }
    }
}