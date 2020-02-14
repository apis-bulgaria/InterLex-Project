import { Component, OnInit } from '@angular/core';
import { IMasterGraphListModel } from 'src/app/models/common.models';
import { HttpService } from 'src/app/core/services/http.service';

@Component({
  selector: 'app-graph-list',
  templateUrl: './graph-list.component.html',
  styleUrls: ['./graph-list.component.scss']
})
export class GraphListComponent implements OnInit {

  data: IMasterGraphListModel[];
  constructor(private http: HttpService) { }

  ngOnInit() {
    this.getGraphList();
  }


  private getGraphList() {
    this.http.getAllGraphs().subscribe(data => {
      if (data.success) {
        this.data = data.masterGraphs;
      }
    });
  }
}
