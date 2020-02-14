namespace NewInterlex.Core.UseCases
{
    using System.Threading.Tasks;
    using Dto.UseCaseResponses;
    using Interfaces.Gateways.Repositories;
    using Interfaces.UseCases;

    public class MasterGraphGetAllUseCase : IMasterGraphGetAll
    {
        private readonly IMasterGraphRepository repo;

        public MasterGraphGetAllUseCase(IMasterGraphRepository repo)
        {
            this.repo = repo;
        }

        public async Task<UcMasterGraphGetAllResponse> Handle()
        {
            var result = await this.repo.GetListWithNames();
            var response = new UcMasterGraphGetAllResponse(result, true);
            return response;
        }
    }
}