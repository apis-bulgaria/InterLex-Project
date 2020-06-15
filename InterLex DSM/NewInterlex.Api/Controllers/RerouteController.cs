namespace NewInterlex.Api.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]")]
    public class RerouteController : ControllerBase
    {
        private readonly IHttpClientFactory clientFactory;

        public RerouteController(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        // here is the connection with the Reasoner Module implemented by the Italian partners
        [HttpGet("GetPrologJson")]
        public async Task<ActionResult> GetPrologJson([FromQuery(Name = "id")] HashSet<string> ids)
        {
            var client = this.clientFactory.CreateClient();
            var workingNodeIds = new HashSet<string>
            {
                "120bbee1-f97b-4ae5-989e-7dd238fe83fe",
                "0e15fdd5-04e6-4ccc-9a29-a243a896a7af",
                "30c7b463-6099-4dd7-92e8-f560ee09139d",
                "0e15fdd5-04e6-4ccc-9a29-a243a896a7af",
                "8bf8849c-b71b-4be6-a327-144c83486440"
            };
            workingNodeIds.IntersectWith(ids);
            if (workingNodeIds.Count > 0)
            {
                var idQuery = string.Join('&', workingNodeIds.Select(x => "id=" + x));
                var jurUrl = "http://interlex-reasoner.eastus.cloudapp.azure.com/interLex/jurisdiction?" + idQuery;
                var lawUrl = "http://interlex-reasoner.eastus.cloudapp.azure.com/interLex/law?" + idQuery;
                var jurTask = client.GetAsync(jurUrl);
                var lawTask = client.GetAsync(lawUrl);
                await Task.WhenAll(jurTask, lawTask);
                var jurRes = await jurTask.Result.Content.ReadAsStringAsync();
                var lawRes = await lawTask.Result.Content.ReadAsStringAsync();
                return this.Ok(new {jur = jurRes, law = lawRes});
            }
            
            return this.Ok();
        }
    }
}