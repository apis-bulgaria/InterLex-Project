using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlex.BusinessLayer.Entities
{
    public class Classifier : IClassifier
    {
        private List<FolderData> _lFolderData;

        public List<FolderData> Get()
        {
            return _lFolderData;
        }

        public Classifier()
        {
            _lFolderData = new List<FolderData>();


            //_lFolderData.Add(new FolderData() { id = "1", key = "1", title = "POLITICS", folder = true, lazy = false });
            //_lFolderData[0].children.Add(new FolderData() { id = "2", key = "1.1", title = "consumer movement", folder = true, lazy = false });

            //_lFolderData.Add(new FolderData() { id = "3", key = "2", title = "LAW", folder = true, lazy = false });
            //_lFolderData[1].children.Add(new FolderData() { id = "4", key = "2.1", title = "consumer movement", folder = true, lazy = false });

            //_lFolderData.Add(new FolderData() { id = "43", key = "3", title = "ECONOMICS", folder = true, lazy = false });
            //_lFolderData[2].children.Add(new FolderData() { id = "5", key = "3.1", title = "consumer behaviour", folder = true, lazy = false });
            //_lFolderData[2].children.Add(new FolderData() { id = "6", key = "3.2", title = "consumer survey", folder = true, lazy = false });

            //_lFolderData.Add(new FolderData() { id = "7", key = "4", title = "TRADE", folder = true, lazy = false });
            //_lFolderData[3].children.Add(new FolderData { id = "8", key = "4.1", title = "consumer ", folder = true, lazy = false });
            //_lFolderData[3].children.Add(new FolderData { id = "9", key = "4.1", title = "consumer behaviour ", folder = true, lazy = false });
            //_lFolderData[3].children.Add(new FolderData { id = "10", key = "4.1", title = "consumer motivation", folder = true, lazy = false });
            //_lFolderData[3].children.Add(new FolderData { id = "11", key = "4.1", title = "consumer survey", folder = true, lazy = false });
            //_lFolderData[3].children.Add(new FolderData { id = "12", key = "4.1", title = "consumer policy", folder = true, lazy = false });
            //_lFolderData[3].children.Add(new FolderData { id = "13", key = "4.1", title = "consumer protection", folder = true, lazy = false });
            //_lFolderData[3].children.Add(new FolderData { id = "14", key = "4.1", title = "consumer information", folder = true, lazy = false });
            //_lFolderData[3].children.Add(new FolderData { id = "15", key = "4.1", title = "consumer movement", folder = true, lazy = false });
            //_lFolderData[3].children.Add(new FolderData { id = "16", key = "4.1", title = "consumer credit", folder = true, lazy = false });
            //_lFolderData[3].children.Add(new FolderData { id = "17", key = "4.1", title = "consumer demand", folder = true, lazy = false });
            //_lFolderData[3].children.Add(new FolderData { id = "18", key = "4.1", title = "cosnumer society", folder = true, lazy = false });
            //_lFolderData[3].children.Add(new FolderData { id = "19", key = "4.1", title = "consumer goods", folder = true, lazy = false });
            //_lFolderData[3].children.Add(new FolderData { id = "20", key = "4.1", title = "consumer cooperative", folder = true, lazy = false });
            //_lFolderData[3].children.Add(new FolderData { id = "21", key = "4.1", title = "consumer association", folder = true, lazy = false });
            //_lFolderData[3].children.Add(new FolderData { id = "22", key = "4.1", title = "consumer durables", folder = true, lazy = false });
            //_lFolderData[3].children.Add(new FolderData { id = "23", key = "4.1", title = "consumer education", folder = true, lazy = false });
            //_lFolderData[3].children.Add(new FolderData { id = "24", key = "4.1", title = "consumer organisation", folder = true, lazy = false });
            //_lFolderData[3].children.Add(new FolderData { id = "25", key = "4.1", title = "consumer policy", folder = true, lazy = false });
            //_lFolderData[3].children.Add(new FolderData { id = "26", key = "4.1", title = "consumer policy action plan", folder = true, lazy = false });
            //_lFolderData[3].children.Add(new FolderData { id = "27", key = "4.1", title = "consumerism", folder = true, lazy = false });
            //_lFolderData[3].children.Add(new FolderData { id = "28", key = "4.1", title = "consumers`rights", folder = true, lazy = false });
            //_lFolderData[3].children.Add(new FolderData { id = "29", key = "4.1", title = "policy of consumerism", folder = true, lazy = false });

            //_lFolderData.Add(new FolderData() { id = "30", key = "5", title = "FINANCE", folder = true, lazy = false });
            //_lFolderData[4].children.Add(new FolderData { id = "31", key = "5.1", title = "consumer credit", folder = true, lazy = false });
            //_lFolderData[4].children.Add(new FolderData { id = "32", key = "5.2", title = "consumer price", folder = true, lazy = false });
            //_lFolderData[4].children.Add(new FolderData { id = "33", key = "5.3", title = "consumer protection", folder = true, lazy = false });
            

            //_lFolderData.Add(new FolderData() { id = "34", key = "6", title = "SOCIAL QUESTIONS", folder = true, lazy = false });
            //_lFolderData[5].children.Add(new FolderData { id = "35", key = "6.1", title = "consumer protection", folder = true, lazy = false });

            //_lFolderData.Add(new FolderData() { id = "36", key = "7", title = "BUSINESS", folder = true, lazy = false });
            //_lFolderData[6].children.Add(new FolderData { id = "37", key = "7.1", title = "consumer information", folder = true, lazy = false });
            //_lFolderData[6].children.Add(new FolderData { id = "38", key = "7.2", title = "consumer cooperative", folder = true, lazy = false });

            //_lFolderData.Add(new FolderData() { id = "39", key = "8", title = "ENVIRONMENT", folder = true, lazy = false });
            //_lFolderData[7].children.Add(new FolderData { id = "40", key = "8.1", title = "consumer protection", folder = true, lazy = false });

            //_lFolderData.Add(new FolderData() { id = "41", key = "9", title = "INTERNATIONAL ORGANISATIONS", folder = true, lazy = false });
            //_lFolderData[8].children.Add(new FolderData { id = "42", key = "9.1", title = "consumer", folder = true, lazy = false });
        }

        
    }
}
