import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
//module
import { AppRoutingModule } from './app-routing.module';
import { MenubarModule } from "primeng/menubar";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { HttpClient, HttpClientModule, HTTP_INTERCEPTORS } from "@angular/common/http";
import { ToastModule } from 'primeng/toast';
import {
  ButtonModule,
  DialogModule,
  BreadcrumbModule,
  CardModule,
  CheckboxModule,
  DropdownModule,
  EditorModule,
  InputSwitchModule,
  InputTextModule, KeyFilterModule, PanelModule, RadioButtonModule,
  TabViewModule,
  MessageService, ProgressBarModule, OverlayPanelModule, InputTextareaModule
} from "primeng/primeng";
import { TableModule } from "primeng/table";
import { ToggleButtonModule } from 'primeng/togglebutton';
import { SplitButtonModule } from 'primeng/splitbutton';
//pipes
import { GraphTypePipe } from './core/pipes/graph-type.pipe';
import { MasterCategoryPipe } from "./core/pipes/master-category.pipe";

//components
import { AppComponent } from './app.component';
import { HeaderComponent } from './core/header/header.component';
import { DiagramComponent } from "./diagram/diagram.component";
import { PresentationComponent } from "./presentation/presentation.component";
import { GraphListComponent } from './admin/graph-list/graph-list.component';
import { MasterGraphComponent } from './admin/master-graph/master-graph.component';
import { SolveCaseComponent } from "./admin/solve-case/solve-case.component";
import { LanguageSwitcherComponent } from './core/language-switcher/language-switcher.component';
import { TranslateLoader, TranslateModule } from "@ngx-translate/core";
import { TranslateHttpLoader } from "@ngx-translate/http-loader";
import { BreadcrumbsComponent } from './breadcrumbs/breadcrumbs.component';
import { HomeComponent } from './core/home/home.component';
import { FooterComponent } from './core/footer/footer.component';


import { AlertComponent } from './core/alert/alert.component';

import { AdminModule } from './admin/admin.module';
import { CreateMasterComponent } from './admin/create-master/create-master.component';
import { LoginComponent } from './admin/login/login.component';
import {TokenInterceptor} from './core/interceptors/token.interceptor';
import { ErrorInterceptor } from './core/interceptors/error.interceptor';
import {AdminComponent} from "./admin/admin.component";
import { AutolinkfilterPipe } from './core/pipes/autolinkfilter.pipe';
import {SafeHtmlPipe} from "./core/pipes/safe-html.pipe";
import {ProgressComponent} from "./core/progress/progress.component";
export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/', '.json');
}

@NgModule({
  declarations: [
    AppComponent,
    DiagramComponent,
    HeaderComponent,
    PresentationComponent,
    GraphListComponent,
    MasterGraphComponent,
    GraphTypePipe,
    MasterCategoryPipe,
    SafeHtmlPipe,
    SolveCaseComponent,
    LanguageSwitcherComponent,
    BreadcrumbsComponent,
    HomeComponent,
    FooterComponent,
    AlertComponent,
    CreateMasterComponent,
    LoginComponent,
    AdminComponent,
    AutolinkfilterPipe,
    ProgressComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BreadcrumbModule,
    FormsModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    MenubarModule,
    ProgressBarModule,
    ButtonModule,
    DialogModule,
    DropdownModule,
    InputTextModule,
    InputTextareaModule,
    InputSwitchModule,
    TabViewModule,
    EditorModule,
    HttpClientModule,
    OverlayPanelModule,
    CheckboxModule,
    CardModule,
    RadioButtonModule,
    PanelModule,
    KeyFilterModule,
    TableModule,
    ToggleButtonModule,
    ToastModule,
    SplitButtonModule,
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: (HttpLoaderFactory),
        deps: [HttpClient]
      }
    }),
    AdminModule
  ],
  providers: [MessageService,
    {provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true},
    {provide: HTTP_INTERCEPTORS, useClass: TokenInterceptor, multi: true}],
  bootstrap: [AppComponent]
})
export class AppModule { }
