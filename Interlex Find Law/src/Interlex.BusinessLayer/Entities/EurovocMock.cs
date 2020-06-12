using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EUCases.ConsumerCasesWebApp.BusinessLayer.Entities
{
    public class EurovocMock : IEurovoc
    {
        private List<FolderData> _lFolderData;

        public EurovocMock()
        {
            _lFolderData = new List<FolderData>();
            _lFolderData.Add(new FolderData() { Id = "1", Key = "1", Title = "POLITICS", Folder = true, Lazy = false });
            _lFolderData[0].Children.Add(new FolderData() { Id = "2", Key = "1.1", Title = "consumer movement", Folder = true, Lazy = false });

            _lFolderData.Add(new FolderData() { Id = "3", Key = "2", Title = "LAW", Folder = true, Lazy = false });
            _lFolderData[1].Children.Add(new FolderData() { Id = "4", Key = "2.1", Title = "consumer movement", Folder = true, Lazy = false });

            _lFolderData.Add(new FolderData() { Id = "43", Key = "3", Title = "ECONOMICS", Folder = true, Lazy = false });
            _lFolderData[2].Children.Add(new FolderData() { Id = "5", Key = "3.1", Title = "consumer behaviour", Folder = true, Lazy = false });
            _lFolderData[2].Children.Add(new FolderData() { Id = "6", Key = "3.2", Title = "consumer survey", Folder = true, Lazy = false });

            _lFolderData.Add(new FolderData() { Id = "7", Key = "4", Title = "TRADE", Folder = true, Lazy = false });
            _lFolderData[3].Children.Add(new FolderData { Id = "8", Key = "4.1", Title = "consumer ", Folder = true, Lazy = false });
            _lFolderData[3].Children.Add(new FolderData { Id = "9", Key = "4.1", Title = "consumer behaviour ", Folder = true, Lazy = false });
            _lFolderData[3].Children.Add(new FolderData { Id = "10", Key = "4.1", Title = "consumer motivation", Folder = true, Lazy = false });
            _lFolderData[3].Children.Add(new FolderData { Id = "11", Key = "4.1", Title = "consumer survey", Folder = true, Lazy = false });
            _lFolderData[3].Children.Add(new FolderData { Id = "12", Key = "4.1", Title = "consumer policy", Folder = true, Lazy = false });
            _lFolderData[3].Children.Add(new FolderData { Id = "13", Key = "4.1", Title = "consumer protection", Folder = true, Lazy = false });
            _lFolderData[3].Children.Add(new FolderData { Id = "14", Key = "4.1", Title = "consumer information", Folder = true, Lazy = false });
            _lFolderData[3].Children.Add(new FolderData { Id = "15", Key = "4.1", Title = "consumer movement", Folder = true, Lazy = false });
            _lFolderData[3].Children.Add(new FolderData { Id = "16", Key = "4.1", Title = "consumer credit", Folder = true, Lazy = false });
            _lFolderData[3].Children.Add(new FolderData { Id = "17", Key = "4.1", Title = "consumer demand", Folder = true, Lazy = false });
            _lFolderData[3].Children.Add(new FolderData { Id = "18", Key = "4.1", Title = "cosnumer society", Folder = true, Lazy = false });
            _lFolderData[3].Children.Add(new FolderData { Id = "19", Key = "4.1", Title = "consumer goods", Folder = true, Lazy = false });
            _lFolderData[3].Children.Add(new FolderData { Id = "20", Key = "4.1", Title = "consumer cooperative", Folder = true, Lazy = false });
            _lFolderData[3].Children.Add(new FolderData { Id = "21", Key = "4.1", Title = "consumer association", Folder = true, Lazy = false });
            _lFolderData[3].Children.Add(new FolderData { Id = "22", Key = "4.1", Title = "consumer durables", Folder = true, Lazy = false });
            _lFolderData[3].Children.Add(new FolderData { Id = "23", Key = "4.1", Title = "consumer education", Folder = true, Lazy = false });
            _lFolderData[3].Children.Add(new FolderData { Id = "24", Key = "4.1", Title = "consumer organisation", Folder = true, Lazy = false });
            _lFolderData[3].Children.Add(new FolderData { Id = "25", Key = "4.1", Title = "consumer policy", Folder = true, Lazy = false });
            _lFolderData[3].Children.Add(new FolderData { Id = "26", Key = "4.1", Title = "consumer policy action plan", Folder = true, Lazy = false });
            _lFolderData[3].Children.Add(new FolderData { Id = "27", Key = "4.1", Title = "consumerism", Folder = true, Lazy = false });
            _lFolderData[3].Children.Add(new FolderData { Id = "28", Key = "4.1", Title = "consumers`rights", Folder = true, Lazy = false });
            _lFolderData[3].Children.Add(new FolderData { Id = "29", Key = "4.1", Title = "policy of consumerism", Folder = true, Lazy = false });

            _lFolderData.Add(new FolderData() { Id = "30", Key = "5", Title = "FINANCE", Folder = true, Lazy = false });
            _lFolderData[4].Children.Add(new FolderData { Id = "31", Key = "5.1", Title = "consumer credit", Folder = true, Lazy = false });
            _lFolderData[4].Children.Add(new FolderData { Id = "32", Key = "5.2", Title = "consumer price", Folder = true, Lazy = false });
            _lFolderData[4].Children.Add(new FolderData { Id = "33", Key = "5.3", Title = "consumer protection", Folder = true, Lazy = false });
            

            _lFolderData.Add(new FolderData() { Id = "34", Key = "6", Title = "SOCIAL QUESTIONS", Folder = true, Lazy = false });
            _lFolderData[5].Children.Add(new FolderData { Id = "35", Key = "6.1", Title = "consumer protection", Folder = true, Lazy = false });

            _lFolderData.Add(new FolderData() { Id = "36", Key = "7", Title = "BUSINESS", Folder = true, Lazy = false });
            _lFolderData[6].Children.Add(new FolderData { Id = "37", Key = "7.1", Title = "consumer information", Folder = true, Lazy = false });
            _lFolderData[6].Children.Add(new FolderData { Id = "38", Key = "7.2", Title = "consumer cooperative", Folder = true, Lazy = false });

            _lFolderData.Add(new FolderData() { Id = "39", Key = "8", Title = "ENVIRONMENT", Folder = true, Lazy = false });
            _lFolderData[7].Children.Add(new FolderData { Id = "40", Key = "8.1", Title = "consumer protection", Folder = true, Lazy = false });

            _lFolderData.Add(new FolderData() { Id = "41", Key = "9", Title = "INTERNATIONAL ORGANISATIONS", Folder = true, Lazy = false });
            _lFolderData[8].Children.Add(new FolderData { Id = "42", Key = "9.1", Title = "consumer", Folder = true, Lazy = false });
        }

        public List<FolderData> Get()
        {
            return _lFolderData;
        }
    }
}
