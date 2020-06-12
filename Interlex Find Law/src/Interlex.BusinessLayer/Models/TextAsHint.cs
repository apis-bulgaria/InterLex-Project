using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Interlex.BusinessLayer.Models
{
    public class TextAsHint
    {
        public string Text
        {
            get;
            set;
        }

        public TextAsHint()
        {
            this.Text = "Article 4" +
"Management procedure <br/>" +
"1. The Commission shall be assisted by a management committee composed of the representatives of the Member States and chaired by the representative of the Commission.<br/>" +
"2. The representative of the Commission shall submit to the committee a draft of the measures to be taken. The committee shall deliver its opinion on the draft within a time-limit which the chairman may lay down according to the urgency of the matter. The opinion shall be delivered by the majority laid down in Article 205(2) of the Treaty, in the case of decisions which the Council is required to adopt on a proposal from the Commission. The votes of the representatives of the Member States within the committee shall be weighted in the manner set out in that Article. The chairman shall not vote.<br/>" +
"3. The Commission shall, without prejudice to Article 8, adopt measures which shall apply immediately. However, if these measures are not in accordance with the opinion of the committee, they shall be communicated by the Commission to the Council forthwith. In that event, the Commission may defer application of the measures which it has decided on for a period to be laid down in each basic instrument but which shall in no case exceed three months from the date of such communication.<br/>" +
"4. The Council, acting by qualified majority, may take a different decision within the period provided for by paragraph 3.<br/>";
        }
    }
}