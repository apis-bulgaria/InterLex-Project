namespace NewInterlex.Api.Controllers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Core.Dto.UseCaseRequests;
    using Core.Enumerations;
    using Core.Interfaces.UseCases;
    using Infrastructure.Helpers;
    using Infrastructure.Identity;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.Request;


    [ApiController]
    [Route("api/[controller]")]
    public class GraphController : ControllerBase
    {
        private readonly IGetMetaInfoUseCase getMetaInfoUseCase;
        private readonly ISaveGraphUseCase saveGraphUseCase;
        private readonly IGetGraphUseCase getGraphUseCase;
        private readonly ISaveGraphDataUseCase saveGraphDataUseCase;
        private readonly IInsertLinksUseCase insertLinksUseCase;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IExportReportUseCase exportReportUseCase;


        public GraphController(IGetMetaInfoUseCase getMetaInfoUseCase, ISaveGraphUseCase saveGraphUseCase,
            IGetGraphUseCase getGraphUseCase, ISaveGraphDataUseCase saveGraphDataUseCase,
            IInsertLinksUseCase insertLinksUseCase,
            UserManager<ApplicationUser> userManager, IExportReportUseCase exportReportUseCase)
        {
            this.getMetaInfoUseCase = getMetaInfoUseCase;
            this.saveGraphUseCase = saveGraphUseCase;
            this.getGraphUseCase = getGraphUseCase;
            this.saveGraphDataUseCase = saveGraphDataUseCase;
            this.insertLinksUseCase = insertLinksUseCase;
            this.userManager = userManager;
            this.exportReportUseCase = exportReportUseCase;
        }


        [HttpGet("GetMeta")]
        public async Task<ActionResult> GetMeta()
        {
            var result = await this.getMetaInfoUseCase.Handle();
            return this.Ok(result);
        }

        [HttpPost("SaveGraph")] // master graph guid as route param?
        public async Task<ActionResult> SaveGraph([FromBody] SaveGraphRequest request)
        {
            var masterGuid = Guid.NewGuid();
            var ucRequest = new UcSaveGraphRequest(request.Content, request.Title, request.GraphType, masterGuid);
            var result = await this.saveGraphUseCase.Handle(ucRequest);
            var response = new JsonContentResult
            {
                StatusCode = (int?) (result.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest),
                Content = JsonSerializer.SerializeObject(result)
            };
            return response;
        }

        [HttpPost("ExportReportpdf")]
        public async Task<ActionResult> ExportReportpdf([FromBody] ReportRequest[] request)
        {
            var ucRequest = new UcExportReportRequestComposite
            {
                Reports = request.Select(r =>
                {
                    var concl = r.Conclusion;
                    var pairs = r.Pairs;
                    var req = new UcExportReportRequest
                    {
                        Conclusion = new Conclusion(concl.Title, concl.ReportDisplay, concl.LegalBasis),
                        Pairs = pairs.Select(x => new ReportPair(x.Index, x.Question, x.Answer)).ToArray(),
                        ConclusionTranslation = r.ConclusionTranslation,
                        AboutCaseTranslation = r.AboutCaseTranslation,
                        CaseReportTranslation = r.CaseReportTranslation,
                        LegalBasisTranslation = r.LegalBasisTranslation
                    };
                    return req;
                }).ToArray(),
                ExportType = HtmlExportTypes.Pdf,
            };
            var result = await this.exportReportUseCase.Handle(ucRequest);
            return this.File(result.FileContent, "application/pdf");
        }

        [HttpPost("ExportReportrtf")]
        public async Task<ActionResult> ExportReportrtf([FromBody] ReportRequest[] request)
        {
            var ucRequest = new UcExportReportRequestComposite
            {
                Reports = request.Select(r =>
                {
                    var concl = r.Conclusion;
                    var pairs = r.Pairs;
                    var req = new UcExportReportRequest
                    {
                        Conclusion = new Conclusion(concl.Title, concl.ReportDisplay, concl.LegalBasis),
                        Pairs = pairs.Select(x => new ReportPair(x.Index, x.Question, x.Answer)).ToArray(),
                        ConclusionTranslation = r.ConclusionTranslation,
                        AboutCaseTranslation = r.AboutCaseTranslation,
                        CaseReportTranslation = r.CaseReportTranslation,
                        LegalBasisTranslation = r.LegalBasisTranslation
                    };
                    return req;
                }).ToArray(),
                ExportType = HtmlExportTypes.Rtf,
            };
            var result = await this.exportReportUseCase.Handle(ucRequest);
            return this.File(result.FileContent, "application/rtf");
        }

        [HttpPost("SaveGraphData/{id}")]
        public async Task<ActionResult> SaveGraphData([FromRoute] Guid id, [FromBody] SaveGraphDataRequest request)
        {
            var ucRequest = new UcSaveGraphDataRequest(id, request.Content);
            var result = await this.saveGraphDataUseCase.Handle(ucRequest);
            var response = new JsonContentResult
            {
                StatusCode = (int?) (result.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest),
            };
            return response;
        }

        [HttpPost("InsertLinks/{id}")]
        public async Task<ActionResult> InsertLinks([FromRoute] Guid id
//            , [FromBody] SaveGraphDataRequest request
        )
        {
            string content;
            using (var reader = new StreamReader(this.Request.Body))
            {
                content = await reader.ReadToEndAsync();
            }

            var ucRequest = new UcInsertLinksRequest(id, content);
            var result = await this.insertLinksUseCase.Handle(ucRequest);
            var response = new JsonContentResult
            {
                StatusCode = (int?) (result.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest),
                Content = result.ContentJson
            };
            return response;
        }

        [HttpGet("GetGraph/{guid}")]
        public async Task<ActionResult> GetGraph([FromRoute] Guid guid)
        {
            var ucRequest = new UcGetGraphRequest(guid);
            var result = await this.getGraphUseCase.Handle(ucRequest);
            var response = new JsonContentResult
            {
                StatusCode = (int?) (result.Success ? HttpStatusCode.OK : HttpStatusCode.NotFound),
                Content = JsonSerializer.SerializeObject(result)
            };
            return response;
        }
    }
}