import {Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {StorageService} from "../core/services/storage.service";
import {Edge, IdType, Network, Options} from "vis";
import {
  CustomData,
  IOldNomenclature,
  MyNode,
  ILink,
  ILinkType,
  TranslateFields,
  IReference, ILinkRequest, MyData, IGraphData, ReportPair, Conclusion, Report
} from "../models/common.models";
import {ActivatedRoute} from "@angular/router";
import {HttpService} from "../core/services/http.service";
import {map, mergeMap} from "rxjs/operators";
import {LangChangeEvent, TranslateService} from "@ngx-translate/core";
import {AuthService} from '../admin/services/auth.service';
import {UtilityService} from "../core/services/utility.service";
import {AlertService} from "../core/services/alert.service";
import {OverlayPanel} from "primeng/overlaypanel";
import {Subscription} from "rxjs";
import {MenuItem} from 'primeng/api';

declare var $: any;

@Component({
  selector: 'app-presentation',
  templateUrl: './presentation.component.html',
  styleUrls: ['./presentation.component.scss']
})
export class PresentationComponent implements OnInit, OnDestroy {

  constructor(private storage: StorageService, private route: ActivatedRoute, private http: HttpService,
              private translateService: TranslateService, public authService: AuthService,
              private utilityService: UtilityService, private alertService: AlertService) {
    this.isOfficialPresentation = this.route.routeConfig.path === 'cases';
  }

  currentLang: string;
  // @ViewChild('visContainer', {static: true}) networkRef;
  network: Network;
  selectedNode: MyNode;
  prevButtonDisabled = true;
  nextButtonDisabled = false;
  radioYesNo: 'yes' | 'no';
  nodesHistory: MyNode[] = [];
  showSelection = false;
  data: CustomData;
  radioOptions: IOldNomenclature[];
  selectedOptionId: string;
  id: string;
  title: string;
  reports: Report[] = [];
  conclusionType:string;
  displayComment: boolean = false;
  commentStr = '';
  displayReference: boolean = false;
  editedLinks: ILink[];

  showReport: boolean = false;
  exportItems: MenuItem[];

  linkTypes: ILinkType[];
  selectedLinkType: ILinkType;
  nodeForEdit: MyNode = {};

  histNodesChanged: number = 0;
  linkHint: any;
  subscriptions: Subscription = new Subscription();
  loggedIn: boolean;
  limitedAdmin: boolean;
  isOfficialPresentation: boolean; // view for clients, meaning noone is logged in
  officialMainDiagramId = '91407d41-adfe-421b-bc59-91cb1b5cace5'; // default id for "cases" route path - this is mastergraph id!
  officialApplicableLawId = 'f515a2ea-7179-46b5-84dc-a029fa6b7d34'; // this is concrete graph id NOT mastergraph
  officialJurisdictionId = '2e39d873-a22b-4f9d-b17b-b67ce05f709c'; //  this is concrete graph id NOT mastergraph
  jurMainNodeId = '1f5208e0-d6cf-4e6a-aaa0-8ee64983f491'; // this and one below not needed for now, but leaving just in case
  lawMainNodeId = 'dab9bc73-4ac6-4441-8b3e-ceffcc786096';
  italianResJur: any;
  italianResLaw: any;

  ngOnInit() {

    this.exportItems = [
      {
        label: 'pdf',
        // icon: 'pi pi-refresh',
        command: () => {
          this.exportReport('pdf');
        }
      },
      {
        label: 'rtf',
        // icon: 'pi pi-times',
        command: () => {
          this.exportReport('rtf');

        }
      }
    ];

    this.currentLang = this.translateService.currentLang || 'en';
    this.translateService.onLangChange.subscribe((x: LangChangeEvent) => {
      this.handleTranslationChange(x);
    });
    this.limitedAdmin = this.authService.isLimitedAdmin();

    const authSubscr = this.authService.getLoggedInStatus().subscribe(x => {
      if (this.isOfficialPresentation) { // official presentation path will display merged graph content so it should be readonly
        this.loggedIn = false;
      } else {
        this.loggedIn = x;
      }
    });
    this.subscriptions.add(authSubscr);

    this.route.paramMap
      .pipe(
        mergeMap(x => {
          const pathId = this.isOfficialPresentation ? this.officialMainDiagramId : x.get('id');
          return this.http.getGraphList(pathId);
        }),
        map(x => x.graphs[0].id)  // this gets first one, change as needed
      ).subscribe(coreId => {
      this.id = coreId;
      this.getGraphData();
    });

    this.http.getMetaInfo().subscribe(info => {
      this.linkTypes = info.linkTypes;
      this.selectedLinkType = info.linkTypes[0];
    });
    // this.data = this.storage.networkData;
    // const options = this.initOptions();
    // if (this.data) {
    //   this.network = new Network(this.networkRef.nativeElement, this.data, options);
    //   this.findStartNode();
    // }
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  private initOptions() {
    const options: Options = {
      layout: {
        hierarchical: {
          enabled: true,
          direction: 'LR',
          nodeSpacing: 100,
          levelSeparation: 250
        }
      },
      physics: {
        stabilization: {
          enabled: true,
          iterations: 1200
        }
      },
      // animation: { can't find this
      //   duration: 2500,
      //   easingFunction: "easeInOutQuad"
      // },
      // smoothCurves: false,
      // physics: {
      //   barnesHut: {
      //     enabled: false,
      //   },
      // },
      interaction: {
        navigationButtons: true,
        keyboard: true,
      },
      edges: {
        arrows: 'to',
        smooth: {
          enabled: true,
          forceDirection: 'none',
          roundness: 0.8,
          type: 'horizontal'
        },
        color: '#666666'
      },
      nodes: {
        widthConstraint: {
          minimum: 0,
          maximum: 250
        },
      },
      groups: {
        startNode: {
          shape: 'box',
          color: {
            background: "#EB7DF4",
            border: "#C4042D",
            highlight: {
              background: "#F0B3F5",
              border: "#C4042D",
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
    };
    return options;
  }

  private findStartNode() {
    const node = this.data.getNodesByGroup('startNode')[0];
    if (node) {
      this.setSelectedNode(node);
      // this.focusNetwork();
      this.saveNodeInHistory(node);
      this.checkForMultiRadioOptions();
    } else {
      alert('Error. No start node!');
    }
  }

  movePrevNode() {
    this.removeNodeAfter(this.selectedNode.id);
    this.nodesHistory.pop();
    let previous = this.nodesHistory.pop();
    while (previous.isHidden) {  // skip hidden nodes, might need change if list of steps will be displayed
      previous = this.nodesHistory.pop();
    }

    this.selectNode(previous.id);
    this.histNodesChanged++;
    // this.focusNetwork();
  }

  moveNextNode() {
    this.removeNodeAfter(this.selectedNode.id);
    const id = this.findNextNodeId(this.selectedNode);
    this.selectNode(id);
    this.histNodesChanged++;
    if (this.limitedAdmin) {
      this.callItalianApi();
    }
    // console.log(this.selectedNode);
    // console.log(this.nodesHistory);
    // this.focusNetwork();
  }


  // here is the connection with the Reasoner Module implemented by the Italian partners
  private callItalianApi() {
    const ids = this.nodesHistory.map(x => x.id.toString());
    this.http.getItalianApiResults(ids).subscribe(res => {
      if (res) {
        this.italianResLaw = res.law ? JSON.parse(res.law) : null;
        this.italianResJur = res.law ? JSON.parse(res.jur) : null;
      } else {
        this.italianResLaw = this.italianResJur = null;
      }
      console.log(res);
    });
  }

  private findNextNodeId(node: MyNode) { // errors if incorrect ending with non endNode - no next edge
    const connectedEdges = this.data.getOutgoingEdgesById(node.id);
    // check for only one path - immediately move that way?


    let relevantEdge = connectedEdges.filter(x => !x.dashes)[0]; // this is needed to avoid selecting conditional edge by default
    if (node.group === "decisionNode") { // assume decision nodes cannot be conditional
      relevantEdge = connectedEdges.find(x => x.label === this.radioYesNo);
    } else if (connectedEdges.length > 1) {
      // here find conditional nodes and then check conditions for them
      const conditionalEdges = this.getConditionalEdges(connectedEdges);
      const conditionalLen = conditionalEdges.length || 0;
      if (connectedEdges.length - conditionalLen > 1) { // multiselect node case
        return this.selectedOptionId;
      }

      if (conditionalEdges.length) {
        const confirmedConditionalEdge = this.checkConditionalEdges(conditionalEdges);
        if (confirmedConditionalEdge) {
          relevantEdge = confirmedConditionalEdge;
        }
      }
    }
    return relevantEdge.to;
  }

  private focusNetwork() {
    this.network.focus(this.selectedNode.id, {scale: 0.6});
  }

  private selectNode(id: IdType) {

    let nextNode = this.data.nodes.get(id); // this logic is needed to facilitate skipping hidden nodes

    this.saveNodeInHistory(nextNode);
    while (nextNode.isHidden) {
      const nextId = this.findNextNodeId(nextNode);
      nextNode = this.data.nodes.get(nextId);
      this.saveNodeInHistory(nextNode);
    }
    if (nextNode.isReport) {
      this.createReport();
    }
    this.setSelectedNode(nextNode);
    this.nextButtonDisabled = this.selectedNode.group === 'endNode';
    this.prevButtonDisabled = this.selectedNode.group === 'startNode';
    this.showSelection = false;
    if (!this.nextButtonDisabled) {
      this.checkForMultiRadioOptions();
    }
  }

  private removeNodeAfter(id) {
    let ind = this.nodesHistory.findIndex(x => x.id == id);
    if (ind > -1) {
      this.nodesHistory.splice(ind, this.nodesHistory.length - ind - 1);
      this.histNodesChanged++;
    }
  }

  private saveNodeInHistory(node: MyNode) {
    this.nodesHistory.push(node);
  }

  private getConditionalEdges(connectedEdges: Edge[]): Edge[] {
    return connectedEdges.filter(x => x.dashes === true);
  }

  private checkConditionalEdges(conditionalEdges: Edge[]): Edge | void {
    if (conditionalEdges.length === 0) {
      return;
    }
    for (const conditionalEdge of conditionalEdges) {
      const target = conditionalEdge.to;
      const targetNode = this.data.nodes.get(target);
      const targetNodeEdges = this.data.getOutgoingEdgesById(targetNode.id);
      const secondaryConditionalEdges = this.getConditionalEdges(targetNodeEdges);
      if (secondaryConditionalEdges.length) {
        for (const secondaryEdge of secondaryConditionalEdges) {
          if (this.nodesHistory.find(x => x.id === secondaryEdge.to)) {
            return conditionalEdge;
          }
        }
      }
    }
  }

  private checkForMultiRadioOptions() {
    const connectedEdges = this.data.getOutgoingEdgesById(this.selectedNode.id);
    const regularEdges = connectedEdges.filter(x => !x.dashes);
    if (regularEdges.length > 1 && this.selectedNode.group !== 'decisionNode') { // multi
      const ids = regularEdges.map(x => x.to);
      const nodes = this.data.nodes.get(ids);
      this.utilityService.ensureLangPropertyMultiple(nodes, this.currentLang);
      this.radioOptions = nodes.sort((a, b) => a.order - b.order).map(n => ({
        name: n.translations[this.currentLang].title,
        id: n.id.toString()
      }));
      this.selectedOptionId = this.radioOptions[0].id;
      this.showSelection = true;
    }
  }

  private getGraphData() {
    if (this.isOfficialPresentation) { // this gets all three graphs - merged one, applicable law and jurisdiction
      this.getMergeGraphs();
    } else {
      this.http.getGraphData(this.id).subscribe(x => {
        this.title = x.title;
        if (x.data) {
          const myObj = JSON.parse(x.data);
          this.data = new CustomData(myObj.nodes, myObj.edges);
        } else {
          this.data = new CustomData();
        }
        this.fillNetwork();
      });
    }
  }

  private getMergeGraphs() {
    this.http.getMultipleGraphData(this.id, this.officialJurisdictionId, this.officialApplicableLawId).subscribe(x => {
      const [mainData, jurData, lawData] = x;
      this.title = mainData.title;
      const myObj = JSON.parse(mainData.data);
      this.data = new CustomData(myObj.nodes, myObj.edges);
      this.insertFixedSecondaryNodesEdges(jurData, 'jur');
      this.insertFixedSecondaryNodesEdges(lawData, 'law');
      this.fillNetwork();

    });
  }

  private insertFixedSecondaryNodesEdges(nodesEdges: IGraphData, type: 'jur' | 'law'): void {
    const oldNewIdsMap = {};
    const myObj = JSON.parse(nodesEdges.data) as { nodes: MyNode[], edges: Edge[] };
    const {nodes, edges} = myObj;
    let startNode: MyNode;
    nodes.forEach(node => {
      const oldId = node.id;
      node.id = null;
      if (node.group === "startNode") {
        startNode = node;
        node.group = "taskNode";
      }
      this.data.nodes.add(node);
      oldNewIdsMap[oldId] = node.id;
    });

    const edge: Edge = {id: null, to: startNode.id}; // this will be not needed if top level nodes get moved to secondary diagrams

    // this changes, keeping for testing
    // edge.from = type === "jur" ? this.jurMainNodeId: this.lawMainNodeId;
    const start = this.data.getNodesByGroup('startNode')[0];
    edge.from = start.id;

    // end of changes
    this.data.edges.add(edge);

    edges.forEach(edge => {
      edge.id = null;
      if (oldNewIdsMap[edge.from] && oldNewIdsMap[edge.to]) {
        edge.from = oldNewIdsMap[edge.from];
        edge.to = oldNewIdsMap[edge.to];
        this.data.edges.add(edge);
      }
    });

    //add edge from second nodes in main to startNodes here
  }

  private fillNetwork() {
    // const options: Options = this.initOptions();
    // this.network = new Network(this.networkRef.nativeElement, this.data, options);
    this.findStartNode();
  }

  showDialogComment() {
    this.commentStr = this.selectedNode.translations[this.currentLang].content;
    this.displayComment = true;
  }

  showDialogReferences() {
    this.editedLinks = [...this.selectedNode.translations[this.currentLang].links];
    this.displayReference = true;
  }

  public saveCommentAndClose() {
    this.displayComment = false;
    const currentTranslation = this.selectedNode.translations[this.currentLang];
    currentTranslation.content = this.commentStr;
    const linkRequst: ILinkRequest = {
      id: this.selectedNode.id,
      text: this.commentStr,
      twoLetterLanguage: this.currentLang
    };
    this.http.insertLinks([linkRequst], this.id).subscribe({
      next: resp => {
        if (resp && resp.texts && resp.texts[0]) {
          const linkItem = resp.texts[0];
          currentTranslation.linkedContent = linkItem.text;
          currentTranslation.autoLinks = this.utilityService.sortAutolinks(linkItem.references);
        }
      },
      complete: () => {
        this.data.nodes.update(this.selectedNode, this.selectedNode.id);
        this.saveData();
      }
    });
  }

  public saveLinksAndClose() {
    this.displayReference = false;
    this.selectedNode.translations[this.currentLang].links = [];
    this.selectedNode.translations[this.currentLang].links = [...this.editedLinks];
    this.data.nodes.update(this.selectedNode, this.selectedNode.id);

    this.saveData();
  }

  private saveData() {
    this.http.saveDiagram(this.data, this.id).subscribe(x => {
      this.alertService.success('Saved successfully');
    });
  }

  removeLink(index: number) {
    this.editedLinks.splice(index, 1);
  }

  addLink() {
    this.editedLinks = [...this.editedLinks, {
      url: '',
      name: '',
      linkType: this.selectedLinkType.shortName
    }];
  }

  public breadcrumbClick(id: string) {
    console.log(id);
    this.selectNode(id);
  }

  private setSelectedNode(node: MyNode) {
    this.utilityService.ensureLangProperty(node, this.currentLang);
    this.selectedNode = node;
  }

  private handleTranslationChange(x: LangChangeEvent) {
    const newLang = x.lang;
    if (this.selectedNode) {
      this.utilityService.ensureLangProperty(this.selectedNode, newLang);
    }
    this.currentLang = newLang;  // needed for dynamic template binding to translation property
    if (this.selectedNode) { // don't put this in upper if
      this.checkForMultiRadioOptions();
    }

    if (this.selectedNode && this.selectedNode.isReport) {
      this.createReport(); // if on report node needed to translate the report
    }

  }

  getLinkHint(autolink: IReference, op: OverlayPanel, event) {
    const url = autolink.hintUrl;
    this.http.getText(url).subscribe(html => {
      const $that = this;
      this.linkHint = html;

      $(event.target).qtip({
        style: {
          classes: 'qtip-light qtip-shadow qtip-rounded'
        },
        overwrite: false, // Make sure another tooltip can't overwrite this one without it being explicitly destroyed
        content: $that.linkHint,
        position: {
          at: 'bottom center', // Position the tooltip above the link
          my: 'top center',
          viewport: $(window), // Keep the tooltip on-screen at all times
          effect: false // Disable positioning animation,
        },
        show: {
          solo: true, // Only show one tooltip at a time
          delay: 100,
          ready: true,
          when: false
        },
        hide: {
          when: {event: 'mouseout unfocus'},
          fixed: true,
          delay: 300
        }
      });

      //  op.show(event);
    });
  }

  showAnswer() {
    if (this.nextButtonDisabled) {

    }
  }

  contentHint(ev: MouseEvent, op: OverlayPanel) {
    const target = ev.target as HTMLElement;
    const tagName = target.tagName.toLowerCase();
    if (tagName !== 'a') {
      return;
    }
    // const url = target.getAttribute('data-apis-hint-url');
    const url = target.dataset['apisHintUrl']; // alternative to upper
    if (url) {
      this.http.getText(url).subscribe(html => {
        const $that = this;
        this.linkHint = html;

        $(target).qtip({
          style: {
            classes: 'qtip-light qtip-shadow qtip-rounded'
          },
          overwrite: false, // Make sure another tooltip can't overwrite this one without it being explicitly destroyed
          content: $that.linkHint,
          position: {
            at: 'bottom center', // Position the tooltip above the link
            my: 'top center',
            viewport: $(window), // Keep the tooltip on-screen at all times
            effect: false // Disable positioning animation,
          },
          show: {
            solo: true, // Only show one tooltip at a time
            delay: 100,
            ready: true,
            when: false
          },
          hide: {
            when: {event: 'mouseout unfocus'},
            fixed: true,
            delay: 300
          }
        });

        //  op.show(ev);
      });
    }
  }

  private createReport() {
    const doubleReport = this.nodesHistory.filter(x => x.isReport).length > 1;
    const indexRef = {index: 1};
    if (doubleReport) {
      const firstReportIndex = this.nodesHistory.findIndex(x => x.isReport);
      const firstHistory = this.nodesHistory.slice(2, firstReportIndex + 1);
      const firstRep = this.getSingleReport(firstHistory, indexRef);
      const secondHistory = this.nodesHistory.slice(firstReportIndex + 1);
      const secondRep = this.getSingleReport(secondHistory, indexRef);
      this.reports = [firstRep, secondRep];
    } else {
      const history = this.nodesHistory.slice(2); // removing the initial branch selection question and answer
      const rep = this.getSingleReport(history, indexRef);
      this.reports = [rep];
    }
  }

  private getSingleReport(nodes: MyNode[], indexRef: { index: number }): Report {
    const qaPairs = [];
    for (let i = 0; i < nodes.length - 2; i++) {
      const currentNodeVisible = !nodes[i].isHidden; // question
      const nextNodeHidden = nodes[i + 1].isHidden; // answer, this doesn't work in some cases
      if (currentNodeVisible && nextNodeHidden) {
        qaPairs.push({
          index: indexRef.index,
          question: nodes[i].translations[this.currentLang].title,
          answer: nodes[i + 1].translations[this.currentLang].reportDisplay || nodes[i + 1].translations[this.currentLang].title
        });
        indexRef.index++;
      }
    }
    const reportNode = nodes[nodes.length - 1];
    const translation = reportNode.translations[this.currentLang];
    const conclusion = {
      title: translation.titleFormat || translation.title,
      legalBasis: translation.legalBasis,
      reportDisplay: translation.reportDisplay
    };
    const conclusionType = this.determineTypeOfConclusion(translation.title);
    this.conclusionType = conclusionType;
    const report: Report = {
      pairs: qaPairs,
      conclusion: conclusion,
      aboutCaseTranslation: this.translateService.instant('SOLVE.OVERVIEW'),
      caseReportTranslation: this.translateService.instant('SOLVE.CASEREPORT'),
      legalBasisTranslation: this.translateService.instant('SOLVE.LEGALBASIS'),
      conclusionTranslation: this.translateService.instant(conclusionType) // this will be based on component level field
    };
    return report;
  }

  private determineTypeOfConclusion(title: string): string {  // very bad but no additional info available
    const lower = title.toLowerCase();
    if (lower.includes('applicab') || lower.includes('приложим') || lower.includes('прилага')) {
      return 'SOLVE.CONCLUSIONLAW';
    } else {
      return 'SOLVE.CONCLUSIONJUR';
    }
  }

  showCaseReport() {
    this.showReport = true;

  }

// FIX THIS IN API TO TAKE ARRAY AND FOREACH ITEMS IN HTML CREATION
  private exportReport(type: 'pdf' | 'rtf') {
    this.http.exportReport(type, this.reports).subscribe(response => {
      let dataType = response.body.type;
      let binaryData = [];
      binaryData.push(response.body);
      let downloadLink = document.createElement('a');
      downloadLink.href = window.URL.createObjectURL(new Blob(binaryData, {type: dataType}));
      downloadLink.download = `Report.${type}`;
      // if (filename)
      //   downloadLink.setAttribute('download', filename);
      document.body.appendChild(downloadLink);
      downloadLink.click();
      downloadLink.remove();
    });
  }
}
