import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, CanActivateChild} from '@angular/router';
import {Observable} from 'rxjs';
import {AuthService} from '../services/auth.service';

@Injectable(
  {providedIn: 'root'}
)
export class AuthGuard implements CanActivate, CanActivateChild {


  constructor(private router: Router, private authService: AuthService) {
  }

  canActivateChild(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    return this.canActivate(route, state);
  }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    const url: string = state.url;

    if (this.authService.isLoggedIn() && !this.authService.isLimitedAdmin()) {
      return true;
    }

    this.router.navigate(['./login'], {queryParams: {returnUrl: state.url}});
    return false;
  }
}
