namespace NewInterlex.Core.UseCases
{
    using System;
    using System.Threading.Tasks;
    using Dto.UseCaseRequests;
    using Dto.UseCaseResponses;
    using Interfaces.Gateways.Repositories;
    using Interfaces.UseCases;

    public class MasterGraphDetailsUseCase : IMasterGraphDetailsUseCase
    {
        private readonly IMasterGraphRepository repo;

        public MasterGraphDetailsUseCase(IMasterGraphRepository repo)
        {
            this.repo = repo;
        }

        public async Task<UcMasterGraphDetailsResponse> Handle(UcMasterGraphDetailsRequest message)
        {
            var guid = new Guid(message.Id);
            var repoResult = await this.repo.GetDetailInfo(guid);
            var response = new UcMasterGraphDetailsResponse(repoResult.Title, repoResult.Graphs, true);
            return response;

        }
    }
}