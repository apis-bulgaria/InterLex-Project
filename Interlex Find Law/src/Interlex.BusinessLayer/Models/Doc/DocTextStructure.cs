using System.Collections.Generic;
using Interlex.BusinessLayer.Entities;

namespace Interlex.BusinessLayer.Models
{
    public class DocTextStructure
    {
        public List<DocTextPart> Parts
        {
            get;
            set;
        }


        public DocTextStructure()
        {
            Parts = new List<DocTextPart>();
            Parts.Add(
                new DocTextPart()
                {
                    DocTextPartId = "",
                    HasPractice = false,
                    PartType = Enums.DocPartTypes.Title,
                    Title = "",
                    Html = @"THE COUNCIL OF THE EUROPEAN COMMUNITIES,<br/>
Having regard to the Treaty establishing the European Economic Community, and in particular Article 100 thereof,<br/>
Having regard to the proposal from the Commission (1),<br/>
Having regard to the opinion of the European Parliament (2),<br/>
Having regard to the opinion of the Economic and Social Committee (3),<br/>
Whereas the laws against misleading advertising now in force in the Member States differ widely; whereas, since advertising reaches beyond the frontiers of individual Member States, it has a direct effect on the establishment and the functioning of the common market;<br/>
Whereas misleading advertising can lead to distortion of competition within the common market;<br/>
Whereas advertising, whether or not it induces a contract, affects the economic welfare of consumers;<br/>
Whereas misleading advertising may cause a consumer to take decisions prejudicial to him when acquiring goods or other property, or using services, and the differences between the laws of the Member States not only lead, in many cases, to inadequate levels of consumer protection, but also hinder the execution of advertising campaigns beyond national boundaries and thus affect the free circulation of goods and provision of services;<br/>
Whereas the second programme of the European Economic Community for a consumer protection and information policy (4) provides for appropriate action for the protection of consumers against misleading and unfair advertising;<br/>
Whereas it is in the interest of the public in general, as well as that of consumers and all those who, in competition with one another, carry on a trade, business, craft or profession, in the common market, to harmonize in the first instance national provisions against misleading advertising and that, at a second stage, unfair advertising and, as far as necessary, comparative advertising should be dealt with, on the basis of appropriate Commission proposals;<br/>
Whereas minimum and objective criteria for determining whether advertising is misleading should be established for this purpose;<br/>
Whereas the laws to be adopted by Member States against misleading advertising must be adequate and effective;<br/>
Whereas persons or organizations regarded under national law as having a legitimate interest in the matter must have facilities for initiating proceedings against misleading advertising, either before a court or before an administrative authority which is competent to decide upon complaints or to initiate appropriate legal proceedings;<br/>
Whereas it should be for each Member State to decide whether to enable the courts or administrative authorities to require prior recourse to other established means of dealing with the complaint;<br/>
Whereas the courts or administrative authorities must have powers enabling them to order or obtain the cessation of misleading advertising;<br/>
Whereas in certain cases it may be desirable to prohibit misleading advertising even before it is published; whereas, however, this in no way implies that Member States are under an obligation to introduce rules requiring the systematic prior vetting of advertising;<br/>
Whereas provision should be made for accelerated procedures under which measures with interim or definitive effect can be taken;<br/>
Whereas it may be desirable to order the publication of decisions made by courts or administrative authorities or of corrective statements in order to eliminate any continuing effects of misleading advertising;<br/>
Whereas administrative authorities must be impartial and the exercise of their powers must be subject to judicial review;<br/>
Whereas the voluntary control exercised by self-regulatory bodies to eliminate misleading advertising may avoid recourse to administrative or judicial action and ought therefore to be encouraged;<br/>
Whereas the advertiser should be able to prove, by appropriate means, the material accuracy of the factual claims he makes in his advertising, and may in appropriate cases be required to do so by the court or administrative authority;<br/>
Whereas this Directive must not preclude Member States from retaining or adopting provisions with a view to ensuring more extensive protection of consumers, persons carrying on a trade, business, craft or profession, and the general public,<br/>
HAS ADOPTED THIS DIRECTIVE:<br/>"
                }
                );
            Parts.Add(
                new DocTextPart()
                {
                    DocTextPartId = "art1",
                    HasPractice = true,
                    PartType = Enums.DocPartTypes.Article,
                    Title = "Article 1",
                    Html = "The purpose of this Directive is to protect consumers, persons carrying on a trade or business or practising a craft or profession and the interests of the public in general against misleading advertising and the unfair consequences thereof."
                }
                );
            Parts.Add(
                new DocTextPart()
                {
                    DocTextPartId = "art2",
                    HasPractice = true,
                    PartType = Enums.DocPartTypes.Article,
                    Title = "Article 2",
                    Html = @"For the purposes of this Directive: <br/>
1. 'advertising' means the making of a representation in any form in connection with a trade, business, craft or profession in order to promote the supply of goods or services, including immovable property, rights and obligations; <br/>
2. 'misleading advertising' means any advertising which in any way, including its presentation, deceives or is likely to deceive the persons to whom it is addressed or whom it reaches and which, by reason of its deceptive nature, is likely to affect their economic behaviour or which, for those reasons, injures or is likely to injure a competitor; <br/>
3. 'person' means any natural or legal person.<br/>"
                }
                );
            Parts.Add(
                new DocTextPart()
                {
                    DocTextPartId = "art3",
                    HasPractice = true,
                    PartType = Enums.DocPartTypes.Article,
                    Title = "Article 3",
                    Html = @"In determining whether advertising is misleading, account shall be taken of all its features, and in particular of any information it contains concerning: <br/>
(a) the characteristics of goods or services, such as their availability, nature, execution, composition, method and date of manufacture or provision, fitness for purpose, uses, quantity, specification, geographical or commercial origin or the results to be expected from their use, or the results and material features of tests or checks carried out on the goods or services; <br/>
(b) the price or the manner in which the price is calculated, and the conditions on which the goods are supplied or the services provided; <br/>
(c) the nature, attributes and rights of the advertiser, such as his identity and assets, his qualifications and ownership of industrial, commercial or intellectual property rights or his awards and distinctions. <br/>"
                }
                );
            Parts.Add(
                new DocTextPart()
                {
                    DocTextPartId = "art4",
                    HasPractice = true,
                    PartType = Enums.DocPartTypes.Article,
                    Title = "Article 4",
                    Html = @"1. Member States shall ensure that adequate and effective means exist for the control of misleading advertising in the interests of consumers as well as competitors and the general public. Such means shall include legal provisions under which persons or organizations regarded under national law as having a legitimate interest in prohibiting misleading advertising may:<br/>
(a) take legal action against such advertising; and/or<br/>
(b) bring such advertising before an administrative authority competent either to decide on complaints or to initiate appropriate legal proceedings.<br/>
It shall be for each Member State to decide which of these facilities shall be available and whether to enable the courts or administrative authorities to require prior recourse to other established means of dealing with complaints, including those referred to in Article 5.<br/>
2. Under the legal provisions referred to in paragraph 1, Member States shall confer upon the courts or administrative authorities powers enabling them, in cases where they deem such measures to be necessary taking into account all the interests involved and in particular the public interest:<br/>
- to order the cessation of, or to institute appropriate legal proceedings for an order for the cessation of, misleading advertising, or<br/>
- if misleading advertising has not yet been published but publication is imminent, to order the prohibition of, or to institute appropriate legal proceedings for an order for the prohibition of, such publication,<br/>
even without proof of actual loss or damage or of intention or negligence on the part of the advertiser.<br/>
Member States shall also make provision for the measures referred to in the first subparagraph to be taken under an accelerated procedure:<br/>
- either with interim effect, or<br/>
- with definitive effect,<br/>
on the understanding that it is for each Member State to decide which of the two options to select.<br/>
Furthermore, Member States may confer upon the courts or administrative authorities powers enabling them, with a view to eliminating the continuing effects of misleading advertising the cessation of which has been ordered by a final decision:<br/>
- to require publication of that decision in full or in part and in such form as they deem adequate,<br/>
- to require in addition the publication of a corrective statement.<br/>
3. The administrative authorities referred to in paragraph 1 must:<br/>
(a) be composed so as not to cast doubt on their impartiality;<br/>
(b) have adequate powers, where they decide on complaints, to monitor and enforce the observance of their decisions effectively;<br/>
(c) normally give reasons for their decisions.<br/>
Where the powers referred to in paragraph 2 are exercised exclusively by an administrative authority, reasons for its decisions shall always be given. Furthermore in this case, provision must be made for procedures whereby improper or unreasonable exercise of its powers by the administrative authority or improper or unreasonable failure to exercise the said powers can be the subject of judicial review."
                }
                );
            Parts.Add(
                new DocTextPart()
                {
                    DocTextPartId = "art5",
                    HasPractice = true,
                    PartType = Enums.DocPartTypes.Article,
                    Title = "Article 5",
                    Html = @"This Directive does not exclude the voluntary control of misleading advertising by self-regulatory bodies and recourse to such bodies by the persons or organizations referred to in Article 4 if proceedings before such bodies are in addition to the court or administrative proceedings referred to in that Article."
                }
                );
            Parts.Add(
                new DocTextPart()
                {
                    DocTextPartId = "art6",
                    HasPractice = true,
                    PartType = Enums.DocPartTypes.Article,
                    Title = "Article 6",
                    Html = @"Member States shall confer upon the courts or administrative authorities powers enabling them in the civil or administrative proceedings provided for in Article 4:<br/>
(a) to require the advertiser to furnish evidence as to the accuracy of factual claims in advertising if, taking into account the legitimate interests of the advertiser and any other party to the proceedings, such a requirement appears appropriate on the basis of the circumstances of the particular case; and<br/>
(b) to consider factual claims as inaccurate if the evidence demanded in accordance with (a) is not furnished or is deemed insufficient by the court or administrative authority."
                }
                );
            Parts.Add(
                new DocTextPart()
                {
                    DocTextPartId = "art7",
                    HasPractice = true,
                    PartType = Enums.DocPartTypes.Article,
                    Title = "Article 7",
                    Html = @"This Directive shall not preclude Member States from retaining or adopting provisions with a view to ensuring more extensive protection for consumers, persons carrying on a trade, business, craft or profession, and the general public."
                }
                );
            Parts.Add(
                new DocTextPart()
                {
                    DocTextPartId = "art8",
                    HasPractice = true,
                    PartType = Enums.DocPartTypes.Article,
                    Title = "Article 8",
                    Html = @"Member States shall bring into force the measures necessary to comply with this Directive by 1 October 1986 at the latest. They shall forthwith inform the Commission thereof.<br/>
Member States shall communicate to the Commission the text of all provisions of national law which they adopt in the field covered by this Directive."
                }
                );
            Parts.Add(
                new DocTextPart()
                {
                    DocTextPartId = "art9",
                    HasPractice = true,
                    PartType = Enums.DocPartTypes.Article,
                    Title = "Article 9",
                    Html = @"This Directive is addressed to the Member States.<br/>
Done at Brussels, 10 September 1984.<br/>
For the Council<br/>
The President<br/>
P. O'TOOLE<br/>
(1) OJ No C 70, 21. 3. 1978, p. 4.<br/>
(2) OJ No C 140, 5. 6. 1979, p. 23.<br/>
(3) OJ No C 171, 9. 7. 1979, p. 43.<br/>
(4) OJ No C 133, 3. 6. 1981, p. 1."
                }
                );

        }
    }
}