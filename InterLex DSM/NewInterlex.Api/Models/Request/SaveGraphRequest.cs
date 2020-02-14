namespace NewInterlex.Api.Models.Request
{
    public class SaveGraphRequest
    {
        public string Content { get; set; }

        public string Title { get; set; }

        public int GraphType { get; set; } // maybe enum - 1 - Core, 2 - National
    }
}