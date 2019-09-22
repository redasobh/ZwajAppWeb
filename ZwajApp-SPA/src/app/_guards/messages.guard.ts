import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../_services/auth.service';
import { UserService } from '../_services/user.service';

@Injectable({
  providedIn: 'root'
})
export class MessagesGuard implements CanActivate {
  constructor(private authService: AuthService, private userService: UserService , private router: Router ) {}
  canActivate(): boolean {
    this.userService.getPaymentForUser(this.authService.currentUser.id).subscribe(
      res => {
        if (res === null) {
          this.router.navigate(['charge']);
          return false;
        }
        else {
         // this.router.navigate(['messages']);
          return true;
        }
      }
    );
    return true;
  }
}
