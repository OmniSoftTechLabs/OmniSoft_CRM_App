import { Component, OnInit, ViewChildren, QueryList, ViewChild, ElementRef } from '@angular/core';
import { Observable } from 'rxjs';
import { LeadMaster, OutcomeMaster, CallTransactionDetail } from '../models/lead-master';
import { FormControl, Validators } from '@angular/forms';
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
import { NgbDateStruct, NgbModal, NgbModalConfig } from '@ng-bootstrap/ng-bootstrap';
import { FilterOptions } from '../models/filter-options';
import { delay } from 'rxjs/operators';
import { LeadCreateComponent } from '../lead-create/lead-create.component';

@Component({
  selector: 'app-lead-list',
  templateUrl: './lead-list.component.html',
  styleUrls: ['./lead-list.component.css']
})
export class LeadListComponent implements OnInit {

  currentUser: UserMaster;
  outcomeList: OutcomeMaster[] = [];
  filterUserList: RmanagerMaster[] = [];
  leadList: Observable<LeadMaster[]>;
  callHistory: CallTransactionDetail[] = [];
  total$: Observable<number>;
  filter = new FormControl('');
  isUploadSucc: boolean = undefined;
  outComeId: number[] = [];
  filteruserId: string = "0";
  filterDateOption: string = "Created Date";
  filterDateById: number;
  fromDate: NgbDateStruct;
  toDate: NgbDateStruct;
  filterOption: FilterOptions = new FilterOptions();
  modalTitle: string = "";
  uploadMsg: string = "";
  isCheckedBox: boolean;
  isAdmin: boolean;
  checkedList: LeadMaster[] = [];
  minDate: NgbDateStruct;

  @ViewChild('labelImport') labelImport: ElementRef;
  @ViewChild('fileInput') fileInput;
  @ViewChildren(NgbdSortableHeader) headers: QueryList<NgbdSortableHeader>;

  dateCtrl = new FormControl('', Validators.required);

  constructor(public service: DataTableService, private leadRepo: LeadRepositoryService, private router: Router, private auth: AuthenticationService,
    private excelService: ExcelExportService, private datePipe: DatePipe, private modalService: NgbModal, private modalConfig: NgbModalConfig) {
    this.auth.currentUser.subscribe(x => this.currentUser = x);
    modalConfig.backdrop = 'static';
    modalConfig.keyboard = false;
    let getFromDate = new Date(new Date().getTime() - (7 * 24 * 60 * 60 * 1000));
    this.fromDate = { day: getFromDate.getDate(), month: getFromDate.getMonth() + 1, year: getFromDate.getFullYear() };
    this.toDate = { day: new Date().getDate(), month: new Date().getMonth() + 1, year: new Date().getFullYear() };
  }

  ngOnInit(): void {
    this.onChangeFilterDateOption(1);
    this.fillLeadListCreatedBy();
    this.fillOutCome();
    if (this.isAdmin == true)
      this.fillTeleCallerList();
    else
      this.fillRManagerList();
  }

  async fillLeadListCreatedBy() {
    this.filterOption.status = this.outComeId;
    this.filterOption.allocatedTo = this.filteruserId;
    this.filterOption.dateFilterBy = this.filterDateById;
    this.filterOption.fromDate = new Date(this.fromDate.year, this.fromDate.month - 1, this.fromDate.day);
    this.filterOption.todate = new Date(this.toDate.year, this.toDate.month - 1, this.toDate.day);
    this.service.TABLE = [];
    this.checkedList = [];
    await this.leadRepo.loadLeadListByCreatedBy(this.currentUser.userId, this.filterOption).then(
      (leads) => {
        this.service.xType = new LeadMaster();
        leads.forEach((obj) => { obj.isChecked = false; });
        this.service.TABLE = leads;
        this.leadList = this.service.dataList$;
        this.service.searchTerm = '';
        //this.filteredUserList = this.filter.valueChanges.pipe(startWith(''), map(text => search(users, text, this.pipe)));
        this.total$ = this.service.total$;
      },
      error => console.error(error)
    );
    setTimeout(() => { this.onSelectAllLeads(false); }, 300);

  }

  fillOutCome() {
    this.leadRepo.loadOutcomeList().subscribe(
      outCome => {
        this.outcomeList = outCome;
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

  onOpenLeadModal(callId: number) {
    const modalRef = this.modalService.open(LeadCreateComponent, { size: 'lg' });
    modalRef.componentInstance.callId = callId;
    modalRef.result.then((result) => {
      this.fillLeadListCreatedBy();
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
    this.fillLeadListCreatedBy();
  }

  async viewHistory(id: number, firstName: string, lastName: string) {
    this.modalTitle = "Call History: " + firstName + " " + lastName;
    await this.leadRepo.loadCallTransById(id).then(
      (history) => {
        this.callHistory = history;
      },
      error => console.error(error)
    );
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
    ExportleadArray = leadArray.map(obj => ({
      'First Name': obj.firstName, 'Last Name': obj.lastName, 'Mobile Number': obj.mobileNumber, 'Address': obj.address, 'City': obj.cityName, 'State': obj.stateName, 'Created Date': this.datePipe.transform(obj.createdDate, "dd-MM-yyyy"),
      'Status': obj.outComeText, 'Allocated To': obj.allocatedToName, 'Appoinment DateTime': this.datePipe.transform(obj.appointmentDateTime, "dd-MM-yyyy HH:mm a"), 'Remarks': obj.remark
    }));

    this.excelService.exportAsExcelFile(ExportleadArray, 'LeadDetail');
  }

  onFileChange(files: FileList) {
    this.labelImport.nativeElement.innerText = files[0].name;
  }

  onUploadFile() {
    let formData = new FormData();
    formData.append('upload', this.fileInput.nativeElement.files[0]);
    this.leadRepo.uploadExcelData(this.currentUser.userId, formData).subscribe({
      next: data => (this.uploadMsg = data, this.isUploadSucc = true, this.fillLeadListCreatedBy()),
      error: error => (console.error(error), this.uploadMsg = error.error, this.isUploadSucc = false)
    });
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
    if (this.currentUser.roleId == roles["Tele Caller"])
      this.router.navigate(['/dash-tele']);
    else if (this.currentUser.roleId == roles["Relationship Manager"])
      this.router.navigate(['/dash-manager']);
    else
      this.router.navigate(['/dashboard']);
  }

  //setDeleteLeadId(callId: string) {
  //  localStorage.removeItem("callIdDelete");
  //  localStorage.setItem("callIdDelete", callId.toString());
  //}

  onDeleteLead(isDelete: boolean) {
    //let callId = Number(localStorage.getItem("callIdDelete"));
    if (isDelete == true) {
      this.leadRepo.deleteLead(this.checkedList).subscribe({
        next: data => (console.log('Success!', data), this.fillLeadListCreatedBy()),
        error: error => (console.error('Error!', error), this.fillLeadListCreatedBy())
      });
    }
  }
}
