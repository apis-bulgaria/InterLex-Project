namespace NewInterlex.Core.UseCases
{
    using System.Threading.Tasks;
    using Dto.UseCaseRequests;
    using Dto.UseCaseResponses;
    using Interfaces.Gateways.Repositories;
    using Interfaces.UseCases;

    public class GetGraphUseCase : IGetGraphUseCase
    {
        private readonly IGraphRepository repo;

        public GetGraphUseCase(IGraphRepository repo)
        {
            this.repo = repo;
        }
        
        
        
        public async Task<UcGetGraphResponse> Handle(UcGetGraphRequest message)
        {
            var entity = await this.repo.GetById(message.GraphId);
            UcGetGraphResponse response;
            if (entity != null)
            {
                response = new UcGetGraphResponse(entity.Title, entity.Data, true);
                
            }
            else
            {
                response = new UcGetGraphResponse(false, "Graph not found");
            }
            
            return response;
        }
    }
}