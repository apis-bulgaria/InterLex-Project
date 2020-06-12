using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlex.BusinessLayer.Entities
{
    public class JurisdictionMock: IJurisdiction
    {
        private List<FolderData> _lFolderData;

        public JurisdictionMock()
        {
            _lFolderData = new List<FolderData>();
            _lFolderData.Add(new FolderData() { id = Guid.Parse("102a78df-6eac-eb49-98f8-74f88a8815bd"), key = "1", title = "European Union", folder = true, lazy = false });
            _lFolderData.Add(new FolderData() { id = Guid.Parse("102a78df-6eac-eb49-98f8-74f88a8815bd"), key = "2", title = "Austria", folder = true, lazy = false });
            _lFolderData.Add(new FolderData() { id = Guid.Parse("102a78df-6eac-eb49-98f8-74f88a8815bd"), key = "3", title = "Bulgaria", folder = true, lazy = false });
            _lFolderData.Add(new FolderData() { id = Guid.Parse("102a78df-6eac-eb49-98f8-74f88a8815bd"), key = "4", title = "France", folder = true, lazy = false });
            _lFolderData.Add(new FolderData() { id = Guid.Parse("102a78df-6eac-eb49-98f8-74f88a8815bd"), key = "5", title = "Germany", folder = true, lazy = false });
            _lFolderData.Add(new FolderData() { id = Guid.Parse("102a78df-6eac-eb49-98f8-74f88a8815bd"), key = "6", title = "Italy", folder = true, lazy = false });
            _lFolderData.Add(new FolderData() { id = Guid.Parse("102a78df-6eac-eb49-98f8-74f88a8815bd"), key = "7", title = "United Kingdom", folder = true, lazy = false });

        }

        public List<FolderData> Get()
        {
            return _lFolderData;
        }
    }
}
