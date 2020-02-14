namespace NewInterlex.Core.Shared
{
    using System;

    public interface ITimestampEntity
    {
        DateTime Created { get; set; }

        DateTime Modified { get; set; }
    }
}