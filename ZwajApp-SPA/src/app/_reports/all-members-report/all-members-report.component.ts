import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/user';
import { UserService } from 'src/app/_services/user.service';
import * as jspdf from 'jspdf';
import * as html2canvas from 'html2canvas';

@Component({
  selector: 'app-all-members-report',
  templateUrl: './all-members-report.component.html',
  styleUrls: ['./all-members-report.component.css']
})
export class AllMembersReportComponent implements OnInit {
  users: User[];

  constructor(private userService: UserService) { }

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.userService.getAllUsers().subscribe((users: User[]) => {
      this.users = users;
     });
     }

     printAll() {
        var data = document.getElementById('print');
        html2canvas(data, {
          onclone: function (clonedDoc) {
              clonedDoc.getElementById('print').style.display = 'block';
          }
      }).then(canvas => {
        var imgWidth = 208;
        var imgHeight = canvas.height * imgWidth / canvas.width;
        const contentDataURL = canvas.toDataURL('image/png');
        let pdf = new jspdf('p', 'mm', 'a4');
        var position = 10;
        pdf.addImage(contentDataURL, 'PNG', 0, position, imgWidth, imgHeight);
        // pdf.save('MYPdf.pdf');//save pdf
        // pdf.output('dataurlnewwindow'); //with firefox only
        window.open(pdf.output('bloburl'), '_blank'); // with all browsers new page
      });
      }
}
