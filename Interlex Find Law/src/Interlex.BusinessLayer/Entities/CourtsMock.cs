using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EUCases.ConsumerCasesWebApp.BusinessLayer.Entities
{
    public class CourtsMock : ICourts
    {
        private List<FolderData> _lFolderData;

        public CourtsMock()
        {
            _lFolderData = new List<FolderData>();

            _lFolderData.Add(new FolderData() { Id = "1", Key = "1", Title = "Austria", Folder = true, Lazy = false });
            _lFolderData[0].Children.Add(new FolderData() { Id = "2", Key = "1.1", Title = "Verfassungsgerichtshof", Folder = true, Lazy = false });
            _lFolderData[0].Children.Add(new FolderData() { Id = "3", Key = "1.2", Title = "Verwaltungsgerichtshof", Folder = true, Lazy = false });
            _lFolderData[0].Children.Add(new FolderData() { Id = "4", Key = "1.3", Title = "Oberster Gerichtshof", Folder = true, Lazy = false });
            _lFolderData[0].Children.Add(new FolderData() { Id = "5", Key = "1.4", Title = "Bundesverwaltungsgericht", Folder = true, Lazy = false });

            _lFolderData.Add(new FolderData() { Id = "6", Key = "2", Title = "Bulgaria", Folder = true, Lazy = false });
            _lFolderData[1].Children.Add(new FolderData() { Id = "7", Key = "2.1", Title = "Върховен касационен съд", Folder = true, Lazy = false });
            _lFolderData[1].Children.Add(new FolderData() { Id = "8", Key = "2.2", Title = "Върховен административен съд", Folder = true, Lazy = false });
            _lFolderData[1].Children.Add(new FolderData() { Id = "9", Key = "2.3", Title = "Конституционен съд", Folder = true, Lazy = false });

            _lFolderData.Add(new FolderData() { Id = "10", Key = "3", Title = "France", Folder = true, Lazy = false });
            _lFolderData[2].Children.Add(new FolderData() { Id = "11", Key = "3.1", Title = "Cour de cassation", Folder = true, Lazy = false });
            _lFolderData[2].Children.Add(new FolderData() { Id = "12", Key = "3.2", Title = "Conseil d'Etat", Folder = true, Lazy = false });
            _lFolderData[2].Children.Add(new FolderData() { Id = "13", Key = "3.3", Title = "Conseil constitutionnel", Folder = true, Lazy = false });

            _lFolderData.Add(new FolderData() { Id = "14", Key = "4", Title = "Germany", Folder = true, Lazy = false });
            _lFolderData[3].Children.Add(new FolderData() { Id = "15", Key = "4.1", Title = "Bundesverfassungsgericht", Folder = true, Lazy = false });
            _lFolderData[3].Children.Add(new FolderData() { Id = "16", Key = "4.2", Title = "Bundesgerichtshof", Folder = true, Lazy = false });
            _lFolderData[3].Children.Add(new FolderData() { Id = "17", Key = "4.3", Title = "Bundessozialgericht", Folder = true, Lazy = false });
            _lFolderData[3].Children.Add(new FolderData() { Id = "18", Key = "4.4", Title = "Bundesarbeitsgericht", Folder = true, Lazy = false });
            _lFolderData[3].Children.Add(new FolderData() { Id = "19", Key = "4.5", Title = "Bundesfinanzhof", Folder = true, Lazy = false });
            _lFolderData[3].Children.Add(new FolderData() { Id = "20", Key = "4.6", Title = "Bundesverwaltungsgericht", Folder = true, Lazy = false });
            _lFolderData[3].Children.Add(new FolderData() { Id = "21", Key = "4.7", Title = "Gemeinsamer Senat der Obersten Gerichtshöfe", Folder = true, Lazy = false });

            _lFolderData.Add(new FolderData() { Id = "22", Key = "5", Title = "Italy", Folder = true, Lazy = false });
            _lFolderData[4].Children.Add(new FolderData() { Id = "23", Key = "5.1", Title = "Corte Suprema di Cassazione", Folder = true, Lazy = false });
            _lFolderData[4].Children.Add(new FolderData() { Id = "24", Key = "5.2", Title = "Consiglio di Stato", Folder = true, Lazy = false });
            _lFolderData[4].Children.Add(new FolderData() { Id = "25", Key = "5.3", Title = "Corte costituzionale", Folder = true, Lazy = false });

            _lFolderData.Add(new FolderData() { Id = "26", Key = "6", Title = "United Kingdom", Folder = true, Lazy = false });
            _lFolderData[5].Children.Add(new FolderData() { Id = "23", Key = "6.1", Title = "Supreme court", Folder = true, Lazy = false });
            _lFolderData[5].Children.Add(new FolderData() { Id = "23", Key = "6.2", Title = "House of Lords", Folder = true, Lazy = false });
            _lFolderData[5].Children.Add(new FolderData() { Id = "23", Key = "6.3", Title = "High Court of Justice", Folder = true, Lazy = false });

        }

        public List<FolderData> Get()
        {
            return _lFolderData;
        }
    }
}
