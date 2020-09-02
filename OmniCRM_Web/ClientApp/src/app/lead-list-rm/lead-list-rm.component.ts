import { Component, OnInit, ViewChild, ViewChildren, QueryList, ElementRef } from '@angular/core';
import { UserMaster } from '../models/user-master';
import { NgbdSortableHeader, SortEvent } from '../services/sortable.directive';
import { Validators, FormControl } from '@angular/forms';
import { DataTableService } from '../services/data-table.service';
import { LeadRepositoryService } from '../services/lead-repository.service';
import { Router } from '@angular/router';
import { AuthenticationService } from '../services/authentication.service';
import { NgbModalConfig, NgbModal, NgbDateStruct, NgbTimeStruct } from '@ng-bootstrap/ng-bootstrap';
import { DatePipe } from '@angular/common';
import { ExcelExportService } from '../services/excel-export.service';
import { AdminSetting } from '../models/admin-setting';
import { LeadMaster, AppoinmentStatusMaster, FollowupHistory } from '../models/lead-master';
import { LeadFollowUpComponent } from '../lead-follow-up/lead-follow-up.component';
import { Observable } from 'rxjs';
import { RmanagerMaster } from '../models/rmanager-master';
import { FilterOptions } from '../models/filter-options';
import { roles } from '../services/generic-enums';

@Component({
  selector: 'app-lead-list-rm',
  templateUrl: './lead-list-rm.component.html',
  styleUrls: ['./lead-list-rm.component.css']
})
export class LeadListRmComponent implements OnInit {

  currentUser: UserMaster;
  appoinStatusList: AppoinmentStatusMaster[] = [];
  filterUserList: RmanagerMaster[] = [];
  leadList: Observable<LeadMaster[]>;
  followupHistory: FollowupHistory[] = [];
  total$: Observable<number>;
  filter = new FormControl('');
  appoinStatusId: number[] = [];
  filteruserId: string = "0";
  filterDateOption: string = "Created Date";
  filterDateById: number;
  fromDate: NgbDateStruct;
  toDate: NgbDateStruct;
  filterOption: FilterOptions = new FilterOptions();
  modalTitle: string = "";
  uploadMsg: string = "";
  adminSetting: AdminSetting;
  overDueDays: number;
  allocateCreatedBytxt: string = "";
  isCheckedBox: boolean;
  isAdmin: boolean;
  checkedList: LeadMaster[] = [];
  minDate: NgbDateStruct;
  nextFollowupDate: NgbDateStruct;
  nextFollowupTime: NgbTimeStruct;
  isLoading: boolean = false;
  dateTimeStr: string;


  @ViewChild('labelImport') labelImport: ElementRef;
  @ViewChild('fileInput') fileInput;
  @ViewChildren(NgbdSortableHeader) headers: QueryList<NgbdSortableHeader>;

  dateCtrl = new FormControl('', Validators.required);
  timeCtrl = new FormControl('', Validators.required);

  constructor(public service: DataTableService, private leadRepo: LeadRepositoryService, private router: Router, private auth: AuthenticationService,
    private excelService: ExcelExportService, private datePipe: DatePipe, private modalService: NgbModal, private modalConfig: NgbModalConfig) {
    this.auth.currentUser.subscribe(x => this.currentUser = x);
    if (this.currentUser.roleId == roles["Admin"])
      this.isAdmin = true;
    modalConfig.backdrop = 'static';
    modalConfig.keyboard = false;
    let getFromDate = new Date(new Date().getTime() - (2 * 24 * 60 * 60 * 1000));
    this.nextFollowupDate = this.minDate = { day: new Date().getDate() + 1, month: new Date().getMonth() + 1, year: new Date().getFullYear() }
    this.nextFollowupTime = { hour: 9, minute: 30, second: 0 };
    this.fromDate = { day: getFromDate.getDate(), month: getFromDate.getMonth() + 1, year: getFromDate.getFullYear() };
    this.toDate = { day: new Date().getDate(), month: new Date().getMonth() + 1, year: new Date().getFullYear() };
    this.adminSetting = <AdminSetting>JSON.parse(localStorage.getItem('adminSetting'));
    this.overDueDays = this.adminSetting.overDueDaysRm;
  }

  ngOnInit(): void {
    if (this.isAdmin == true) {
      this.fillRManagerList();
      this.onChangeFilterDateOption(3);
    }
    else {
      this.fillTeleCallerList();
      this.onChangeFilterDateOption(2);
    }
    this.fillAppoinStatus();
    this.fillLeadListByRM();
  }

  async fillLeadListByRM() {
    this.filterOption.status = this.appoinStatusId;
    if (this.isAdmin == true)
      this.filterOption.allocatedTo = this.filteruserId;
    else
      this.filterOption.createdBy = this.filteruserId;
    this.filterOption.dateFilterBy = this.filterDateById;
    this.filterOption.fromDate = new Date(this.fromDate.year, this.fromDate.month - 1, this.fromDate.day);
    this.filterOption.todate = new Date(this.toDate.year, this.toDate.month - 1, this.toDate.day);
    this.service.TABLE = [];
    this.isLoading = true;
    await this.leadRepo.loadLeadListByRM(this.currentUser.userId, this.filterOption).then(
      (leads) => {
        this.service.xType = new LeadMaster();
        leads.forEach((obj) => {
          obj.isOverDue = obj.appointmentDateTime != null && new Date(obj.appointmentDateTime).getTime() < (new Date().getTime() - (this.overDueDays * 24 * 60 * 60 * 1000)) ? true : false;
          obj.isChecked = false;
        });
        this.service.TABLE = leads;
        this.leadList = this.service.dataList$;
        this.service.searchTerm = '';
        //this.filteredUserList = this.filter.valueChanges.pipe(startWith(''), map(text => search(users, text, this.pipe)));
        this.total$ = this.service.total$;
        this.isLoading = false;
      },
      error => { console.error(error); this.isLoading = false; }
    );
    setTimeout(() => { this.onSelectAllLeads(false); }, 300);
  }

  fillAppoinStatus() {
    this.leadRepo.loadAppoinmentStatusList().subscribe(
      aStatus => {
        this.appoinStatusList = aStatus;
      }, error => console.error(error)
    );
  }

  fillTeleCallerList() {
    this.allocateCreatedBytxt = "Created By";
    this.leadRepo.loadTeleCallerList().subscribe(
      rManager => {
        this.filterUserList = rManager;
      }, error => console.error(error)
    );
  }

  fillRManagerList() {
    this.allocateCreatedBytxt = "Allocated To";
    this.leadRepo.loadRManagerList().subscribe(
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

  followUp(callId: string) {
    //localStorage.removeItem("callIdFollowUp");
    //localStorage.setItem("callIdFollowUp", callId.toString());
    this.router.navigate(['/lead-followup/' + callId]);
  }

  onOpenFollowupModal(callId: number) {
    const modalRef = this.modalService.open(LeadFollowUpComponent, { size: 'lg' });
    modalRef.componentInstance.callId = callId;
    modalRef.result.then((result) => {
      this.fillLeadListByRM();
    });
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
    else if (id == 3) {
      this.filterDateOption = "Last Change Date";
      this.filterDateById = 3;
    }
  }

  onFilterSearch() {
    this.fillLeadListByRM();
  }

  async viewHistory(id: number, firstName: string, lastName: string) {
    this.modalTitle = "Followup History: " + firstName + " " + lastName;
    await this.leadRepo.loadFollowupHistoryById(id).then(
      (history) => {
        this.followupHistory = history;
      },
      error => console.error(error)
    );
  }

  exportAsXLSX(): void {
    let leadArray: LeadMaster[];
    this.leadList.subscribe(data => leadArray = data);
    let ExportleadArray: any[];
    ExportleadArray = leadArray.map(obj => ({
      'First Name': obj.firstName, 'Last Name': obj.lastName, 'Mobile Number': obj.mobileNumber, 'Address': obj.address, 'City': obj.cityName, 'State': obj.stateName, 'Created By': obj.createdByName,
      'Created Date': this.datePipe.transform(obj.createdDate, "dd-MM-yyyy"), 'Status': obj.outComeText, 'Allocated To': obj.allocatedToName, 'Appoinment DateTime': this.datePipe.transform(obj.appointmentDateTime, "dd-MM-yyyy HH:mm a"),
      'Last Changed Date': this.datePipe.transform(obj.lastChangedDate, "dd-MM-yyyy"), 'Remarks': obj.remark
    }));
    this.excelService.exportAsExcelFile(ExportleadArray, 'LeadDetail');
  }

  onSelectAllLeads(isChecked: boolean) {
    this.isCheckedBox = isChecked;
    this.leadList.subscribe(p => p.forEach((obj) => {
      obj.isChecked = this.isCheckedBox;
    }));
    if (this.isCheckedBox) {
      this.leadList.subscribe(p => p.forEach((obj) => {
        this.checkedList.push(obj);
      }));
    }
    else
      this.checkedList = [];
  }

  onDismissAll() {

    this.leadRepo.dismissLeads(this.checkedList).subscribe({
      next: data => (console.log('Success!', data), this.fillLeadListByRM()),
      error: error => (console.error('Error!', error), this.fillLeadListByRM())
    });
  }

  onRemindLater() {
    let date = new Date(this.nextFollowupDate.year, this.nextFollowupDate.month - 1, this.nextFollowupDate.day, this.nextFollowupTime.hour, this.nextFollowupTime.minute, 0, 0);
    let strDate = this.datePipe.transform(date, "dd-MM-yyyy HH:mm a")
    this.leadRepo.remindMelater(this.checkedList, strDate).subscribe({
      next: data => (console.log('Success!', data), this.fillLeadListByRM()),
      error: error => (console.error('Error!', error), this.fillLeadListByRM())
    });

  }

  showHideDismissButton(lead: LeadMaster, event: any) {
    lead.isChecked = event.currentTarget.checked;
    if (lead.isChecked)
      this.checkedList.push(lead);
    else {
      const index = this.checkedList.indexOf(lead);
      if (index > -1) {
        this.checkedList.splice(index, 1);
      }
    }

    if (this.checkedList.length > 0)
      this.isCheckedBox = true;
    else
      this.isCheckedBox = false;

  }

  onCardClose() {
    if (this.isAdmin == true)
      this.router.navigate(['/dashboard']);
    else
      this.router.navigate(['/dash-manager']);
  }

  onDateChange() {
    let date = new Date(this.nextFollowupDate.year, this.nextFollowupDate.month - 1, this.nextFollowupDate.day, this.nextFollowupTime.hour, this.nextFollowupTime.minute, 0, 0);
    this.dateTimeStr = this.datePipe.transform(date, "dd/MM/yyyy hh:mm a");
  }
}
