import {Component, OnInit} from '@angular/core';
import {
  euLanguages,
  ICaseMetaContentResponse, IExpertContentResponse,
  IExpertMaterial, IFile, ILicense,
  IMetaEditor,
  INomenclature,
  ISource
} from "../../models/case-editor.model";
import {AlertService} from "../../core/services/alert.service";
import {HttpService} from "../../core/services/http.service";
import {ActivatedRoute, Router} from "@angular/router";
import {ConfirmationService, MenuItem, SelectItem} from "primeng/api";
import {v4 as uuid} from 'uuid';
import {LanguageModel} from "../../models/language.model";
import {CacheService} from "../../core/services/cache.service";
import {HttpResponse} from "@angular/common/http";
import {fileFormats} from "../../models/file-formats.model";

@Component({
  selector: 'app-expert',
  templateUrl: './expert.component.html',
  styleUrls: ['./expert.component.css'],
  providers: [ConfirmationService]
})
export class ExpertComponent implements OnInit {

  id: string;
  model: IExpertMaterial = {
    publisher: [],
    corporateAuthor: [],
    author: [],
    sources: [],
    keywords: [],
    issn: [],
    isbn: [],
    doi: [],
    languages: [],
    formats: []
  };
  submitted = false;
  editable = true;
  categories: INomenclature[] = [{
    label: 'Articles', code: 'A'
  }, {
    label: 'Books and studies', code: 'B'
  }, {
    label: 'Training Materials and Guidelines', code: 'T'
  }, {
    label: 'E-learning an Other Useful Resources', code: 'E'
  }];
  showAddAuthor = false;
  showAddCorpAuthor = false;
  showAddPublisher = false;
  showAddSource = false;
  showAddFormat = false;
  showAddKeyword = false;
  showAddIssn = false;
  showAddIsbn = false;
  showAddFile = false;
  showAddDoi = false;
  showAddLanguage = false;
  author: string;
  corpAuthor: string;
  publisher: string;
  issn: string;
  isbn: string;
  doi: string;
  source: ISource = {};
  editSourceIndex: number = null;
  keyword: string;
  fileLanguage: LanguageModel;
  formats: string[];
  languages: LanguageModel[];
  currentFile: IFile;
  files: IFile[] = [];
  tempLanguages: string[] = [];
  tempFormats: string[] = [];
  otherFormat: string;
  yearsRange: SelectItem[] = Array.from(Array(30).keys()) // generate last 30 years for dropdown
    .map(x => (new Date().getFullYear() - x).toString())
    .map(x => ({value: x, label: x}));
  licenses: ILicense[] = [
    {title: 'Attribution', label: 'CC BY', value: 'https://creativecommons.org/licenses/by/4.0/'},
    {title: 'Attribution-ShareAlike', label: 'CC BY-SA', value: 'https://creativecommons.org/licenses/by-sa/4.0/'},
    {title: 'Attribution-NoDerivs', label: 'CC BY-ND', value: 'https://creativecommons.org/licenses/by-nd/4.0/'},
    {title: 'Attribution-NonCommercial', label: 'CC BY-NC', value: 'https://creativecommons.org/licenses/by-nc/4.0/'},
    {
      title: 'Attribution-NonCommercial-ShareAlike',
      label: 'CC BY-NC-SA',
      value: 'https://creativecommons.org/licenses/by-nc-sa/4.0/'
    },
    {
      title: 'Attribution-NonCommercial-NoDerivs',
      label: 'CC BY-NC-ND',
      value: 'https://creativecommons.org/licenses/by-nc-nd/4.0/'
    },
    {title: 'Other', label: 'Other', value: null}];

  constructor(private alertService: AlertService, private http: HttpService, private route: ActivatedRoute,
              private router: Router, private confirmation: ConfirmationService, private cache: CacheService) {
    this.route.paramMap.subscribe((params) => {
      this.id = params.get('id');
      if (this.id) {
        this.initModelFromApi();
      }
    });
    this.cache.getEuLanguages().subscribe(x => this.languages = x);
  }

  ngOnInit() {
    this.formats = fileFormats;
  }


  submitForm(needToClose: boolean) {
    this.submitted = true;
    if (!this.validateRequiredFields()) {
      return;
    }
    const content = JSON.stringify(this.model);
    const title = this.model.title;
    const files = this.files;
    const body = {
      title, content, files
    };
    const id = this.route.snapshot.params.id;
    if (id) {
      this.http.post(`case/EditExpert/${id}`, body).subscribe(x => {
        this.alertService.success('Ok');
        if (needToClose) {
          this.router.navigate(['expertlist']);
        }
      }, (error) => {
        this.alertService.error('Error');
      });
    } else {
      this.http.post('case/SaveExpert', body).subscribe(idInserted => {
        this.alertService.success('Ok');
        if (needToClose) {
          this.router.navigate(['expertlist']);
        } else {
          this.router.navigate([`expertlist/${idInserted}`]);
        }
      }, (error) => {
        this.alertService.error('Error');
      });
    }
  }

  validateRequiredFields() {
    if (!this.model.category) {
      // if (!this.mod.title || !this.mod.keywords || !this.mod.summary || !this.mod.jurisdiction
      //   || !this.mod.court || !this.mod.dateOfDocument || !this.mod.language || !this.mod.decisionType
      // )

      this.alertService.error('Please select a category!');
      return false;
    }
    // if (this.mod.keywords.length === 0) {
    //   this.alertService.error('Please fill all mandatory fields!');
    //   return false;
    // }
    return true;
  }

  addAuthor() {
    if (this.author) {
      this.model.author.push(this.author);
      this.showAddAuthor = false;
    }
  }

  addAuthorClick() {
    this.author = '';
    this.showAddAuthor = true;
  }

  addCorpAuthor() {
    if (this.corpAuthor) {
      this.model.corporateAuthor.push(this.corpAuthor);
      this.showAddCorpAuthor = false;
    }
  }

  addCorpAuthorClick() {
    this.corpAuthor = '';
    this.showAddCorpAuthor = true;
  }

  addPublisherClick() {
    this.publisher = '';
    this.showAddPublisher = true;
  }

  addPublisher() {
    if (this.publisher) {
      this.model.publisher.push(this.publisher);
      this.showAddPublisher = false;
    }
  }

  addSourceClick() {
    this.source = {};
    this.editSourceIndex = null;
    this.showAddSource = true;
  }

  addSource() {
    if (this.editSourceIndex !== null) { // editing
      this.source.url = this.source.url ? this.fixUrl(this.source.url) : this.source.url;
      this.model.sources[this.editSourceIndex] = {...this.source};
    } else if (this.source.name) {
      this.source.url = this.source.url ? this.fixUrl(this.source.url) : this.source.url;
      this.model.sources.push(this.source);
    }
    this.showAddSource = false;
  }

  fixUrl(url: string): string {
    if (url) {
      const fixed = url.startsWith('http') ? url : 'http://' + url;
      return fixed;
    }
  }

  private initModelFromApi() {
    const id = this.id;
    this.editable = false;
    this.http.get('./case/GetExpertContent/' + id).subscribe((response: IExpertContentResponse) => {
      this.editable = response.editable;
      const data = JSON.parse(response.content) as IExpertMaterial;
      this.model = {
        ...data,
        publicationDate: data.publicationDate == null ? null : new Date(data.publicationDate),
        languages: data.languages ? data.languages : [],
        formats: data.formats ? data.formats : []
      };
      this.files = response.files || [];
    }, (error) => {

    });
  }

  removeSource(source: ISource) {
    this.confirmation.confirm({
      message: 'Do you want to delete this citation?',
      header: 'Confirmation',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.model.sources = this.model.sources.filter(x => x !== source);
      }
    });
  }

  editSource(index: number) {
    this.editSourceIndex = index;
    this.source = {...this.model.sources[index]};
    this.showAddSource = true;
  }

  addKeywordClick() {
    this.keyword = '';
    this.showAddKeyword = true;
  }

  addKeyword() {
    if (this.keyword) {
      this.model.keywords.push(this.keyword);
      this.showAddKeyword = false;
    }
  }

  addIssnClick() {
    this.issn = '';
    this.showAddIssn = true;
  }

  addIssn() {
    if (this.issn) {
      this.model.issn.push(this.issn);
      this.showAddIssn = false;
    }
  }

  addIsbn() {
    if (this.isbn) {
      this.model.isbn.push(this.isbn);
      this.showAddIsbn = false
    }
  }

  addDoi() {
    if (this.doi) {
      this.model.doi.push(this.doi);
      this.showAddDoi = false;
    }
  }

  addIsbnClick() {
    this.isbn = '';
    this.showAddIsbn = true;
  }

  addDoiClick() {
    this.doi = '';
    this.showAddDoi = true;
  }

  addFileClick() {
    this.currentFile = null;
    this.fileLanguage = null;
    this.showAddFile = true;
  }

  onUpload(event: { files: File[] }) {
    const file = event.files[0];
    let fileReader = new FileReader();
    fileReader.readAsDataURL(file);
    fileReader.onload = () => {
      const result = fileReader.result as string;
      const base64 = result.split(',').pop();
      this.currentFile = {base64Content: base64, filename: file.name, mimeType: file.type, id: uuid()};
    };
  };

  addFile() {
    if (this.fileLanguage) {
      this.currentFile.language = this.fileLanguage;
      this.files = [...this.files, this.currentFile];
      this.showAddFile = false;
      console.log(this.files);
    } else {
      this.alertService.error('Please select language!');
    }
  }

  getFile(file: IFile) {
    if (file.base64Content) {
      return; // not sure what to do here, local file still not uploaded to server

    } else {
      this.http.getFile('case/GetExpertFile/' + file.id).subscribe((response: HttpResponse<Blob>) => {
        const encodedFileName = response.headers.get('File-name');
        const fileName = decodeURIComponent(encodedFileName);
        let dataType = response.body.type;
        let binaryData = [];
        binaryData.push(response.body);
        let downloadLink = document.createElement('a');
        downloadLink.href = window.URL.createObjectURL(new Blob(binaryData, {type: dataType}));
        downloadLink.download = fileName;
        // if (filename)
        //   downloadLink.setAttribute('download', filename);
        document.body.appendChild(downloadLink);
        downloadLink.click();
        downloadLink.remove();
      });
    }
  }

  removeFile(file: IFile) {
    this.files = this.files.filter(x => x !== file);
  }

  addLanguageClick() {
    this.tempLanguages = [...this.model.languages];
    this.showAddLanguage = true;
  }

  addLanguage() {   // change to Checkbox/filter dropdown if Ico wants it that way
    this.model.languages = [...this.tempLanguages];
    this.showAddLanguage = false;
  }

  checkAllFunc(checked: boolean) {
    if (checked) {
      this.tempLanguages = this.languages.map(x => x.nameEn);
    } else {
      this.tempLanguages = [];
    }
  }

  addFormatClick() {
    this.tempFormats = [...this.model.formats];
    this.otherFormat = null;
    this.showAddFormat = true;
  }

  addFormat() {
    this.model.formats = [...this.tempFormats];
    if (this.otherFormat) {
      this.model.formats.push(this.otherFormat);
    }
    this.showAddFormat = false;
  }

  licenseChange(event: any) {
    const value = event.value;
    console.log(value);
  }
}
