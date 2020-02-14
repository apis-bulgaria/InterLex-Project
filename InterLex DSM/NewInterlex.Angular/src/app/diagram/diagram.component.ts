import {AfterViewInit, ChangeDetectionStrategy, Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {DataSet, Edge, Network, Options} from "vis";
import {
  CustomData,
  EditNode,
  ExportModel,
  ILanguage, ILinksTexts,
  ILinkType,
  INodeTypes,
  INomenclature,
  IOldNomenclature,
  MyData,
  MyNode, TranslateFields
} from "../models/common.models";
import {nodeTypes} from "../models/constants";
import {HttpService} from "../core/services/http.service";
import {StorageService} from "../core/services/storage.service";
import {ActivatedRoute} from "@angular/router";
import {LangChangeEvent, TranslateService} from "@ngx-translate/core";
import {UtilityService} from "../core/services/utility.service";
import {AlertService} from "../core/services/alert.service";

@Component({
  selector: 'app-diagram',
  templateUrl: './diagram.component.html',
  styleUrls: ['./diagram.component.scss'],
  // changeDetection: ChangeDetectionStrategy.Default
})
export class DiagramComponent implements OnInit, AfterViewInit, OnDestroy {

  @ViewChild('visContainer', {static: true}) networkRef;
  network: Network;
  networkData: CustomData;
  title: string;
  data: any;
  callback: Function;

  showAddNode = false;
  showAddEdge = false;
  nodeTypes: INodeTypes[] = nodeTypes;

  languages: ILanguage[];
  linkTypes: ILinkType[];
  graphConnectionTypes: INomenclature[];
  selectedLinkType: ILinkType;
  currentLang: string;

  id: string;
  editNodeObj: EditNode = new EditNode();
  nodeForEdit: MyNode = {};
  selectedNodeType: INodeTypes = this.nodeTypes[0];
  edgeType = false;
  conditionalConnection = false;
  name: string;
  showEditNode = false;
  networkId: string;
  tabViewIndex = 0;
  showExportData = false;
  exportName: string;
  showImportData = false;
  exportedNames: IOldNomenclature[];
  selectedExportName: IOldNomenclature;
  showAddDependency = false;
  linkingInProgress = false;

  // this comes from outside

  constructor(private http: HttpService, private storage: StorageService, private route: ActivatedRoute,
              private translateService: TranslateService, private utilityService: UtilityService,
              private alertService: AlertService) {
  }

  ngOnInit() {
    this.currentLang = this.translateService.currentLang || 'en';
    this.translateService.onLangChange.subscribe((x: LangChangeEvent) => {
      this.handleTranslationChange(x);
    });

    this.route.paramMap.subscribe(params => { // maybe this needs to be in afterviewinit
      this.id = params.get('id');
      this.getGraphData();
    });

    this.http.getMetaInfo().subscribe(info => {
      this.languages = info.languages;
      this.linkTypes = info.linkTypes;
      this.selectedLinkType = info.linkTypes[0];
      this.graphConnectionTypes = info.graphConnectionTypes;
    });
  }

  ngOnDestroy(): void {
    this.storage.networkData = this.networkData;
  }

  ngAfterViewInit(): void {
    // this.fillNetwork();
  }

  private fillNetwork() {
    // const nodes = new DataSet<MyNode>();
    // nodes.add([
    //   {id: 6, label: 'Start Node', group: 'startNode', content: 'The content of the start node'},
    //   {id: 1, label: 'Node 1', links: [], keywords: []},
    //   {id: 2, label: 'Node 2', links: [], keywords: []},
    //   {id: 3, label: 'Node 3', links: [], keywords: []},
    //   {id: 4, label: 'Node 4', links: [], keywords: []},
    //   {id: 5, label: 'Node 5', links: [], keywords: []}
    // ]);
    //
    // // create an array with edges
    // const edges = new DataSet<Edge>();
    // edges.add([
    //   {from: 1, to: 2, arrows: 'to'},
    //   {from: 2, to: 3, arrows: 'to'},
    //   {from: 2, to: 4, arrows: 'to'},
    //   {from: 3, to: 5, arrows: 'to'},
    //   {from: 4, to: 5, arrows: 'to'},
    //   {from: 6, to: 1, arrows: 'to'}
    // ]);
    //
    // // create a network
    // this.networkData = this.storage.networkData || {
    //   nodes,
    //   edges
    // };
    const options: Options = this.initOptions();

    this.network = new Network(this.networkRef.nativeElement, this.networkData, options);
    // this.network.on('click', params => {
    //   console.log(params);
    // });
    // this.network.on('click', x => {
    //   const nodes = x.nodes;
    //   if (nodes.length) {
    //     this.networkData.edges.forEach(e => {
    //       if (e.dashes) {
    //         this.networkData.edges.update({id: e.id, hidden: true});
    //       }
    //     });
    //     const edges = x.edges as string[];
    //     this.networkData.edges.update(edges.map(id => ({id, hidden: false})));
    //   }
    // });

  }

  saveDiagram() {
    this.network.storePositions();
    // this.storage.networkData = this.networkData;
    this.http.saveDiagram(this.networkData, this.id).subscribe(x => {
      this.alertService.success('saved successfully');
    });
  }

  insertLinks() {
    const reqArr = [];
    this.networkData.nodes.get().forEach(x => {
      for (const lang in x.translations) {
        if (x.translations.hasOwnProperty(lang)) {
          const obj = {id: x.id, twoLetterLanguage: lang, text: x.translations[lang].content};
          if (obj.text) {
            reqArr.push(obj);
          }
        }
      }
    });
    this.linkingInProgress = true;
    this.http.insertLinks(reqArr, this.id).subscribe({
      next: (x: ILinksTexts) => {
        this.patchNodesWithLinks(x);
        this.alertService.success('Links inserted successfully');
      },
      error: err => {
        this.alertService.error('Error inserting links');
        console.log(err);
      },
      complete: () => {
        this.linkingInProgress = false;
      }
    });
  }

  private initOptions(): Options {
    const options: Options = {
      interaction: {
        navigationButtons: true,
        multiselect: true
        // keyboard: false
      },
      physics: {
        stabilization: false,
        barnesHut: {
          gravitationalConstant: 0,
          centralGravity: 0,
          springConstant: 0
        }
      },
      nodes: {
        widthConstraint: {
          minimum: 0,
          maximum: 250
        },
      },
      edges: {
        arrows: 'to',
        smooth: false
      },
      manipulation: {
        enabled: true,
        addNode: (data, callback) => this.onAddNode(data, callback),
        addEdge: (data, callback) => this.onAddEdge(data, callback),
        editNode: (data, callback) => this.onEditNode(data, callback),
        editEdge: (data, callback) => this.onEditEdge(data, callback),
        deleteNode: (data, callback) => this.onDeleteNode(data, callback),
        deleteEdge: (data, callback) => this.onDeleteEdge(data, callback)
      },
      groups: {
        startNode: {
          shape: 'box',
          color: {
            background: "#EB7DF4",
            border: "#E129F0",
            highlight: {
              background: "#F0B3F5",
              border: "#E129F0",
            },
          },
        },
        endNode: {
          shape: 'box',
          color: {
            background: "#FB7E81",
            border: "#FA0A10",
            highlight: {
              background: "#FFAFB1",
              border: "#FA0A10",
            },
          },
        },
        linkNode: {
          shape: 'box',
          color: {
            background: "#FFFF00",
            border: "#FFA500",
            highlight: {
              background: "#FFFFA3",
              border: "#FFA500",
            },
          },
        },
        taskNode: {
          shape: 'box',
          color: {
            background: "#97C2FC",
            border: "#2B7CE9",
            highlight: {
              background: "#D2E5FF",
              border: "#2B7CE9",
            },
          },
          heightConstraint: {
            minimum: 0,
            maximum: 30 // no such property :(
          }
        },
        decisionNode: {
          shape: 'box',
          color: {
            background: "#7BE141",
            border: "#41A906",
            highlight: {
              background: "#A1EC76",
              border: "#41A906",
            },
          },
        },
      },
      //nodes: { shape: "box" }
    };
    return options;
  }

  private onAddNode(data: MyNode, callback: Function) {
    this.data = data;
    this.callback = callback;
    this.showAddNode = true;
  }

  private onEditNode(data: MyNode, callback: Function) {
    this.data = data;
    this.nodeForEdit = JSON.parse(JSON.stringify(data));
    this.mapGroupToTypeDropdown(data);
    this.callback = callback;

    this.utilityService.ensureLangProperty(this.nodeForEdit, this.currentLang);
    this.nodeForEdit.language = this.languages.find(x => x.shortLang === this.currentLang);

    this.showEditNode = true;
  }

  private mapGroupToTypeDropdown(data: MyNode) {
    this.selectedNodeType = this.nodeTypes.find(x => x.id === data.group);
  }

  addNode() {
    const nodeData = this.data as MyNode;
    nodeData.label = this.name || this.selectedNodeType.id;
    nodeData.group = this.selectedNodeType.id;
    // todo: modifications for newly created nodes/graphs!!! consider if permanent!, remove root level content props

    this.utilityService.ensureLangProperty(nodeData, this.currentLang);
    nodeData.translations[this.currentLang].title = this.name;

    const language = this.languages.find(x => x.shortLang === this.currentLang);
    nodeData.language = language;


    this.resetState();
    this.http.saveNode(nodeData);
    this.callback(this.data);
  }

  editNode() {

  }

  private resetState() {
    this.showAddNode = false;
    this.selectedNodeType = this.nodeTypes[0];
    this.name = null;
  }

  private onEditEdge(data: Edge, callback: Function) {
    callback(data);
  }

  private onAddEdge(data: Edge, callback: Function) {
    if (data.from == data.to) {
      const conf = confirm("Do you want to connect the node to itself?");
      if (conf === true) {
        callback(data);
      }
    } else {
      const sourceNode = this.networkData.nodes.get(data.from);
      if (sourceNode.group === 'decisionNode') {
        this.showAddEdge = true;
        this.data = data;
        this.callback = callback;
      } else {
        this.showAddDependency = true;
        this.data = data;
        this.callback = callback;

        // this.http.saveEdge(data);
        // callback(data);
      }
    }
  }

  removeKeyword(index: number) {
    this.nodeForEdit.translations[this.currentLang].keywords.splice(index, 1);
  }

  addKeyword() {
    this.nodeForEdit.translations[this.currentLang].keywords = [...this.nodeForEdit.translations[this.currentLang].keywords, ''];
  }

  trackByIndex(index: number) {
    return index;
  }

  addLink() {
    this.nodeForEdit.translations[this.currentLang].links = [...this.nodeForEdit.translations[this.currentLang].links, {
      url: '',
      name: '',
      linkType: this.selectedLinkType.shortName
    }];
  }

  removeLink(index: number) {
    this.nodeForEdit.translations[this.currentLang].links.splice(index, 1);
  }

  addEdge() {
    this.data.label = this.edgeType ? 'No' : 'Yes';
    this.showAddEdge = false;
    this.http.saveEdge(this.data);
    this.callback(this.data);
  }

  addConditional() {
    if (this.conditionalConnection) {
      this.data.dashes = true;
      this.data.color = {
        color: 'green',
        inherit: false
      };
    }
    this.showAddDependency = false;
    this.conditionalConnection = false;
    this.callback(this.data);
  }

  private onDeleteNode(data: any, callback: Function) {
    if (data.nodes.length) {
      this.http.deleteNodes(data.nodes);
    }
    if (data.edges.length) {
      this.http.deleteEdges(data.edges);
    }

    callback(data);
  }

  private onDeleteEdge(data: any, callback: Function) {
    if (data.edges.length) {
      this.http.deleteEdges(data.edges);
    }

    callback(data);
  }

  cancelAddNode() {
    this.showAddNode = false;
    console.log('in cancel add');
    this.callback(null);
  }

  cancelEditNode() {
    this.showEditNode = false;
    this.tabViewIndex = 0;
    this.callback(this.data);
  }

  saveEditNode(shouldClose: boolean) {
    this.showEditNode = !shouldClose;
    if (shouldClose) {
      this.tabViewIndex = 0;
    }

    this.nodeForEdit.group = this.selectedNodeType.id;
    const globalLang = this.translateService.currentLang; // this.currentLang is being changed by inner lang selector
    this.nodeForEdit.label = this.nodeForEdit.translations[globalLang].title;
    this.callback(this.nodeForEdit);
  }

  copyNodes(copyText = true) {
    const nodesEdges = this.getSelectedNodesEdges();
    if (!nodesEdges) {
      return;
    }
    this.insertClonedNodesEdges(nodesEdges, copyText);
  }

  exportNodes() {
    const nodesEdges = this.getSelectedNodesEdges();
    if (!nodesEdges) {
      return;
    }

    const exportModel: ExportModel = {data: nodesEdges, name: this.exportName};
    try {
      this.storage.saveExported(exportModel);
      this.showExportData = false;
    } catch { // keep check for api here it won't blow up
      alert('Name already used!');
    }
    this.exportName = '';
  }

  importNodes() {
    if (!this.selectedExportName) {
      return;
    }
    const data = this.storage.getExportedData(this.selectedExportName.name);
    console.log(data);
    this.insertClonedNodesEdges(data);
    this.showImportData = false;
    this.selectedExportName = null;
  }

  private getSelectedNodesEdges(): MyData {
    const selectedNodeIds = this.network.getSelectedNodes();
    if (selectedNodeIds.length === 0) {
      return;
    }

    const selectedEdgesIds = this.network.getSelectedEdges();
    const nodes = this.networkData.nodes.get(selectedNodeIds);
    const edges = this.networkData.edges.get(selectedEdgesIds);
    return {nodes, edges};
  }

  private insertClonedNodesEdges(nodesEdges: MyData, copyText = true): void {
    const oldNewIdsMap = {};
    const {nodes, edges} = nodesEdges;
    const nodesToSelect = [];
    nodes.forEach(node => {
      const oldId = node.id;
      const cloneNode = {...node, id: null, y: node.y + 20};
      if (!copyText) {
        for (const translation in cloneNode.translations) {
          if (cloneNode.translations.hasOwnProperty(translation)) {
            cloneNode.translations[translation].keywords = [];
            cloneNode.translations[translation].links = [];
          }
        }
      }

      this.networkData.nodes.add(cloneNode);
      nodesToSelect.push(cloneNode.id);
      oldNewIdsMap[oldId] = cloneNode.id;
    });

    // edge copying disabled for now because not needed

    // edges.forEach(edge => {
    //   const newEdge = {...edge, id: null};
    //   if (oldNewIdsMap[edge.from] && oldNewIdsMap[edge.to]) {
    //     newEdge.from = oldNewIdsMap[edge.from];
    //     newEdge.to = oldNewIdsMap[edge.to];
    //     this.networkData.edges.add(newEdge);
    //   }
    // });

    this.network.selectNodes(nodesToSelect, true);
  }

  cancelExport() {
    this.showExportData = false;
    this.exportName = '';
  }

  cancelImport() {
    this.showImportData = false;
    this.selectedExportName = null;
  }

  showImport() {
    this.exportedNames = this.storage.getExportedNames().map(x => ({id: x, name: x}));
    this.selectedExportName = this.exportedNames[0];
    this.showImportData = true;
  }

  private getGraphData() {
    this.http.getGraphData(this.id).subscribe(x => {
      this.title = x.title;
      if (x.data) {
        const myObj = JSON.parse(x.data);


        // this will modify the old records, remove when done !!!
        const nodes = myObj.nodes as MyNode[];
        this.fixNodesLabels(nodes);

        // nodes.forEach(n => {
        //   if (n.translations && n.translations.bg && n.translations.en && n.translations.it) { //already fixed
        //     return;
        //   }
        //   n.translations = {
        //     'bg': {
        //       content: n.content,
        //       keywords: [...n.keywords],
        //       links: [...n.links],
        //       textNavigator: n.textNavigator,
        //       title: n.label,
        //       titleFormat: n.titleFormat
        //     },
        //     'en': {
        //       content: n.content,
        //       keywords: [...n.keywords],
        //       links: [...n.links],
        //       textNavigator: n.textNavigator,
        //       title: n.label,
        //       titleFormat: n.titleFormat
        //     },
        //     'it': {
        //       content: n.content,
        //       keywords: [...n.keywords],
        //       links: [...n.links],
        //       textNavigator: n.textNavigator,
        //       title: n.label,
        //       titleFormat: n.titleFormat
        //     }
        //   };
        //   delete n.content;
        //   delete n.keywords;
        //   delete n.links;
        //   delete n.textNavigator;
        //   delete n.titleFormat;
        // });


        // this will modify the old records, remove when done !!!


        this.networkData = new CustomData(myObj.nodes, myObj.edges);
      } else {
        this.networkData = new CustomData();
      }
      this.fillNetwork();
    });
  }

  textChange(ev: any) {
  }


  editLangChange(event: ILanguage) {
    const langCode = event.shortLang;
    this.utilityService.ensureLangProperty(this.nodeForEdit, langCode);
    this.currentLang = langCode;
  }

  private fixNodesLabels(nodes: MyNode[]) {
    if (nodes) {
      const lang = this.currentLang;
      this.utilityService.ensureLangPropertyMultiple(nodes, lang);
      nodes.forEach(x => {
        x.label = x.translations[lang].title;
      });
    }
  }

  private handleTranslationChange(x: LangChangeEvent) {
    this.currentLang = x.lang;  // needed for dynamic template binding to translation property\
    if (this.networkData) {
      const data = this.networkData.nodes.get();
      this.fixNodesLabels(data);
      this.networkData.nodes.update(data);
      // this.network = new Network(this.networkRef.nativeElement, new CustomData(data, this.networkData.edges.get()), this.initOptions());
      // this.network.redraw();
    }
  }

  private patchNodesWithLinks(x: ILinksTexts) {
    if (x.texts) {
      x.texts.forEach(z => {
        const node = this.networkData.nodes.get(z.id);
        node.translations[z.twoLetterLanguage].linkedContent = z.text;
        node.translations[z.twoLetterLanguage].autoLinks = this.utilityService.sortAutolinks(z.references);
        this.networkData.nodes.update(node);
      });
    }
  }
}
