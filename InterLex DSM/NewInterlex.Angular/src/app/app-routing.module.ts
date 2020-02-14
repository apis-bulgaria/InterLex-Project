import {NgModule} from '@angular/core';
import {Routes, RouterModule, ExtraOptions} from '@angular/router';
import {DiagramComponent} from "./diagram/diagram.component";
import {PresentationComponent} from "./presentation/presentation.component";
import {HomeComponent} from './core/home/home.component';
import {LoginComponent} from './admin/login/login.component';
import {AdminComponent} from "./admin/admin.component";
import {AuthGuard} from "./admin/guards/auth.guard";
import {SolveCaseComponent} from "./admin/solve-case/solve-case.component";
import {CreateMasterComponent} from "./admin/create-master/create-master.component";
import {GraphListComponent} from "./admin/graph-list/graph-list.component";
import {MasterGraphComponent} from "./admin/master-graph/master-graph.component";

const routes: Routes = [
  {path: '', redirectTo: '/home', pathMatch: 'full'},
  {component: HomeComponent, path: 'home'},
  {component: DiagramComponent, path: 'diagram/:id'},
  {component: PresentationComponent, path: 'presentation/:id'},
  {component: PresentationComponent, path: 'cases'},
  {component: LoginComponent, path: 'login'},
  {
    path: 'admin',
    component: AdminComponent,
    canActivate: [AuthGuard],
    children: [
      {
        path: '',
        canActivateChild: [AuthGuard],
        children: [
          { path: '', component: SolveCaseComponent },
          { path: 'create-master', component: CreateMasterComponent },
          { component: GraphListComponent, path: 'graph-list' },
          { component: MasterGraphComponent, path: 'master-graph/:id' },
          { component: SolveCaseComponent, path: 'solve-case' },

        ]
      }
    ]
  },
  {path: '**', redirectTo: '/home'},

];

const options: ExtraOptions = {
  onSameUrlNavigation: "reload",
  anchorScrolling: "enabled",
};

@NgModule({
  imports: [RouterModule.forRoot(routes, options)],
  exports: [RouterModule]
})
export class AppRoutingModule {
}
