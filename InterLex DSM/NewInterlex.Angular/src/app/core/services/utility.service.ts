import {Injectable} from '@angular/core';
import {IReference, MyNode, TranslateFields} from "../../models/common.models";

@Injectable({
  providedIn: 'root'
})
export class UtilityService {

  constructor() {
  }

  public ensureLangProperty(node: MyNode, currentLang: string) {
    if (node.translations == undefined) {
      node.translations = {};
    }
    if (node.translations[currentLang] == undefined) {
      node.translations[currentLang] = new TranslateFields();
    }
  }

  public ensureLangPropertyMultiple(nodes: MyNode[], currentLang: string) {
    if (nodes) {
      nodes.forEach(x => this.ensureLangProperty(x, currentLang));
    }
  }

  public sortAutolinks (links: IReference[]) : IReference[] {
    return links.sort((a, b) => a.docNumber.localeCompare(b.docNumber));
  }
}
