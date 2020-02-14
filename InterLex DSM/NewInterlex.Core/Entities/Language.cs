namespace NewInterlex.Core.Entities
{
    using Shared;

    public class Language : BaseIdEntity
    {
        public string ShortLang { get; set; }
        public string Lang { get; set; }
        public string Code { get; set; }
        public string DisplayText { get; set; }
    }
}