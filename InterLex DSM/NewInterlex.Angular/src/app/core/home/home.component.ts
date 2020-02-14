import { Component, OnInit } from '@angular/core';
import { TranslateService } from "@ngx-translate/core";
import { Subscription } from "rxjs";


@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  allText = true;

  constructor(private translate: TranslateService) { }

  ngOnInit() {
 
  }

  seeMore: boolean = true;

 expandCollapse() {
    if (this.allText === true) {
      this.allText = false;
    } else {
      this.allText = true;
    }
  }
}

