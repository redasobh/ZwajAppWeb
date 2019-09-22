import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { BsDatepickerConfig, BsLocaleService } from 'ngx-bootstrap';
import { defineLocale } from 'ngx-bootstrap/chronos';
import { arLocale } from 'ngx-bootstrap/locale';
import { User } from '../_models/user';
import { Router } from '@angular/router';
defineLocale('ar', arLocale);

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
 /* @Input() valuesFromRegister: any;*/
  @Output() cancelRegister = new EventEmitter();
user: User;
registerForm: FormGroup;
bsConfig: Partial<BsDatepickerConfig>;
locale: 'ar';
  constructor(private router: Router, private authService: AuthService, private alertify: AlertifyService, private fb: FormBuilder,
              private localeService: BsLocaleService ) {
              this.localeService.use(this.locale);
              }
  ngOnInit() {
    // this.registerForm = new FormGroup({
    //  username: new FormControl('', Validators.required),
    //  password : new FormControl('', [Validators.required, Validators.maxLength(8), Validators.minLength(4)]),
    //  confirmPassword: new FormControl('', Validators.required)
    // }, this.passwordMatchValidator);
    this.bsConfig = {
      containerClass: 'theme-red',
      showWeekNumbers: false
    };
    this.createRegisterForm();
  }
  createRegisterForm() {
    this.registerForm = this.fb.group({
      gender: ['رجل'],
      username: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: [null, Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', Validators.required]
    }, {Validators: this.passwordMatchValidator});
  }
  passwordMatchValidator(form: FormControl) {
    return form.get('password').value === form.get('confirmPassword').value ? null : { 'mismatch': true};
  }
register() {
/*console.log('تم الاشتراك');
console.log(this.model);*/
if (this.registerForm.valid) {
this.user = Object.assign({}, this.registerForm.value);
this.authService.register(this.user).subscribe(
  () => { this.alertify.success('تم الاشتراك بنجاح'); },
  error => { this.alertify.error(error); },
  () => {this.authService.login(this.user).subscribe(
() => {
  this.router.navigate(['/members']);
}
  )}
  );
}
// console.log(this.registerForm.value);
}
cancel() {
  /*console.log('ليس الان ');*/
  this.cancelRegister.emit(false);
}
}
