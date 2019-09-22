import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Photo } from 'src/app/_models/photo';
import { FileUploader } from 'ng2-file-upload';
import { environment } from 'src/environments/environment';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { User } from 'src/app/_models/user';


@Component({
  selector: 'app-photo-editer',
  templateUrl: './photo-editer.component.html',
  styleUrls: ['./photo-editer.component.css']
})
export class PhotoEditerComponent implements OnInit {
@Input() photo: Photo[];
@Output() getMemberPhotoChange = new EventEmitter<string>();
   uploader: FileUploader ;
   hasBaseDropZoneOver = false;
   // hasAnotherDropZoneOver = false;
   baseUrl = environment.apiUrl;
   CurrentMain: Photo;
   user: User;
  constructor(private authService: AuthService , private userService: UserService,
              private alertify: AlertifyService, private route: ActivatedRoute) { }

  ngOnInit() {
    this.initializeUploader();
     // this.route.data.subscribe(data => {
     // this.user = data['user'];
       // });

  }
   fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }
  initializeUploader() {
    this.uploader = new FileUploader({
url: this.baseUrl + 'users/' + this.authService.decodedToken.nameid + '/photo',
authToken: 'Bearer ' + localStorage.getItem('token'),
isHTML5: true,
allowedFileType: ['image'],
removeAfterUpload: true,
autoUpload: false,
maxFileSize: 10 * 1024 * 1024
    });
    this.uploader.onAfterAddingFile = (file) => { file.withCredentials = false; };
    this.uploader.onSuccessItem = (item, Response, status, Headers) => {
      if (Response) {
        const res: Photo = JSON.parse(Response);
        const photo = {
          id: res.id,
          url: res.url,
          dateAdded: res.dateAdded,
          isMain: res.isMain,
          isApproved: res.isApproved
        };
        this.photo.push(photo);
        if (photo.isMain) {
          this.authService.changeMemberPhoto(photo.url);
          this.authService.currentUser.photoURL = photo.url;
          localStorage.setItem('user', JSON.stringify(this.authService.currentUser));
        }
      }
    };
  }
  SetMainPhoto(photo: Photo) {
    this.userService.SetMainPhoto(this.authService.decodedToken.nameid, photo.id).subscribe(
      () => {this.CurrentMain = this.photo.filter( p => p.isMain === true)[0];
             this.CurrentMain.isMain = false;
             photo.isMain = true;
            // this.getMemberPhotoChange.emit(photo.url);
            // this.user.photoURL = photo.url;
             this.authService.changeMemberPhoto(photo.url);
             this.authService.currentUser.photoURL = photo.url;
             localStorage.setItem('user', JSON.stringify(this.authService.currentUser));
  },
      () => {this.alertify.error('يوجد مشكلة فى الصورة الاساسية'); }
    );
  }
  delete(id: number) {
    this.alertify.confirm('هل تريد حذف الصورة', () => {
this.userService.deletePhoto(this.authService.decodedToken.nameid, id).subscribe(
  () => {
    this.photo.splice(this.photo.findIndex(p => p.id === id), 1);
    this.alertify.success('تم حذف الصورة بنجاح');
  }, error => {this.alertify.error('حدث خطأ أثناء حذف الصورة'); }
);
    });
  }
}
