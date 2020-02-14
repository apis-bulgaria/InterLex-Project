using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Interlex.Controllers
{
    using Exceptions;
    using Common;
    using Microsoft.AspNetCore.Authorization;
    using Models.RequestModels;
    using Services;

    [Route("api/[controller]")]
    // [Authorize]
    [ApiController]
    public class CaseController : ControllerBase
    {
        private readonly CaseService service;
        private readonly AknConvertService aknConvertService;

        public CaseController(CaseService service, AknConvertService aknConvertService)
        {
            this.service = service;
            this.aknConvertService = aknConvertService;
        }

        [HttpGet("GetTreeData/{id:guid?}")]
        public IActionResult GetTreeData([FromRoute] Guid? id)
        {
            var data = this.service.GetTreeModel(id);
            return this.Ok(data);
        }

        [HttpGet("GetWholeTree")]
        public async Task<IActionResult> GetWholeTree()
        {
            var tree = this.service.GetWholeTree();
            return this.Ok(tree);
        }

        [HttpGet("GetSuggestions/{code}")]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> GetSuggestions(string code)
        {
            var suggestions = await this.service.GetSuggestions(code);
            return this.Ok(suggestions);
        }

        [HttpGet("GetAllSuggestions")]
        [ResponseCache(NoStore = true)]
        [Authorize(Policy = Constants.SuperAdmin)]
        public IActionResult GetAllSuggestions()
        {
            var suggestions = this.service.GetAllSuggestions();
            return this.Ok(suggestions);
        }

        [HttpPost("DeleteSuggestion")]
        [Authorize(Policy = Constants.SuperAdmin)]
        public async Task<IActionResult> DeleteSuggestion([FromBody] SuggestionReqModel model)
        {
            try
            {
                await this.service.DeleteSuggestion(model);
            }
            catch (NotFoundException e)
            {
                return this.BadRequest(e.Message);
            }

            return this.Ok();
        }

        [HttpPost("SaveCase")]
        public async Task<IActionResult> SaveCase([FromBody] CaseModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var id = await this.service.SaveCase(model);
            return this.Ok(id);
        }

        [HttpPost("SaveExpert")]
        public async Task<IActionResult> SaveExpert([FromBody] ExpertModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var id = await this.service.SaveExpert(model);
            return this.Ok(id);
        }


        [HttpPost("SaveMetadata")]
        public async Task<IActionResult> SaveMetadata([FromBody] MetadataModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var id = await this.service.SaveMetadata(model);
            return this.Ok(id);
        }

        [HttpPost("EditCase/{id:int}")]
        public async Task<IActionResult> EditCase([FromBody] CaseModel model, [FromRoute] int id)
        {
            try
            {
                await this.service.EditCase(model, id);
            }
            catch (NotFoundException e)
            {
                return this.BadRequest(e.Message);
            }
            catch (NotAuthorizedException)
            {
                return this.Unauthorized();
            }

            return this.Ok();
        }

        [HttpPost("EditMetadata/{id:int}")]
        public async Task<IActionResult> EditMetadata([FromBody] MetadataModel model, [FromRoute] int id)
        {
            try
            {
                await this.service.EditMetadata(model, id);
            }
            catch (NotFoundException e)
            {
                return this.BadRequest(e.Message);
            }
            catch (NotAuthorizedException)
            {
                return this.Unauthorized();
            }

            return this.Ok();
        }

        [HttpPost("EditExpert/{id:int}")]
        public async Task<IActionResult> EditExpert([FromBody] ExpertModel model, [FromRoute] int id)
        {
            try
            {
                await this.service.EditExpert(model, id);
            }
            catch (NotFoundException e)
            {
                return this.BadRequest(e.Message);
            }
            catch (NotAuthorizedException)
            {
                return this.Unauthorized();
            }

            return this.Ok();
        }

        [HttpGet("GetMetaFile/{id:int}")]
        //        [AllowAnonymous]
        public async Task<IActionResult> GetMetaFile([FromRoute] int id)
        {
            var data = await this.service.GetMetaFile(id);
            this.AttachFileResultHeaders(data.Name);
            return this.File(data.Content, data.MimeType, data.Name);
        }

        [HttpGet("GetExpertFile/{id:guid}")]
        public async Task<IActionResult> GetExpertFile([FromRoute] Guid id)
        {
            var data = await this.service.GetExpertFile(id);
            this.AttachFileResultHeaders(data.Name);
            return this.File(data.Content, data.MimeType, data.Name);
        }

        [HttpGet("GetMetaTranslatedFile/{id:int}")]
        //        [AllowAnonymous]
        public async Task<IActionResult> GetMetaTranslatedFile([FromRoute] int id)
        {
            var data = await this.service.GetMetaTranslatedFile(id);
            this.AttachFileResultHeaders(data.Name);
            return this.File(data.Content, data.MimeType, data.Name);
        }

        private void AttachFileResultHeaders(string fileName)
        {
            var urlEncodedName = Uri.EscapeDataString(fileName);
            this.Response.Headers.Add("File-name", urlEncodedName);
            this.Response.Headers.Add("access-control-expose-headers",
                "File-name"); // needed to expose headers to Angular
        }

      
        [HttpPost("DeleteMeta/{id:int}")]
        public async Task<IActionResult> DeleteMeta([FromRoute] int id)
        {
            try
            {
                await this.service.DeleteMeta(id);
            }
            catch (NotFoundException ex)
            {
                return this.BadRequest(ex.Message);
            }
            catch (NotAuthorizedException)
            {
                return this.Unauthorized();
            }

            return this.Ok();
        }

        [HttpPost("DeleteExpert/{id:int}")]
        public async Task<IActionResult> DeleteExpert([FromRoute] int id)
        {
            try
            {
                await this.service.DeleteExpert(id);
            }
            catch (NotFoundException ex)
            {
                return this.BadRequest(ex.Message);
            }
            catch (NotAuthorizedException)
            {
                return this.Unauthorized();
            }

            return this.Ok();
        }

        [HttpPost("DeleteCase/{id:int}")]
        public async Task<IActionResult> DeleteCase([FromRoute] int id)
        {
            try
            {
                await this.service.DeleteCase(id);
            }
            catch (NotFoundException ex)
            {
                return this.BadRequest(ex.Message);
            }
            catch (NotAuthorizedException)
            {
                return this.Unauthorized();
            }

            return this.Ok();
        }

        [HttpPost("GetCasesList")]
        public async Task<IActionResult> GetCasesListAsync([FromBody] CaseListRequestModel request)
        {
            var data = await this.service.GetCaseList(request);
            return this.Ok(data);
        }

        [HttpPost("GetMetaList")]
        public async Task<IActionResult> GetMetaListAsync([FromBody] CaseListRequestModel request)
        {
            var data = await this.service.GetMetadataList(request);
            return this.Ok(data);
        }

        [HttpPost("GetExpertList")]
        public async Task<IActionResult> GetExpertListAsync([FromBody] CaseListRequestModel request)
        {
            var data = await this.service.GetExpertList(request);
            return this.Ok(data);
        }

        [HttpGet("GetCaseContent/{id:int}")]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> GetCaseContent(int id)
        {
            var data = await this.service.GetCaseContent(id);

            return this.Ok(data);
        }

        [HttpGet("GetMetaContent/{id:int}")]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> GetMetaContent(int id)
        {
            var data = await this.service.GetMetaContent(id);
            return this.Ok(data);
        }

        [HttpGet("GetExpertContent/{id:int}")]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> GetExpertContent(int id)
        {
            var data = await this.service.GetExpertContent(id);
            return this.Ok(data);
        }

        [HttpGet("GetCaseHtmlContent/{id:int}")]
        public async Task<IActionResult> GetCaseHtmlContent(int id)
        {
            var jsonContent = (await this.service.GetCaseContent(id)).Content;
            var documentType = 1;

            var html = await this.aknConvertService.ConvertToHtmlAsync(documentType, jsonContent);

            return this.Ok(html);
        }

        [HttpGet("GetMetaHtmlContent/{id:int}")]
        public async Task<IActionResult> GetMetaHtmlContent(int id)
        {
            var jsonContent = (await this.service.GetMetaContent(id)).Content;
            var documentType = 2;

            var html = await this.aknConvertService.ConvertToHtmlAsync(documentType, jsonContent);

            return this.Ok(html);
        }
    }
}