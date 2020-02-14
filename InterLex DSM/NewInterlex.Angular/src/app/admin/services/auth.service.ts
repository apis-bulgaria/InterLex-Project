import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { User} from '../../models/user.model';
import { BehaviorSubject, Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';

@Injectable(
  // { providedIn: 'root'}
  )
export class AuthService {
  apiUrl = environment.apiBaseUrl;

  loggedIn = false; // alternative to checking localstorage?
  private subject = new BehaviorSubject<boolean>(this.isLoggedIn());

  constructor(private http: HttpClient) {
  }

  login(username: string, password: string) {
    return this.http.post(this.apiUrl + 'auth/login', { username, password })
      .pipe(map((user: User) => {
        if (user && user.accessToken.token) {
          localStorage.setItem('dsmUser', JSON.stringify(user));
          this.subject.next(true);
        }
        return user;
      }));
  }

  logout() {
    localStorage.removeItem('dsmUser');
    this.subject.next(false);
  }

  isLoggedIn() {
    return localStorage.getItem('dsmUser') !== null;
  }

  isAdmin() {
    const user = localStorage.getItem('dsmUser');
    if (user) {
        return true;
      }
    return false;
  }

  isSuperAdmin() {
    const user = localStorage.getItem('dsmUser');
    if (user) {
        return true;
    }
    return false;
  }

  getUsername() {
    const user = localStorage.getItem('dsmUser');
    if (user) {
      const parsed: User = JSON.parse(user);
      return parsed.userName;
    }
  }

  isLimitedAdmin() { // for the italian guys, used to show guids in navigation and hiding the admin tab
    const user = localStorage.getItem('dsmUser');
    if (user) {
      const parsed: User = JSON.parse(user);
      return parsed.userName === 'giuseppe.contissa@unibo.it';
    }
    return false;
  }

  getToken() {
    const user = localStorage.getItem('dsmUser');
    if (user) {
      const parsed: User = JSON.parse(user);
      return parsed.accessToken.token;
    }
  }

  getLoggedInStatus(): Observable<boolean> {
    return this.subject.asObservable();
  }
}
