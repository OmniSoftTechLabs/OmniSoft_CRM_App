import { Component, OnInit, ViewChildren, QueryList } from '@angular/core';
import { Observable } from 'rxjs';
import { LeadMaster, OutcomeMaster, AppoinmentStatusMaster, CallTransactionDetail, FollowupHistory } from '../models/lead-master';
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
import { RmanagerMaster } from '../models/rmanager-master';
import { NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { FilterOptions } from '../models/filter-options';

@Component({
  selector: 'app-lead-list',
  templateUrl: './lead-list.component.html',
  styleUrls: ['./lead-list.component.css']
})
export class LeadListComponent implements OnInit {

  currentUser: UserMaster;
  outcomeList: OutcomeMaster[] = [];
  appoinStatusList: AppoinmentStatusMaster[] = [];
  filterUserList: RmanagerMaster[] = [];
  leadList: Observable<LeadMaster[]>;
  callHistory: CallTransactionDetail[] = [];
  followupHistory: FollowupHistory[] = [];
  total$: Observable<number>;
  filter = new FormControl('');
  isTeleCaller: boolean;
  isManager: boolean;
  outComeId: number = 1;
  appoinStatusId: number = 7
  filteruserId: string = "0";
  filterDateOption: string = "Created Date";
  filterDateById: number;
  fromDate: NgbDateStruct;
  toDate: NgbDateStruct;
  filterOption: FilterOptions = new FilterOptions();
  allocateCreateByTxt: string = "";
  modalTitle: string = "";

  @ViewChildren(NgbdSortableHeader) headers: QueryList<NgbdSortableHeader>;
  constructor(public service: DataTableService, private leadRepo: LeadRepositoryService, private router: Router, private auth: AuthenticationService,
    private excelService: ExcelExportService, private datePipe: DatePipe) {
    this.auth.currentUser.subscribe(x => this.currentUser = x);
    this.fromDate = { day: new Date().getDate() - 7, month: new Date().getMonth() + 1, year: new Date().getFullYear() };
    this.toDate = { day: new Date().getDate(), month: new Date().getMonth() + 1, year: new Date().getFullYear() };
  }

  ngOnInit(): void {
    if (this.currentUser.roleId == roles["Relationship Manager"]) {
      this.allocateCreateByTxt = "Created By";
      this.onChangeFilterDateOption(2);
      this.fillLeadListByRM();
      this.fillAppoinStatus();
      this.fillTeleCallerList();
      this.isManager = true;
    }
    else if (this.currentUser.roleId == roles["Tele Caller"]) {
      this.allocateCreateByTxt = "Allocated To";
      this.onChangeFilterDateOption(1);
      this.fillLeadListCreatedBy();
      this.fillOutCome();
      this.fillRManagerList();
      this.isTeleCaller = true;
    }
  }

  async fillLeadListCreatedBy() {
    this.filterOption.status = this.outComeId;
    this.filterOption.allocatedTo = this.filteruserId;
    this.filterOption.dateFilterBy = this.filterDateById;
    this.filterOption.fromDate = new Date(this.fromDate.year, this.fromDate.month - 1, this.fromDate.day);
    this.filterOption.todate = new Date(this.toDate.year, this.toDate.month - 1, this.toDate.day);
    await this.leadRepo.loadLeadListByCreatedBy(this.currentUser.userId, this.filterOption).then(
      (leads) => {
        this.service.xType = new LeadMaster();
        this.service.TABLE = leads;
        this.leadList = this.service.dataList$;
        //this.filteredUserList = this.filter.valueChanges.pipe(startWith(''), map(text => search(users, text, this.pipe)));
        this.total$ = this.service.total$;
      },
      error => console.error(error)
    );
    this.service.searchTerm = '';
  }

  fillLeadListByRM() {
    this.filterOption.status = this.appoinStatusId;
    this.filterOption.createdBy = this.filteruserId;
    this.filterOption.dateFilterBy = this.filterDateById;
    this.filterOption.fromDate = new Date(this.fromDate.year, this.fromDate.month - 1, this.fromDate.day);
    this.filterOption.todate = new Date(this.toDate.year, this.toDate.month - 1, this.toDate.day);
    this.leadRepo.loadLeadListByRM(this.currentUser.userId, this.filterOption).subscribe(
      (leads) => {
        this.service.xType = new LeadMaster();
        this.service.TABLE = leads;
        this.leadList = this.service.dataList$;
        //this.filteredUserList = this.filter.valueChanges.pipe(startWith(''), map(text => search(users, text, this.pipe)));
        this.total$ = this.service.total$;
      },
      error => console.error(error)
    );
    this.service.searchTerm = '';
  }

  fillOutCome() {
    this.leadRepo.loadOutcomeList().subscribe(
      outCome => {
        this.outcomeList = outCome;
      }, error => console.error(error)
    );
  }

  fillAppoinStatus() {
    this.leadRepo.loadAppoinmentStatusList().subscribe(
      aStatus => {
        this.appoinStatusList = aStatus;
      }, error => console.error(error)
    );
  }

  fillRManagerList() {
    this.leadRepo.loadRManagerList().subscribe(
      rManager => {
        this.filterUserList = rManager;
      }, error => console.error(error)
    );
  }

  fillTeleCallerList() {
    this.leadRepo.loadTeleCallerList().subscribe(
      rManager => {
        this.filterUserList = rManager;
      }, error => console.error(error)
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

  onChangeFilterDateOption(id: number) {
    if (id == 1) {
      this.filterDateOption = "Created Date";
      this.filterDateById = 1;
    }
    else if (id == 2) {
      this.filterDateOption = "Appoinment Date";
      this.filterDateById = 2;
    }
  }

  onFilterSearch() {
    if (this.isManager)
      this.fillLeadListByRM();
    else if (this.isTeleCaller)
      this.fillLeadListCreatedBy();
  }

  async viewHistory(id: number, firstName: string, lastName: string) {
    if (this.isTeleCaller) {
      this.modalTitle = "Call History: " + firstName + " " + lastName;
      await this.leadRepo.loadCallTransById(id).then(
        (history) => {
          this.callHistory = history;
        },
        error => console.error(error)
      );
    }
    else if (this.isManager) {
      this.modalTitle = "Followup History: " + firstName + " " + lastName;
      await this.leadRepo.loadFollowupHistoryById(id).then(
        (history) => {
          this.followupHistory = history;
        },
        error => console.error(error)
      );
    }
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
