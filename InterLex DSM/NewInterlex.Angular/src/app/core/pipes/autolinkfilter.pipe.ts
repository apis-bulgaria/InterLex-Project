import { Pipe, PipeTransform } from '@angular/core';
import {IReference} from "../../models/common.models";

@Pipe({
  name: 'autolinkfilter'
})
export class AutolinkfilterPipe implements PipeTransform {

  transform(autolinks: IReference[], type: number): IReference[] {
    let arr = [];
    if (autolinks) {
      arr =  autolinks.filter(x => x.type === type);
    }
    return arr;
  }

}
