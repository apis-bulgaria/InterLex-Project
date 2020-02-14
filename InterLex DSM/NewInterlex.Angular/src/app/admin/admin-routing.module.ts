import {NgModule} from '@angular/core';
import {Routes, RouterModule} from '@angular/router';
import {AdminComponent} from './admin.component';
import {AuthGuard} from './guards/auth.guard';
import {AuthService} from './services/auth.service';
import {CreateMasterComponent} from './create-master/create-master.component';
import {GraphListComponent} from './graph-list/graph-list.component';
import {MasterGraphComponent} from './master-graph/master-graph.component';
import {SolveCaseComponent} from './solve-case/solve-case.component';

const adminRoutes: Routes = [
  {
    path: 'admin',
    component: AdminComponent,
    canActivate: [AuthGuard],
    children: [
      {
        path: '',
        canActivateChild: [AuthGuard],
        children: [
          {path: '', component: SolveCaseComponent},
          {path: 'create-master', component: CreateMasterComponent},
          {component: GraphListComponent, path: 'graph-list'},
          {component: MasterGraphComponent, path: 'master-graph/:id'},
          {component: SolveCaseComponent, path: 'solve-case'},
        ]
      }
    ]
  }
];

@NgModule({
  imports: [
    // RouterModule.forChild(adminRoutes)
  ],
  exports: [
    RouterModule
  ],
  providers: [
    AuthGuard,
    AuthService
  ]
})
export class AdminRoutingModule {
}
