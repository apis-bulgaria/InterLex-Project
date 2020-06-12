namespace Interlex.BusinessLayer
{
    using System.Collections.Generic;
    using Interlex.BusinessLayer.Models;

    internal class AlphabetComparer : IComparer<AlphabetLetter>
    {
        public int Compare(AlphabetLetter firstLetter, AlphabetLetter secondLetter)
        {
            return firstLetter.Letter.CompareTo(secondLetter.Letter);
        }
    }
}
