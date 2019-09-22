import { Directive, OnInit, ElementRef, AfterViewInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';

// @ts-ignore
import * as words from '../../assets/jsons/dictionary.json';
@Directive({
  selector: '[lang]'
})
export class LangDirective implements OnInit, AfterViewInit {

_words = [];
  constructor(private ref: ElementRef, private authService: AuthService ) { }
  ngOnInit(): void {
    this._words = words.default;
  }
  ngAfterViewInit(): void {
    this.authService.lang.subscribe(
      lang => {
        if (lang === 'en') {
          try {
            var word = this._words.filter(word => word['ar'].match(this.ref.nativeElement.innerText));
            if (word[0]['ar'] === this.ref.nativeElement.innerText)
            this.ref.nativeElement.innerText = word[0]['en'];
          } catch { }
        }
        if (lang === 'ar') {
          try {
            var word = this._words.filter(word => word['en'].match(this.ref.nativeElement.innerText));
            if (word[0]['en'] === this.ref.nativeElement.innerText)
            this.ref.nativeElement.innerText = word[0]['ar'];
          } catch { }
        }
      }
    );
  }
}
