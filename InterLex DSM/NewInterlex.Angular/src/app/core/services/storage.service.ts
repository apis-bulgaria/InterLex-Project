import {Injectable} from '@angular/core';
import {CustomData, ExportModel, MyData, MyNode} from "../../models/common.models";
import {DataSet, Edge} from "vis";

@Injectable({providedIn: "root"})
export class StorageService {

  constructor() {
  }


  get networkData(): CustomData {
    const stringData = localStorage.getItem('customData');
    if (stringData) {
      const myObj = JSON.parse(stringData);
      return new CustomData(myObj.nodes, myObj.edges);
    }
  }

  set networkData(value: CustomData) {
    const myObj = {nodes: value.nodes.get(), edges: value.edges.get()};
    localStorage.setItem('customData', JSON.stringify(myObj));
  }

  saveExported(exportModel: ExportModel) {
    const allExported = localStorage.getItem('exportedItems') || '{}';
    const obj = JSON.parse(allExported);
    obj[exportModel.name] = exportModel.data;
    localStorage.setItem('exportedItems', JSON.stringify(obj));
  }

  getExportedNames() {
    const all = localStorage.getItem('exportedItems') || '{}';
    const obj = JSON.parse(all);
    var arr = Object.keys(obj);
    return arr;
  }

  getExportedData(name: string): MyData {
    const allExported = localStorage.getItem('exportedItems') || '{}';
    const obj = JSON.parse(allExported);
    return obj[name];
  }

}
