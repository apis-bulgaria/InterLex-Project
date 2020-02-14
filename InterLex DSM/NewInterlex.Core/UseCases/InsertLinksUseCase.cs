namespace NewInterlex.Core.UseCases
{
    using System.Threading.Tasks;
    using Dto.UseCaseRequests;
    using Dto.UseCaseResponses;
    using Interfaces.Services;
    using Interfaces.UseCases;

    public class InsertLinksUseCase : IInsertLinksUseCase
    {
        private readonly ILinkInsertService linkInserter;


        public InsertLinksUseCase(ILinkInsertService linkInserter)
        {
            this.linkInserter = linkInserter;
        }

        public async Task<UcInsertLinksResponse> Handle(UcInsertLinksRequest message)
        {
            var res = await this.linkInserter.InsertLinks(message.Content);
            

            var resp = new UcInsertLinksResponse(res,true);
            return resp;
        }
    }
}