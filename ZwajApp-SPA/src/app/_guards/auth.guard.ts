import { Injectable } from '@angular/core';
import {  CanActivate, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';

/*import { ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, CanActivate } from '@angular/router';
import { Observable } from 'rxjs';*/

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private authservice: AuthService, private router: Router, private alertify: AlertifyService) {
  }
  canActivate(next: ActivatedRouteSnapshot): boolean {
    const roles = next.firstChild.data['roles'] as Array<string>;
    if (roles) {
      const match = this.authservice.roleMatch(roles);
      if (match) {
        return true;
      } else {
        this.router.navigate(['members']);
        this.alertify.error('غير مسموح لك بالدخول');
      }
    }
    if (this.authservice.loggedIn()) {
      this.authservice.hubConnection.stop();
      return true;
    }
    this.alertify.error('يجب تسجيل الدخول أولا');
    this.router.navigate(['']);
    return false;
    }
  /* Observable<boolean> | Promise<boolean> |
  next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot */
}
