import {Component, OnInit, Input, Output, EventEmitter} from '@angular/core';
import {MenuItem} from 'primeng/api';
import {MyNode} from '../models/common.models';
import {LangChangeEvent, TranslateService} from "@ngx-translate/core";
import {UtilityService} from "../core/services/utility.service";


@Component({
  selector: 'app-breadcrumbs',
  templateUrl: './breadcrumbs.component.html',
  styleUrls: ['./breadcrumbs.component.scss']
})
export class BreadcrumbsComponent implements OnInit {

  private _nodes = [];

  constructor(private translateService: TranslateService, private utilityService: UtilityService) {
  }

  currentLang: string;

  @Input('nodes')
  set nodes(nodesInput: MyNode[]) {
    this._nodes = nodesInput;
    this.fillItems(this._nodes);
  }

  @Input('histChanged')
  set histChanged(nodesInput: number) {
    this.fillItems(this._nodes);
  }

  @Output() clickId = new EventEmitter<string>();

  private fillItems(nodes: MyNode[]) {

    if (nodes) {
      const items = nodes.filter(x => !x.isHidden).map((x) => {
        return {
          label: x.translations[this.currentLang].textNavigator || x.translations[this.currentLang].title,
          id: x.id.toString(),
          command: (event) => {
            this.clicked(event.item.id);
          }
        }
      });
      items.pop(); // in presentation component nodes are saved in history prematurely, so fixing here
      this.items = items;
    } else {
      this.items = [];
    }
  }

  public clicked(id: string) {
    this.clickId.emit(id);
  }

  public items: MenuItem[];

  home: MenuItem;


  ngOnInit() {
    this.currentLang = this.translateService.currentLang || 'en';
    this.translateService.onLangChange.subscribe((x: LangChangeEvent) => {
      this.handleTranslationChange(x);
    });

    this.items = [
      // { label: 'First question' , command: (event)=>console.log(event.item.label)},
      // { label: 'Second question' },
      // { label: 'Third question' },
      // { label: 'Fourth question' },
      // { label: 'Lionel Messi', url: 'https://en.wikipedia.org/wiki/Lionel_Messi' }
    ];
  }

  private handleTranslationChange(x: LangChangeEvent) {
    this.currentLang = x.lang;  // needed for dynamic template binding to translation property
    this.utilityService.ensureLangPropertyMultiple(this._nodes, this.currentLang);
    this.fillItems(this._nodes);
  }
}
