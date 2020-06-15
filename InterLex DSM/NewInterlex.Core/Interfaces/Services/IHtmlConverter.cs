namespace NewInterlex.Core.Interfaces.Services
{
    using System.Threading.Tasks;
    using Enumerations;

    public interface IHtmlConverter
    {
        Task<byte[]> ConvertHtml(string html, HtmlExportTypes type);
    }
}