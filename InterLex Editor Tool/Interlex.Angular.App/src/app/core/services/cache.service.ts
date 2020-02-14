import {Injectable} from '@angular/core';
import {LanguageModel} from "../../models/language.model";
import {Observable, of} from "rxjs";
import {HttpService} from "./http.service";
import {filter, map, tap} from "rxjs/operators";

@Injectable({providedIn: "root"})
export class CacheService {

  private euLanguages: LanguageModel[];

  constructor(private http: HttpService) {

  }

  getEuLanguages(): Observable<LanguageModel[]> {
    if (this.euLanguages) {
      return of(this.euLanguages);
    } else {
      return (this.http.get('Data/GetEuLanguages') as Observable<LanguageModel[]>)
        .pipe(
          map(x => x.filter(z => z.nameEn !== 'Serbian')),
          tap(data => this.euLanguages = data)
        );
    }
  }

}
