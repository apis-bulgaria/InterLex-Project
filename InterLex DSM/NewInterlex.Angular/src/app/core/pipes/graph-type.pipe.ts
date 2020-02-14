import {Pipe, PipeTransform} from '@angular/core';
import {GraphTypes} from "../../models/common.models";

@Pipe({
  name: 'graphType'
})
export class GraphTypePipe implements PipeTransform {

  transform(value: any, args?: any): any {
    return GraphTypes[value];
  }

}
