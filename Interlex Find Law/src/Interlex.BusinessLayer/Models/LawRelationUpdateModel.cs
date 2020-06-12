namespace Interlex.BusinessLayer.Models
{
    using System.Collections.Generic;

    public class LawRelationUpdateModel
    {
        public string FromCelex { get; set; }

        public string FromArticle { get; set; }

        public string ToCelex { get; set; }

        public string ToArticle { get; set; }

        public IEnumerable<int> LinkIds { get; set; } = new List<int>();

        public int ToDocParId { get; set; }
    }
}
