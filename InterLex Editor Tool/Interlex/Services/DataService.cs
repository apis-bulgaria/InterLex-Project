namespace Interlex.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Data;
    using Models.ResponseModels;

    public class DataService
    {
        private readonly AppDbContext context; // use repo

        public DataService(AppDbContext context)
        {
            this.context = context;
        }

        public List<LanguageModel> GetEuLanguages()
        {
            var res = this.context.Languages.Select(x => new LanguageModel
            {
                Id = x.Id,
                TwoLetter = x.TwoLetter,
                NameEn = x.NameEn,
                Name = x.Name
            }).ToList();
            return res;
        }
    }
}