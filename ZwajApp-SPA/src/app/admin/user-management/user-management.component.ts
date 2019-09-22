import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/user';
import { AdminService } from 'src/app/_services/admin.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { BsModalRef, BsModalService } from 'ngx-bootstrap';
import { RolesModalComponent } from '../roles-modal/roles-modal.component';
import { UserService } from 'src/app/_services/user.service';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {
users: User[];
userId: number;
bsModalRef: BsModalRef;
  constructor(private adminService: AdminService, private alertify: AlertifyService,
              private modalService: BsModalService, private userService: UserService) { }

  ngOnInit() {
    this.getUsersWithRoles();
  }
  getUsersWithRoles() {
    this.adminService.getUsersWithRoles().subscribe(
      (users: User[]) => {
        this.users = users;
      },
      error => {
        this.alertify.error('حدثت مشكلة فى جلب المستخدمين');
      }
    );
  }
editRolesModal(user: User) {
  const initialState = {
   user,
   roles: this.getRolesArray(user)
  };
  this.bsModalRef = this.modalService.show(RolesModalComponent, {initialState});
  // this.bsModalRef.content.closeBtnName = 'Close';
  this.bsModalRef.content.updateSelectedRoles.subscribe((values) => {
    const rolesToUpdate = {
      rolesNames : [...values.filter(el => el.checked === true).map(el => el.value)]
    };
    // console.log(rolseToupdate);
    if (rolesToUpdate) {
      this.adminService.updateUserRoles(user, rolesToUpdate).subscribe(
        () => {
          user.roles = [...rolesToUpdate.rolesNames];
        }, error => this.alertify.error(error)
      );
    }
  });
}
private getRolesArray(user) {
  const roles = [];
  const userRoles = user.roles as any[];
  const availableRoles: any[] = [
    {name: 'مدير النظام', value: 'Admin'},
    {name: 'مشرف', value: 'Moderator'},
    {name: 'عضو', value: 'Member'},
    {name: 'مشترك', value: 'VIP'},
  ];
  availableRoles.forEach(aRole => {
    let isMatch = false;
    userRoles.forEach(uRole => {
      if (aRole.value === uRole) {
        isMatch = true;
        aRole.checked = true;
        roles.push(aRole);
        return;
       }
    });
    if (!isMatch) {
      aRole.checked = false;
      roles.push(aRole);
    }
  });
  return roles;
}
getUserReport() {
  this.userService.GetReportForUser(this.userId).subscribe((response) => {
    let file = new Blob([response], { type: 'application/pdf' });
    var fileURL = URL.createObjectURL(file);
    window.open(fileURL, '_blank');
      });
}
}
