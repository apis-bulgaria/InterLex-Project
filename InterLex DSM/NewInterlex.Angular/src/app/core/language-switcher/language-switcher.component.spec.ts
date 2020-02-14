import { TestBed, ComponentFixtureAutoDetect, tick } from "@angular/core/testing";
import { LanguageSwitcherComponent } from './language-switcher.component';
import { TranslateModule, TranslateLoader, TranslateService } from '@ngx-translate/core';
import { By, BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { HttpClientTestingModule, HttpTestingController } from "@angular/common/http/testing";
import { HttpLoaderFactory } from 'src/app/app.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClient } from '@angular/common/http';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { DropdownModule, Dropdown } from 'primeng/primeng';
import { NO_ERRORS_SCHEMA } from '@angular/core';



describe('translate service', () => {
    let http: HttpTestingController;
    let translate: TranslateService;
    beforeEach(() => {
        TestBed.configureTestingModule({
            declarations: [LanguageSwitcherComponent],
            imports: [BrowserModule, FormsModule, BrowserAnimationsModule, HttpClientTestingModule, DropdownModule,
                TranslateModule.forRoot({
                    loader: {
                        provide: TranslateLoader,
                        useFactory: (httpClient: HttpClient) => new TranslateHttpLoader(httpClient),
                        deps: [HttpClient]
                    }
                })],
                providers: [TranslateService, 
                    { provide: ComponentFixtureAutoDetect, useValue: true }
                  ],
                  schemas: [NO_ERRORS_SCHEMA]
        });
        translate = TestBed.get(TranslateService);
        http = TestBed.get(HttpTestingController);
    });

    it('check p-dropdown', () => {
        

        const fixture = TestBed.createComponent(LanguageSwitcherComponent);
        let dd = fixture.debugElement.query(By.css('p-dropdown'));
        expect(fixture).toBeDefined();
        expect(dd).toBeDefined();
        expect(dd.nativeElement.innerText).toEqual("English");
        dd.componentInstance.selectItem({},{label: 'Български', value: 'bg'});
        fixture.detectChanges();
        expect(dd.nativeElement.innerText).toEqual("Български");
        dd.componentInstance.selectItem({},{label: 'Italiano', value: 'it'});
        fixture.detectChanges();
        expect(dd.nativeElement.innerText).toEqual("Italiano");
        fixture.detectChanges();
    });
});