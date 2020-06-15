namespace NewInterlex.Infrastructure.Services
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Apis.WordConvert.Model;
    using Apis.WordConvert.RESTClient;
    using Core.Enumerations;
    using Core.Interfaces.Services;

    public class HtmlConverter : IHtmlConverter
    {
        public async Task<byte[]> ConvertHtml(string html, HtmlExportTypes type)
        {
            var bytes = Encoding.UTF8.GetBytes(html);
            var client = new WordConvertRESTClient();
            var request = new WordConvertRequestModel
            {
                Data = bytes,
                ResultFormat = type == HtmlExportTypes.Pdf ? WordConvertType.Pdf : WordConvertType.Rtf,
                SourceFormat = ".html",
                Orientation = WordConvertOrientationType.Portrait,
                PreserveCSSRules = true
            };
            var result = await client.ConvertAsync(request, opt => opt.Timeout = TimeSpan.FromMinutes(2));
            return result.Data;
        }
    }
}