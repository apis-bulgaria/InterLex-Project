import {Data, DataSet, Edge, IdType, Node} from "vis";

export type NodeTypes = 'startNode' | 'endNode' | 'linkNode' | 'taskNode' | 'decisionNode';

export interface INodeTypes {
  id: NodeTypes,
  name: string
}

export interface IOldNomenclature {
  id: string,
  name: string
}

export interface INomenclature {
  id: number;
  name: string;
}

export enum MasterGraphCategories {
  'International jurisdiction' = 1,
  'Applicable law' = 2
}

export enum GraphTypes {
  Core = 1,
  National = 2
}

export interface ICreateMaster {
  title?: string;
  order?: number;
  masterGraphCategory?: MasterGraphCategories;
}

export interface IMasterDetailModel {
  title: string;
  graphs: IGraphListModel[];
}

export interface IGraphListModel {
  id: string;
  title: string;
  type: GraphTypes;
  // countrie : ???
}

export interface IGraphData {
  title: string;
  data: string;
}

export interface ICreateResponse {
  id: string;
  success: boolean;
  message: string;
}

export interface IGetAllResponse {
  success: boolean;
  masterGraphs: IMasterGraphListModel[];
}

export interface IMasterGraphListModel {
  id: string;
  order: number;
  title: string;
  masterGraphCategory: number;
}

export interface IMetaInfoResponse {
  languages: ILanguage[];
  graphConnectionTypes: INomenclature[];
  linkTypes: ILinkType[];
}

export interface NodeData {
  id: string;
  label: string;
  group?: string;
  x: number;
  y: number;
}

export interface ILanguage {
  id: number;
  shortLang: string;
  lang: string;
  code: string;
  displayText: string;
}

export interface ILinkType {
  id: number;
  shortName: string;
  fullName: string;
}

export interface ILink {
  name: string;
  url: string;
  linkType: string;
}

export interface ILinksTexts {
  texts: IText[];
}

export interface IText {
  id: string;
  twoLetterLanguage: string;
  text: string;
  references: IReference[];
}

export interface IReference {
  text: string;
  url: string;
  hintUrl: string;
  type: number;
  docNumber: string;
}

export class EditNode {
  language?: IOldNomenclature;
  isVisible?: boolean;
  textNavigator?: string;
  content?: string;
  keywords?: string[] = [];
  links?: ILink[] = [];
}

export interface MyNode extends Node {
  group?: NodeTypes;
  language?: ILanguage;
  isHidden?: boolean;
  // textNavigator?: string;
  // titleFormat?: string;
  // content?: string;
  // keywords?: string[];
  // links?: ILink[];
  order?: number;
  translations?: ITranslate;
}

export interface ITranslate {
  [code: string]: TranslateFields;
}

export class TranslateFields {
  title?: string;
  titleFormat?: string;
  content?: string;
  linkedContent?: string;
  textNavigator?: string;
  keywords: string[] = [];
  links: ILink[] = [];
  autoLinks: IReference[];
}

export interface ILinkRequest {
  id: string | number;
  twoLetterLanguage: string;
  text: string;
}

export class CustomData implements Data {
  nodes: DataSet<MyNode>;
  edges: DataSet<Edge>;


  constructor(nodes: MyNode[] = undefined, edges: Edge[] = undefined) {
    this.nodes = nodes ? new DataSet<MyNode>(nodes) : new DataSet<MyNode>();
    this.edges = edges ? new DataSet<Edge>(edges) : new DataSet<Edge>();
  }

  public getNodesByGroup(groupName: string): MyNode[] {
    return this.nodes.get({filter: x => x.group === groupName});
  }

  public getOutgoingEdgesById(nodeId: IdType): Edge[] {
    return this.edges.get({filter: x => x.from === nodeId});
  }
}

export interface ExportModel {
  name: string;
  data: MyData;
}

export interface MyData {
  nodes: MyNode[];
  edges: Edge[];
}

export interface EdgeData {
  from: any;
  to: any;

  [z: string]: any;
}
