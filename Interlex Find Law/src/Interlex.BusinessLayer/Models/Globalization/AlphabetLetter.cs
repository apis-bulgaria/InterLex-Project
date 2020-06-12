using System;

namespace Interlex.BusinessLayer.Models
{
    public class AlphabetLetter
    {
        public string Letter { get; set; }

        public bool HasOccurrencesInitially { get; set; }

        public int InitialOccurrencesCount { get; set; }

        public AlphabetLetter(string letter, bool hasOccurencesInitally)
        {
            this.Letter = letter;
            this.HasOccurrencesInitially = hasOccurencesInitally;
        }

        public AlphabetLetter(string letter, bool hasOccurencesInitally, int initialOccurencesCount) : this(letter, hasOccurencesInitally)
        {
            this.InitialOccurrencesCount = initialOccurencesCount;
        }
    }
}
