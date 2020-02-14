namespace NewInterlex.Core.UseCases
{
    using System.Threading.Tasks;
    using Dto.UseCaseRequests;
    using Dto.UseCaseResponses;
    using Interfaces.Gateways.Repositories;
    using Interfaces.UseCases;

    public class SaveGraphUseCase: ISaveGraphUseCase
    {
        private readonly IMasterGraphRepository repo;


        public SaveGraphUseCase(IMasterGraphRepository repo)
        {
            this.repo = repo;
        }

        public async Task<UcSaveGraphResponse> Handle(UcSaveGraphRequest message)
        {
            var masterGraph = await this.repo.GetById(message.MasterGuid);
            var newGraphId = masterGraph.AddGraph(message);
            await this.repo.Update(masterGraph);            
            var response = new UcSaveGraphResponse(newGraphId.ToString(), true);
            return response;
        }
    }
}