namespace NewInterlex.Core.UseCases
{
    using System.Threading.Tasks;
    using Dto.UseCaseResponses;
    using Interfaces.Gateways.Repositories;
    using Interfaces.UseCases;

    public class GetMetaInfoUseCase : IGetMetaInfoUseCase
    {
        private readonly ILanguageRepository languageRepository;
        private readonly ILinkTypeRepository linkTypeRepository;
        private readonly IGraphConnectionTypeRepository graphConnectionTypeRepository;

        public GetMetaInfoUseCase(ILanguageRepository languageRepository, ILinkTypeRepository linkTypeRepository,
            IGraphConnectionTypeRepository graphConnectionTypeRepository)
        {
            this.languageRepository = languageRepository;
            this.linkTypeRepository = linkTypeRepository;
            this.graphConnectionTypeRepository = graphConnectionTypeRepository;
        }

        public async Task<UcGetMetaInfoResponse> Handle()
        {
            var languages = await this.languageRepository.GetAll();
            var linkTypes = await this.linkTypeRepository.GetAll();
            var graphConnectionTypes = await this.graphConnectionTypeRepository.GetAll();
            var response = new UcGetMetaInfoResponse(graphConnectionTypes, languages, linkTypes, true);
            return response;
        }
    }
}