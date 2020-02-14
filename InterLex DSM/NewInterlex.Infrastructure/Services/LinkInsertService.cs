namespace NewInterlex.Infrastructure.Services
{
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Core.Dto;
    using Core.Interfaces.Services;
    using Helpers;

    internal sealed class LinkInsertService : ILinkInsertService
    {
        private static readonly string url = "http://techno.eucases.eu:8338/api/addinslink/PutInterlexDSMLinks";

        public async Task<string> InsertLinks(string json)
        {
            var client = new HttpClient();
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var res = await client.PostAsync(url, content);
            var responseContent = await res.Content.ReadAsStringAsync();
            return responseContent;

        }
    }
}