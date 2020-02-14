import { Component, OnInit } from '@angular/core';
import { MenuItem } from "primeng/api";
import { Router } from '@angular/router';
import { AuthService } from './services/auth.service';
import { Subscription } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
export class AdminComponent implements OnInit {
  items: MenuItem[];
  subscr: Subscription;
  constructor(public authService: AuthService, public router: Router, private translate: TranslateService) {
    const home = 'HEADER.HOME';
    const solve = 'HEADER.SOLVE';
    const create = 'HEADER.CREATE';
    const manage = 'HEADER.MANAGE';
    this.subscr = this.translate.stream([home, solve, create, manage]).subscribe(res => {
      this.items = [
        {label: res[home], icon: 'pi pi-home', routerLink: '/home'},
        {label: res[solve], routerLink: 'solve-case'},
        {label: res[create], routerLink: 'create-master'},
        {label: res[manage], routerLink: 'graph-list'}
      ];
    });
  }

  ngOnInit() {
  }
  logout() {
    this.authService.logout();
    this.router.navigate(['./login']);
  }
  ngOnDestroy(): void {
    this.subscr.unsubscribe();
  }
}
