import { Component, OnInit, ViewChildren, QueryList } from '@angular/core';
import { Observable } from 'rxjs';
import { LeadMaster } from '../models/lead-master';
import { FormControl } from '@angular/forms';
import { NgbdSortableHeader, SortEvent } from '../services/sortable.directive';
import { DataTableService } from '../services/data-table.service';
import { LeadRepositoryService } from '../services/lead-repository.service';
import { Router } from '@angular/router';
import { UserMaster } from '../models/user-master';
import { AuthenticationService } from '../services/authentication.service';
import { roles } from '../services/generic-enums';
import { ExcelExportService } from '../services/excel-export.service';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-lead-list',
  templateUrl: './lead-list.component.html',
  styleUrls: ['./lead-list.component.css']
})
export class LeadListComponent implements OnInit {

  currentUser: UserMaster;
  leadList: Observable<LeadMaster[]>;
  total$: Observable<number>;
  filter = new FormControl('');
  isTeleCaller: boolean;
  isManager: boolean;

  @ViewChildren(NgbdSortableHeader) headers: QueryList<NgbdSortableHeader>;
  constructor(public service: DataTableService, private leadRepo: LeadRepositoryService, private router: Router, private auth: AuthenticationService,
    private excelService: ExcelExportService, private datePipe: DatePipe) {
    this.auth.currentUser.subscribe(x => this.currentUser = x);
  }

  ngOnInit(): void {
    if (this.currentUser.roleId == roles["Relationship Manager"]) {
      this.fillLeadListByRM();
      this.isManager = true;
    }
    else if (this.currentUser.roleId == roles["Tele Caller"]) {
      this.fillLeadListCreatedBy();
      this.isTeleCaller = true;
    }

    this.service.searchTerm = '';
  }

  fillLeadListCreatedBy() {
    this.leadRepo.loadLeadListByCreatedBy(this.currentUser.userId).subscribe(
      (leads) => {
        this.service.xType = new LeadMaster();
        this.service.TABLE = leads;
        this.leadList = this.service.dataList$;
        //this.filteredUserList = this.filter.valueChanges.pipe(startWith(''), map(text => search(users, text, this.pipe)));
        this.total$ = this.service.total$;
      },
      error => console.error(error)
    );
  }

  fillLeadListByRM() {
    this.leadRepo.loadLeadListByRM(this.currentUser.userId).subscribe(
      (leads) => {
        this.service.xType = new LeadMaster();
        this.service.TABLE = leads;
        this.leadList = this.service.dataList$;
        //this.filteredUserList = this.filter.valueChanges.pipe(startWith(''), map(text => search(users, text, this.pipe)));
        this.total$ = this.service.total$;
      },
      error => console.error(error)
    );
  }

  onSort({ column, direction }: SortEvent) {
    // resetting other headers
    this.headers.forEach(header => {
      if (header.sortable !== column) {
        header.direction = '';
      }
    });

    this.service.sortColumn = column;
    this.service.sortDirection = direction;
  }

  editLead(callId: string) {
    localStorage.removeItem("callIdEdit");
    localStorage.setItem("callIdEdit", callId.toString());
    this.router.navigate(['/lead-create']);
  }

  followUp(callId: string) {
    localStorage.removeItem("callIdFollowUp");
    localStorage.setItem("callIdFollowUp", callId.toString());
    this.router.navigate(['/lead-followup']);
  }

  exportAsXLSX(): void {
    let leadArray: LeadMaster[];
    this.leadList.subscribe(data => leadArray = data);
    //leadArray = leadArray.filter(function (props) {
    //  delete props.callId;
    //  delete props.followupHistory;
    //  return true;
    //});
    let ExportleadArray: any[];
    if (this.isTeleCaller == true) {
      ExportleadArray = leadArray.map(obj => ({
        'First Name': obj.firstName, 'Last Name': obj.lastName, 'Mobile Number': obj.mobileNumber, 'Address': obj.address, 'Created Date': this.datePipe.transform(obj.createdDate, "dd-MM-yyyy"),
        'Status': obj.outComeText, 'Allocated To': obj.allocatedToName, 'Appoinment DateTime': this.datePipe.transform(obj.appointmentDateTime, "dd-MM-yyyy HH:mm a"), 'Remarks': obj.remark
      }));
    }
    else if (this.isManager == true) {
      ExportleadArray = leadArray.map(obj => ({
        'First Name': obj.firstName, 'Last Name': obj.lastName, 'Mobile Number': obj.mobileNumber, 'Address': obj.address, 'Created Date': this.datePipe.transform(obj.createdDate, "dd-MM-yyyy"),
        'Status': obj.outComeText, 'Created By': obj.createdByName, 'Appoinment DateTime': this.datePipe.transform(obj.appointmentDateTime, "dd-MM-yyyy HH:mm a"), 'Remarks': obj.remark
      }));
    }
    this.excelService.exportAsExcelFile(ExportleadArray, 'LeadDetail');
  }
}
