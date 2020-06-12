using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EUCases.ConsumerCasesWebApp.BusinessLayer.Entities
{
    public class EULegislationMock: IEULegislation
    {
        private List<FolderData> _lFolderData;

        public EULegislationMock()
        {
            _lFolderData = new List<FolderData>();
            _lFolderData.Add(new FolderData() { Id = "1", Key = "", Title = "Treaties", Folder = true, Lazy = false });
            _lFolderData[0].Children.Add(new FolderData() { Id = "2", Key = "", Title = "Treaty on the Functioning of the EU", Folder = true, Lazy = false });
            _lFolderData[0].Children[0].Children.Add(new FolderData() { Id = "3", Key = "", Title = "Article 1", Folder = true, Lazy = false });
            _lFolderData[0].Children[0].Children.Add(new FolderData() { Id = "4", Key = "", Title = "Article 5", Folder = true, Lazy = false });
            _lFolderData[0].Children.Add(new FolderData() { Id = "5", Key = "", Title = "Treaty on EU", Folder = true, Lazy = false });
            _lFolderData[0].Children[1].Children.Add(new FolderData() { Id = "6", Key = "", Title = "Article 3", Folder = true, Lazy = false });
            _lFolderData[0].Children[1].Children.Add(new FolderData() { Id = "7", Key = "", Title = "Article 15", Folder = true, Lazy = false });


            _lFolderData.Add(new FolderData() { Id = "8", Key = "", Title = "Regulations", Folder = true, Lazy = false });
            _lFolderData[1].Children.Add(new FolderData() { Id = "9", Key = "", Title = "Regulation 44/2001", Folder = true, Lazy = false });
            _lFolderData[1].Children[0].Children.Add(new FolderData() { Id = "10", Key = "", Title = "Article 7", Folder = true, Lazy = false });
            _lFolderData[1].Children[0].Children.Add(new FolderData() { Id = "11", Key = "", Title = "Article 12", Folder = true, Lazy = false });
            _lFolderData[1].Children.Add(new FolderData() { Id = "12", Key = "", Title = "Regulation 2100/1994", Folder = true, Lazy = false });
            _lFolderData[1].Children[1].Children.Add(new FolderData() { Id = "13", Key = "", Title = "Article 2", Folder = true, Lazy = false });
            _lFolderData[1].Children[1].Children.Add(new FolderData() { Id = "14", Key = "", Title = "Article 3", Folder = true, Lazy = false });

            _lFolderData.Add(new FolderData() { Id = "15", Key = "", Title = "Directives", Folder = true, Lazy = false });
            _lFolderData[2].Children.Add(new FolderData() { Id = "16", Key = "", Title = "Directive 87/54/EEC", Folder = true, Lazy = false });
            _lFolderData[2].Children[0].Children.Add(new FolderData() { Id = "17", Key = "", Title = "Article 3", Folder = true, Lazy = false });
            _lFolderData[2].Children[0].Children.Add(new FolderData() { Id = "18", Key = "", Title = "Article 15", Folder = true, Lazy = false });
            _lFolderData[2].Children.Add(new FolderData() { Id = "19", Key = "", Title = "Directive 96/9/EC", Folder = true, Lazy = false });
            _lFolderData[2].Children[1].Children.Add(new FolderData() { Id = "20", Key = "", Title = "Article 17", Folder = true, Lazy = false });
            _lFolderData[2].Children[1].Children.Add(new FolderData() { Id = "21", Key = "", Title = "Article 21", Folder = true, Lazy = false });

            _lFolderData.Add(new FolderData() { Id = "22", Key = "", Title = "Decisions", Folder = true, Lazy = false });
            _lFolderData[3].Children.Add(new FolderData() { Id = "23", Key = "", Title = "Decision 89/196/EEC", Folder = true, Lazy = false });
            _lFolderData[3].Children.Add(new FolderData() { Id = "24", Key = "", Title = "Decision 93/16/EEC", Folder = true, Lazy = false });
        }

        public List<FolderData> Get()
        {
            return _lFolderData;
        }
    }
}
