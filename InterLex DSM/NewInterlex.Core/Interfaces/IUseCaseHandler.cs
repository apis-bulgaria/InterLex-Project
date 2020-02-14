namespace NewInterlex.Core.Interfaces
{
    using System.Threading.Tasks;

    public interface IUseCaseHandler<TUseCaseResponse> where TUseCaseResponse : UseCaseResponseMessage
    {
        Task<TUseCaseResponse> Handle();
    }
}