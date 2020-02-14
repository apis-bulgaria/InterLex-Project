import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {environment} from 'src/environments/environment';
import {Edge} from "vis";
import {
  CustomData,
  ICreateMaster,
  ICreateResponse, IGetAllResponse, IGraphData, ILinkRequest, ILinksTexts,
  IMasterDetailModel,
  IMetaInfoResponse,
  MyNode
} from "../../models/common.models";
import {forkJoin, Observable} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class HttpService {

  apiUrl = environment.apiBaseUrl;

  constructor(private http: HttpClient) {
  }

  getMetaInfo(): Observable<IMetaInfoResponse> {
    return this.http.get<IMetaInfoResponse>(this.apiUrl + 'Graph/GetMeta');
  }

  saveNode(node: MyNode) {
    console.log(node);
    // return this.http.post(this.apiUrl + 'Graph/SaveInitialNode', node);
  }

  saveEdge(data: Edge) {
    console.log(data);
    // return this.http.post(this.apiUrl + Graph/SaveEdge, edge);
  }

  deleteNodes(nodes: string[]) {
    console.log(nodes);
  }

  deleteEdges(edges: string[]) {
    console.log(edges);
  }

  saveDiagram(networkData: CustomData, id: string): Observable<any> {
    const content = JSON.stringify({nodes: networkData.nodes.get(), edges: networkData.edges.get()});
    return this.http.post(this.apiUrl + 'Graph/SaveGraphData/' + id, {content});
  }

  insertLinks(arr: ILinkRequest[], id: string): Observable<ILinksTexts> {
    return this.http.post<ILinksTexts>(this.apiUrl + 'Graph/InsertLinks/' + id, {texts: arr});
  }

  createMaster(model: ICreateMaster): Observable<ICreateResponse> {
    return this.http.post<ICreateResponse>(this.apiUrl + 'MasterGraph/Create', model);
  }

  getGraphList(id: string): Observable<IMasterDetailModel> {
    return this.http.get<IMasterDetailModel>(this.apiUrl + 'MasterGraph/Details/' + id);
  }

  getGraphData(id: string): Observable<IGraphData> {
    return this.http.get<IGraphData>(this.apiUrl + 'Graph/GetGraph/' + id);
  }

  getMultipleGraphData(...ids : string[]) : Observable<IGraphData[]>{
    const obs = ids.map(x => this.getGraphData(x));
    return forkJoin(obs);
  }

  getAllGraphs(): Observable<IGetAllResponse> {
    return this.http.get<IGetAllResponse>(this.apiUrl + 'MasterGraph/GetAll');
  }

  getText(url: string): Observable<string> {
    return this.http.get(url, {responseType: "text"});
  }
}
