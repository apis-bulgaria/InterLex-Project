import {MyNode} from "../models/common.models";
import {Component, NO_ERRORS_SCHEMA, OnInit} from '@angular/core';
import {TestBed} from '@angular/core/testing';
import {BreadcrumbsComponent} from './breadcrumbs.component';
import {BrowserModule} from '@angular/platform-browser';
import {BreadcrumbModule} from 'primeng/primeng';
import {FormsModule} from '@angular/forms';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {By} from '@angular/platform-browser';


@Component({
  template: `
    <app-breadcrumbs [nodes]="nodesHistory" [histChanged]="histNodesChanged"
                     (click)="breadcrumbClick($event)"></app-breadcrumbs>`
})
class TestHostComponent implements OnInit {
  public nodesHistory: MyNode[] = [];
  public histNodesChanged: number = 0;

  public breadcrumbClick($event) {
  }

  ngOnInit() {
    this.nodesHistory = [
      {
        id: "ee8b822a-7461-4f92-86ae-a5b3452ed141",
        x: -503,
        y: -33,
        label: "Start",
        group: "startNode",
        links: [{url: "2222", name: "4444", linkType: "IL"}],
        keywords: [],
        content: "<p>dfdff зfdffd </p>"
      },
      {
        id: "dd4e5313-faa4-4e30-9507-284daabb61a5",
        x: -407,
        y: -34,
        label: "Избор на цел",
        group: "taskNode",
        links: [],
        keywords: [],
        content: "<p>Тест дсдсд гртфртртр</p>"
      },
      {
        id: "db3d7297-7dd3-4850-b077-c1f1ad13029b",
        x: -235,
        y: -184,
        label: "Компетентност",
        group: "taskNode",
        links: [],
        keywords: [],
        content: ""
      },
    ];
  }
}

beforeEach(() => {
    TestBed.configureTestingModule(
      {
        declarations: [TestHostComponent, BreadcrumbsComponent],
        imports: [BrowserModule, BreadcrumbModule, FormsModule, BrowserAnimationsModule,],
        schemas: [NO_ERRORS_SCHEMA]
      })
  }
);

describe('BreadCrumbs', () => {
  it('show nodes history', () => {
    const hostFixture = TestBed.createComponent(TestHostComponent);
    hostFixture.autoDetectChanges();
    const bc = hostFixture.debugElement.query(By.css('app-breadcrumbs'));
    hostFixture.componentInstance.histNodesChanged++;
    hostFixture.detectChanges();
    expect(bc.componentInstance._nodes.length).toEqual(3);
    const ul = bc.query(By.css('ul'));
    expect(ul.nativeElement.innerText).toEqual('StartИзбор на целКомпетентност');

    bc.componentInstance.click.subscribe((id: string) => {
      console.log(id);
      expect(id).toBe('dd4e5313-faa4-4e30-9507-284daabb61a5')
    });
    bc.componentInstance.clicked('dd4e5313-faa4-4e30-9507-284daabb61a5');
  });
});
