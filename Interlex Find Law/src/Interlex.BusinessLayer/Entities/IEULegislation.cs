using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlex.BusinessLayer.Entities
{
    public interface IEULegislation
    {
        List<FolderData> Get();
    }
}
