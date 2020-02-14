namespace NewInterlex.Core.Entities
{
    using Shared;

    public class LinkType : BaseIdEntity
    {
        public string ShortName { get; set; }
        public string FullName { get; set; }
    }
}