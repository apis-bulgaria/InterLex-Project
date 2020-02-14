namespace NewInterlex.Core.Interfaces.Services
{
    using System.Threading.Tasks;

    public interface ILinkInsertService
    {
        Task<string> InsertLinks(string jsonArray);
    }
}