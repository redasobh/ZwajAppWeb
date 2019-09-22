import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AllMembersReportComponent } from './all-members-report.component';

describe('AllMembersReportComponent', () => {
  let component: AllMembersReportComponent;
  let fixture: ComponentFixture<AllMembersReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AllMembersReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AllMembersReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
