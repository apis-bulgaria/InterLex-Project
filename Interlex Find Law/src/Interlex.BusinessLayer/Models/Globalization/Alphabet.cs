namespace Interlex.BusinessLayer.Models
{
    using System.Collections.Generic;

    public class Alphabet
    {
        public int LangId { get; set; }

        public SortedSet<AlphabetLetter> Letters { get; set; }

        public Alphabet(int langId, SortedSet<AlphabetLetter> letters)
        {
            this.LangId = langId;
            this.Letters = letters;
        }
    }
}
