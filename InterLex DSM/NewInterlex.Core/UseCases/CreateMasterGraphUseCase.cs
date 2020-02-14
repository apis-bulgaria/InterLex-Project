namespace NewInterlex.Core.UseCases
{
    using System.Threading.Tasks;
    using Dto.UseCaseRequests;
    using Dto.UseCaseResponses;
    using Interfaces.Gateways.Repositories;
    using Interfaces.UseCases;

    public class CreateMasterGraphUseCase : ICreateMasterGraphUseCase
    {
        private readonly IMasterGraphRepository repo;

        public CreateMasterGraphUseCase(IMasterGraphRepository repo)
        {
            this.repo = repo;
        }
        
        public async Task<UcCreateMasterGraphResponse> Handle(UcCreateMasterGraphRequest message)
        {
            var result = await this.repo.Create(message.Title, message.Order, message.MasterGraphCategory);
            var response = new UcCreateMasterGraphResponse(result.Id.ToString(), true);
            return response;
        }
    }
}