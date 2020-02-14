namespace NewInterlex.Core.Dto
{
    public sealed class Error
    {
        public string Code { get; set; }

        public string Description { get; set; }

        public Error(string code, string description)
        {
            this.Code = code;
            this.Description = description;
        }
    }
}