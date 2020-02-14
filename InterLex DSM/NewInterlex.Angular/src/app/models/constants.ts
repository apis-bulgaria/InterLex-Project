import {INodeTypes, INomenclature, IOldNomenclature} from "./common.models";

export const nodeTypes: INodeTypes[] = [
  {id: 'startNode', name: 'Начало'},
  {id: 'taskNode', name: 'Задача'},
  {id: 'decisionNode', name: 'Решение'},
  {id: 'endNode', name: 'Край/Завърши'},
  {id: 'linkNode', name: 'Линк'},
];

export const languages: IOldNomenclature[] = [
  {id: 'bg', name: 'bg'},
  {id: 'en', name: 'en'},
];

export const MasterGraphCats: INomenclature[] = [
  {id: 1, name: 'International jurisdiction'},
  {id: 2, name: 'Applicable law'}
];
