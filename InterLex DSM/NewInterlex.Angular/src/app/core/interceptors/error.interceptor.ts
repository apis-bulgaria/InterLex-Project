import {Injectable} from '@angular/core';
import {HttpEvent, HttpHandler, HttpInterceptor, HttpRequest} from '@angular/common/http';
import {Observable, throwError} from 'rxjs';
import {catchError} from 'rxjs/operators';
import {Router} from '@angular/router';
import { AuthService } from 'src/app/admin/services/auth.service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private auth: AuthService, private router: Router) {
  }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(catchError(err => {
      if (err.status === 401) {
        this.auth.logout();
        this.router.navigate(['login']);
      }
      // const error = err.error.message || err.statusText;
      return throwError(err);
    }));
  }
}
