import {Component, OnInit} from '@angular/core';
import {SelectItem} from 'primeng/api';
import {TranslateService} from "@ngx-translate/core";
import {HttpService} from "../services/http.service";


@Component({
  selector: 'app-language-switcher',
  templateUrl: './language-switcher.component.html',
  styleUrls: ['./language-switcher.component.scss']
})
export class LanguageSwitcherComponent implements OnInit {

  selectedLanguage: string;
  languages: SelectItem[];

  constructor(private translate: TranslateService, private http: HttpService) {
    const defaultLang = 'en';  // todo: this needs thinking when language becomes part of profile,
    //todo: currenlty currentLang is set slowly so it's undefined in other components initially
    this.translate.use(defaultLang);
    this.translate.setDefaultLang(defaultLang);
    this.selectedLanguage = defaultLang;
    this.http.getMetaInfo().subscribe(info => {
      const languages = info.languages;
      this.languages = languages.map(x => ({label: x.displayText, value: x.shortLang}));
    });
  }

  ngOnInit() {
  }

  changeLang() {
    this.translate.use(this.selectedLanguage);
  }
}
