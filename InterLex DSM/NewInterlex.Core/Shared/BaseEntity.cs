using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace NewInterlex.Core.Shared
{
    public abstract class BaseEntity : BaseIdEntity, ITimestampEntity
    {
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
}
