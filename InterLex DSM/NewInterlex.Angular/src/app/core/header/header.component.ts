import { Component, OnDestroy, OnInit } from '@angular/core';
import { MenuItem } from "primeng/api";
import { TranslateService } from "@ngx-translate/core";
import { Subscription } from "rxjs";
import { AuthService } from 'src/app/admin/services/auth.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit, OnDestroy {

  items: MenuItem[];

  constructor(private translate: TranslateService, public authService: AuthService) {
  }

  subscr: Subscription;

  ngOnInit() {
    const home = 'HEADER.HOME';
    const solve = 'HEADER.SOLVE';
    const create = 'HEADER.CREATE';
    const manage = 'HEADER.MANAGE';
    const admin = 'HEADER.ADMIN';
    this.subscr = this.translate.stream([home, solve, create, manage, admin]).subscribe(res => {
      this.items = [
        { label: res[home], icon: 'pi pi-home', routerLink: 'home' }
        // {label: res[solve], routerLink: 'solve-case'},
        // {label: res[create], routerLink: 'create-master'},
        // {label: res[manage], routerLink: 'graph-list'},
      ];
      if (this.authService.isLoggedIn() && !this.authService.isLimitedAdmin()) {
        this.items.push({ label: res[admin], routerLink: 'admin' });
      }
    });
  }

  ngOnDestroy(): void {
    this.subscr.unsubscribe();
  }

}
