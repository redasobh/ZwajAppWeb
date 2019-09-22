import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PhotoEditerComponent } from './photo-editer.component';

describe('PhotoEditerComponent', () => {
  let component: PhotoEditerComponent;
  let fixture: ComponentFixture<PhotoEditerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PhotoEditerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PhotoEditerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
