import { Component, OnInit, ViewChild } from '@angular/core';
import { AllMembersReportComponent } from 'src/app/_reports/all-members-report/all-members-report.component';

@Component({
  selector: 'app-admin-panel',
  templateUrl: './admin-panel.component.html',
  styleUrls: ['./admin-panel.component.css']
})
export class AdminPanelComponent implements OnInit {
@ViewChild('report') report: AllMembersReportComponent;
  constructor() { }

  ngOnInit() {
  }
printAll() {
  this.report.printAll();
}
}
