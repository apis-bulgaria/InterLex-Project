namespace NewInterlex.Core.Interfaces
{
    using System.Threading.Tasks;

    public interface IUseCaseRequestHandler<in TUseCaseRequest, TUseCaseResponse> where TUseCaseRequest: IUseCaseRequest<TUseCaseResponse> where TUseCaseResponse: UseCaseResponseMessage
    {
        Task<TUseCaseResponse> Handle(TUseCaseRequest message);
    }
}