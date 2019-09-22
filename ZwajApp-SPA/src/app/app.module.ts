import { NgxGalleryModule } from 'ngx-gallery';
import { RouterModule } from '@angular/router';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BsDropdownModule, TabsModule, BsDatepickerModule, PaginationModule, ButtonsModule, ModalModule } from 'ngx-bootstrap';
import { JwtModule } from '@auth0/angular-jwt';
import { FileUploadModule } from 'ng2-file-upload';

import { AppComponent } from './app.component';
import {TimeAgoPipe} from 'time-ago-pipe';
import { NavComponent } from './nav/nav.component';
import { AuthService } from './_services/auth.service';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';
import { ErrorInterceptorProvidor } from './_services/error.interceptor';
import { AlertifyService } from './_services/alertify.service';
import { MemberListComponent } from './members/member-list/member-list.component';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { appRoutes } from './routes';
import { AuthGuard } from './_guards/auth.guard';
import { UserService } from './_services/user.service';
import { MemberCardComponent } from './members/member-card/member-card.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberDetailResolver } from './_resolvers/member-detail.resolver';
import { MemberListResolver } from './_resolvers/member-list.resolver';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberEditResolver } from './_resolvers/member-edit.resolver';
import { PreventUnsavedChangesGuard } from './_guards/prevent-unsaved-changes.guard';
import { PhotoEditerComponent } from './members/photo-editer/photo-editer.component';
import { ListResolver } from './_resolvers/lists.resolver';
import { MessageResolver } from './_resolvers/message.resolver';
import { MemberMessagesComponent } from './members/member-messages/member-messages.component';
import { PaymentComponent } from './payment/payment.component';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { HasRoleDirective } from './_directives/has-role.directive';
import { UserManagementComponent } from './admin/user-management/user-management.component';
import { PhotoManagementComponent } from './admin/photo-management/photo-management.component';
import { AdminService } from './_services/admin.service';
import { RolesModalComponent } from './admin/roles-modal/roles-modal.component';
import { LangDirective } from './_directives/lang.directive';
import { AllMembersReportComponent } from './_reports/all-members-report/all-members-report.component';

export function tokenGetter() {
   return localStorage.getItem('token');
}

@NgModule({
   declarations: [
      AppComponent,
      NavComponent,
      HomeComponent,
      RegisterComponent,
      MemberListComponent,
      ListsComponent,
      MessagesComponent,
      MemberCardComponent,
      MemberDetailComponent,
      MemberEditComponent,
      PhotoEditerComponent,
      TimeAgoPipe,
      MemberMessagesComponent,
      PaymentComponent,
      AdminPanelComponent,
      HasRoleDirective,
      UserManagementComponent,
      PhotoManagementComponent,
      RolesModalComponent,
      LangDirective,
      AllMembersReportComponent
   ],
   imports: [
      BrowserModule,
      HttpClientModule,
      FormsModule,
      ReactiveFormsModule,
      FileUploadModule,
      NgxGalleryModule,
      PaginationModule.forRoot(),
      BsDropdownModule.forRoot(),
      BsDatepickerModule.forRoot(),
      RouterModule.forRoot(appRoutes),
      ButtonsModule,
      ModalModule.forRoot(),
      TabsModule.forRoot(),
      JwtModule.forRoot({
         config: {
            tokenGetter: tokenGetter,
            whitelistedDomains: ['localhost:5000'],
            blacklistedRoutes: ['localhost:5000/api/auth']
         }
      })
   ],
   providers: [
      AuthService,
      ErrorInterceptorProvidor,
      AlertifyService,
      AuthGuard,
      PreventUnsavedChangesGuard,
      UserService,
      MemberDetailResolver,
      MemberListResolver,
      MemberEditResolver,
      ListResolver,
      MessageResolver,
      AdminService
   ],
   entryComponents: [RolesModalComponent],
   bootstrap: [
      AppComponent
   ]
})
export class AppModule { }
