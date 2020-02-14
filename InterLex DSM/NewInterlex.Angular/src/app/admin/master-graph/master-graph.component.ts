import {Component, OnInit} from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import { IMasterDetailModel } from 'src/app/models/common.models';
import { HttpService } from 'src/app/core/services/http.service';

@Component({
  selector: 'app-master-graph',
  templateUrl: './master-graph.component.html',
  styleUrls: ['./master-graph.component.scss']
})
export class MasterGraphComponent implements OnInit {

  id: string;
  data: IMasterDetailModel;

  constructor(private route: ActivatedRoute, private http: HttpService) {
  }

  ngOnInit() {
    this.route.paramMap.subscribe(map => {
      this.id = map.get('id');
      this.getGraphList();
    });
  }

  private getGraphList() {
    this.http.getGraphList(this.id).subscribe(data => {
      this.data = data;
    });
  }
}
