using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interlex.BusinessLayer.Enums;

namespace Interlex.BusinessLayer.Models
{
    public class ClassifierItem : IEquatable<ClassifierItem>
    {
        public Guid Id { get; set; }
        public ClassifierTypes Type { get; set; }
        public string Name { get; set; }
        public int DocsCount { get; set; }

        public int OrderValue { get; set; }

        public override String ToString()
        {
            return Name;
        }

        public bool Equals(ClassifierItem item)
        {
            return this.Id == item.Id;
        }
    }
}
