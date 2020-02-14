import {Pipe, PipeTransform} from '@angular/core';
import {MasterGraphCategories} from "../../models/common.models";

@Pipe({
  name: 'masterCategory'
})
export class MasterCategoryPipe implements PipeTransform {

  transform(value: any, args?: any): any {
    return MasterGraphCategories[value];
  }
}
