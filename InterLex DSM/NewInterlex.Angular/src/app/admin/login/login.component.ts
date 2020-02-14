import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ActivatedRoute, Router} from '@angular/router';
import {first} from 'rxjs/operators';
import {AlertService} from '../../core/services/alert.service';
import { AuthService } from '../services/auth.service';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  loading = false;
  submitted = false;
  returnUrl: string;

  constructor(private formBuilder: FormBuilder, private route: ActivatedRoute, private authService: AuthService,
              private router: Router, private alertService: AlertService, private msgSrc: MessageService) {
  }

  ngOnInit() {
    this.loginForm = this.formBuilder.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });

    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
  }

  get f() {
    return this.loginForm.controls;
  }

  onSubmit() {
    this.submitted = true;
    if (this.loginForm.invalid) {
      return;
    }

    this.loading = true;

    this.authService.login(this.f.username.value, this.f.password.value)
      .pipe(first())
      .subscribe((data) => {
        this.router.navigate([this.returnUrl]);
        },
        error => {
          this.alertService.error('Wrong password or no such account.');
          //this.alertService.error('Wrong password or no such account.');
          this.loading = false;
        });
  }

}
