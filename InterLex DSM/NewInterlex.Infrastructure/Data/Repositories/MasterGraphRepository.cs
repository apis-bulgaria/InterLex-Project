namespace NewInterlex.Infrastructure.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Dto;
    using Core.Entities;
    using Core.Enumerations;
    using Core.Interfaces.Gateways.Repositories;
    using Microsoft.EntityFrameworkCore;

    internal sealed class MasterGraphRepository : GuidEfRepository<MasterGraph>, IMasterGraphRepository
    {
        public MasterGraphRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<MasterGraph> Create(string title, int order, MasterGraphCategories masterGraphCategory)
        {
            var props = new CommonGraphProp
            {
                EnText = title
            };
            var graph = new MasterGraph
            {
                Id = Guid.NewGuid(),
                Order = order,
                MasterGraphCategoryId = (int) masterGraphCategory,
                Props = props
            };
            var coreGraph = new Graph
            {
                Id = Guid.NewGuid(),
                Title = "Core Graph",
                GraphTypeId = (int) GraphTypes.Core,
                MasterGraphId = graph.Id,
                Data = "" // this might need changing
            };
            graph.Graphs.Add(coreGraph);
            this.dbContext.MasterGraphs.Add(graph);
            await this.dbContext.SaveInfoAndChangesAsync();
            return graph;
        }

        public async Task<MasterGraphDetails> GetDetailInfo(Guid id)
        {
            var result = await this.dbContext.MasterGraphs.AsNoTracking().Include(x => x.Props).Include(x => x.Graphs)
                .Where(x => x.Id == id)
                .Select(x => new MasterGraphDetails
                {
                    Title = x.Props.EnText,
                    Graphs = x.Graphs.Select(g => new GraphListModel
                    {
                        Id = g.Id,
                        Title = g.Title,
                        Type = (GraphTypes) g.GraphTypeId // maybe return the name???
                    })
                }).FirstOrDefaultAsync();
            return result;
        }

        public async Task<IReadOnlyList<MasterGraphListModel>> GetListWithNames()
        {
            var result = await this.dbContext.MasterGraphs.AsNoTracking().Include(x => x.Props)
                .Select(x => new MasterGraphListModel
                {
                    Id = x.Id,
                    Order = x.Order,
                    Title = x.Props.EnText,
                    MasterGraphCategory = x.MasterGraphCategoryId
                }).ToListAsync();
            return result;
        }
    }
}