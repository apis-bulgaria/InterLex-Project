namespace NewInterlex.Core.Dto
{
    using System;

    public class MasterGraphListModel
    {
        public Guid Id { get; set; }

        public int Order { get; set; }

        public string Title { get; set; }

        public int MasterGraphCategory { get; set; }
    }
}