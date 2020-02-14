import {Component, OnInit} from '@angular/core';
import {Router} from "@angular/router";
import { INomenclature, ICreateMaster } from 'src/app/models/common.models';
import { MasterGraphCats } from 'src/app/models/constants';
import { HttpService } from 'src/app/core/services/http.service';

@Component({
  selector: 'app-create-master',
  templateUrl: './create-master.component.html',
  styleUrls: ['./create-master.component.scss']
})
export class CreateMasterComponent implements OnInit {

  masterGraphCategories: INomenclature[] = MasterGraphCats;
  selectedGraphCategory: INomenclature = this.masterGraphCategories[0];
  model: ICreateMaster = {};

  constructor(private router: Router, private http: HttpService) {
  }

  ngOnInit() {
  }

  cancel() {
    this.router.navigate(['graph-list']);
  }

  create() {
    this.model.masterGraphCategory = this.selectedGraphCategory.id;
    this.http.createMaster(this.model).subscribe(x => {
      if (x.success) {
        this.router.navigate(['master-graph', x.id]);
      } else {
        alert(x.message);
      }
    });
  }
}
