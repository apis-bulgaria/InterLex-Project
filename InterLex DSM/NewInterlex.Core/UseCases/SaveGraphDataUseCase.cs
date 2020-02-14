namespace NewInterlex.Core.UseCases
{
    using System.Threading.Tasks;
    using Dto.UseCaseRequests;
    using Dto.UseCaseResponses;
    using Interfaces.Gateways.Repositories;
    using Interfaces.UseCases;

    public class SaveGraphDataUseCase : ISaveGraphDataUseCase
    {
        private readonly IGraphRepository repo;

        public SaveGraphDataUseCase(IGraphRepository repo)
        {
            this.repo = repo;
        }

        public async Task<UcSaveGraphDataResponse> Handle(UcSaveGraphDataRequest message)
        {
            var entity = await this.repo.GetById(message.Id);
            entity.Data = message.Content;
            await this.repo.Update(entity);
            var response = new UcSaveGraphDataResponse(true);
            return response;
        }
    }
}