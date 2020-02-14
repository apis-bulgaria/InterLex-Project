namespace NewInterlex.Core.Interfaces.UseCases
{
    using Dto.UseCaseRequests;
    using Dto.UseCaseResponses;

    public interface ISaveGraphDataUseCase : IUseCaseRequestHandler<UcSaveGraphDataRequest, UcSaveGraphDataResponse>
    {
        
    }
}