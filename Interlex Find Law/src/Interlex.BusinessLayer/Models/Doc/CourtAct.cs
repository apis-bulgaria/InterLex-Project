using System.Collections.Generic;

namespace Interlex.BusinessLayer.Models
{
    public class CourtAct : Document
    {
        public CourtAct() : base() { }

        public CourtAct(string strXML, int langIdofDoc, int docLangId, int uiLangId, DocHighlightSearchParams highlightParams)
            : base(strXML, langIdofDoc, docLangId, uiLangId, highlightParams)
        { }
    }
}
