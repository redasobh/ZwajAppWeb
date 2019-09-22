import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { Injectable } from '@angular/core';
import { User } from '../_models/user';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Message } from '../_models/message';
import { AuthService } from '../_services/auth.service';

@Injectable()
export class MessageResolver implements Resolve<Message[]> {
pageNumber = 1;
pageSize = 6;
messageType = 'Unread';
constructor(private authService: AuthService, private userService: UserService, private router: Router,
            private alertify: AlertifyService) {}
resolve(route: ActivatedRouteSnapshot): Observable<Message[]> {
    return this.userService.getMessages(this.authService.decodedToken.nameid , this.pageNumber, this.pageSize, this.messageType).pipe(
        catchError(error => {
            this.alertify.error('يوجد مشكلة فى عرض الرسائل');
            this.router.navigate(['']);
            return of(null);
        })
    );
}
}
