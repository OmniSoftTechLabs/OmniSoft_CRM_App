import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { NgForm, FormControl, Validators } from '@angular/forms';
import { LeadMaster, OutcomeMaster, AppointmentDetail, StateMaster, CityMaster } from '../models/lead-master';
import { LeadRepositoryService } from '../services/lead-repository.service';
import { UserMaster } from '../models/user-master';
import { AuthenticationService } from '../services/authentication.service';
import { RmanagerMaster } from '../models/rmanager-master';
import { NgbDateStruct, NgbTimeStruct, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { AppoinmentStatus, LeadOutCome } from '../services/generic-enums';
import { AdminSetting } from '../models/admin-setting';
import { Observable, concat, of, Subject } from 'rxjs';
import { distinctUntilChanged, tap, switchMap, catchError } from 'rxjs/operators';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-lead-create',
  templateUrl: './lead-create.component.html',
  styleUrls: ['./lead-create.component.css']
})
export class LeadCreateComponent implements OnInit {

  @ViewChild('leadAdd') form: NgForm;
  @Input() callId: number;

  //callId: number;
  IsSucess: boolean = false;
  IsError: boolean = false;
  formTitle: string = "Create Lead";
  saveBtnTxt: string = "Save";
  is_progress: boolean = false;
  leadModel: LeadMaster = new LeadMaster();
  appointmentDetailObj: AppointmentDetail = new AppointmentDetail();
  outcomeList: OutcomeMaster[] = [];
  rManagerList: RmanagerMaster[] = [];
  is_edit: boolean;
  errorMsg: string;
  successMsg: string;
  currentUser: UserMaster;
  appointmentDate: NgbDateStruct;
  appointmentTime: NgbTimeStruct;
  nextCallDateNg: NgbDateStruct;
  nextCallTimeNg: NgbTimeStruct;
  minDate: NgbDateStruct;
  minuteStep: number = 15;
  isOnDatePickerLoad: boolean = true;
  placement = 'left';
  adminSetting: AdminSetting;
  stateInput = new Subject<string>();
  stateMaster: Observable<StateMaster[]>;
  cityInput = new Subject<string>();
  cityMaster: Observable<CityMaster[]>;
  selectedState: StateMaster;
  selectedCity: CityMaster;
  loading: boolean;
  dateTimeStr: string;

  timeCtrl = new FormControl('', (control: FormControl) => {
    const value = control.value;

    if (!value) {
      return null;
    }

    let datep: NgbDateStruct;
    if (this.appointmentDate != null)
      datep = this.appointmentDate
    if (this.nextCallDateNg != null)
      datep = this.nextCallDateNg

    if (datep != null) {
      let datetime = new Date(datep.year, datep.month - 1, datep.day, value.hour, value.minute, 0, 0);
      this.dateTimeStr = this.datePipe.transform(datetime, "dd/MM/yyyy hh:mm a");
    }

    if (value.hour < 9 || ((value.minute < 30 && value.hour <= 9))) {
      return { tooEarly: true };
    }
    if (value.hour >= 18 && value.minute >= 0) {
      return { tooLate: true };
    }

    return null;
  });

  dateCtrl = new FormControl('', (control: FormControl) => {
    const value = control.value;

    if (!value) {
      return null;
    }

    let timep: NgbTimeStruct;
    if (this.appointmentTime != null)
      timep = this.appointmentTime
    if (this.nextCallTimeNg != null)
      timep = this.nextCallTimeNg

    if (timep != null) {
      let datetime = new Date(value.year, value.month - 1, value.day, timep.hour, timep.minute, 0, 0);
      this.dateTimeStr = this.datePipe.transform(datetime, "dd-MM-yyyy hh:mm a");
    }
  });


  constructor(private leadRepo: LeadRepositoryService, private auth: AuthenticationService, public activeModal: NgbActiveModal, private datePipe: DatePipe) {
    this.auth.currentUser.subscribe(x => this.currentUser = x);
    this.minDate = { day: new Date().getDate(), month: new Date().getMonth() + 1, year: new Date().getFullYear() }
    this.adminSetting = <AdminSetting>JSON.parse(localStorage.getItem('adminSetting'));
    this.minuteStep = this.adminSetting.appoinTimeInterval;
  }

  ngOnInit(): void {
    this.fillOutCome();
    this.fillRManagerList();
    this.searchStateMaster();
    this.searchCityMaster();

    //this.callId = Number(localStorage.getItem("callIdEdit"));

    if (this.callId > 0) {
      this.is_edit = true;
      this.formTitle = "Edit Lead";
      this.leadRepo.getLeadById(this.callId).subscribe(
        data => {
          this.leadModel = data;
          if (data.appointmentDetail.length > 0) {
            this.appointmentDetailObj.relationshipManagerId = data.appointmentDetail[0].relationshipManagerId,
              this.appointmentDate = {
                day: new Date(data.appointmentDetail[0].appointmentDateTime).getDate(),
                month: new Date(data.appointmentDetail[0].appointmentDateTime).getMonth() + 1,
                year: new Date(data.appointmentDetail[0].appointmentDateTime).getFullYear()
              },
              this.appointmentTime = {
                hour: new Date(data.appointmentDetail[0].appointmentDateTime).getHours(),
                minute: new Date(data.appointmentDetail[0].appointmentDateTime).getMinutes(),
                second: 0
              }
          }
          if (data.stateId != null)
            this.leadRepo.getSelectedState(data.stateId).subscribe(state => { this.selectedState = state });
          if (data.cityId != null)
            this.leadRepo.getSelectedCity(data.cityId).subscribe(city => { this.selectedCity = city });
        }, error => console.error('Error!', error));

      //localStorage.removeItem("callIdEdit");
    }
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
        this.rManagerList = rManager;
      }, error => console.error(error)
    );
  }

  searchStateMaster() {
    this.stateMaster = concat(
      of([]), // default items
      this.stateInput.pipe(
        distinctUntilChanged(),
        tap(() => this.loading = true),
        switchMap(term => this.loadStateMaster(term).pipe(

          catchError(() => of([])), // empty list on error
          tap(() => this.loading = false)
        ))
      )
    );
  }

  loadStateMaster(term: string): Observable<StateMaster[]> {
    return this.leadRepo.loadStateMaster(term);
  }

  searchCityMaster() {
    this.cityMaster = concat(
      of([]), // default items
      this.cityInput.pipe(
        distinctUntilChanged(),
        tap(() => this.loading = true),
        switchMap(term => this.loadCityMaster(term).pipe(

          catchError(() => of([])), // empty list on error
          tap(() => this.loading = false)
        ))
      )
    );
  }

  loadCityMaster(term: string): Observable<CityMaster[]> {
    return this.leadRepo.loadCityMaster(this.selectedState.stateId, term);
  }

  onSavelead() {
    this.is_progress = true;
    this.saveBtnTxt = "Saving...";
    if (this.selectedState != null)
      this.leadModel.stateId = this.selectedState.stateId;
    if (this.selectedCity != null)
      this.leadModel.cityId = this.selectedCity.cityId;

    let newAppDateTime: any;
    if (this.appointmentDate != null)
      newAppDateTime = new Date(this.appointmentDate.year, this.appointmentDate.month - 1, this.appointmentDate.day, this.appointmentTime.hour, this.appointmentTime.minute, 0, 0);

    let nextCalldate: any;
    if (this.nextCallDateNg != null)
      nextCalldate = new Date(this.nextCallDateNg.year, this.nextCallDateNg.month - 1, this.nextCallDateNg.day, this.nextCallTimeNg.hour, this.nextCallTimeNg.minute, 0, 0);

    if (this.leadModel.outComeId == LeadOutCome.CallLater) {
      this.leadModel.nextCallDate = nextCalldate;
    }

    if (this.leadModel.outComeId == LeadOutCome.AppoinmentTaken && (this.leadModel.appointmentDetail.length == 0
      || this.leadModel.appointmentDetail[0].relationshipManagerId != this.appointmentDetailObj.relationshipManagerId
      || this.leadModel.appointmentDetail[0].appointmentDateTime != newAppDateTime)) {
      this.appointmentDetailObj.callId = this.leadModel.callId;
      this.appointmentDetailObj.appoinStatusId = AppoinmentStatus.Pending;
      this.appointmentDetailObj.appointmentDateTime = newAppDateTime;
      this.appointmentDetailObj.createdBy = this.currentUser.userId;
      this.appointmentDetailObj.remarks = this.leadModel.remark;
      this.leadModel.appointmentDetail.length = 0;
      this.leadModel.appointmentDetail.push(this.appointmentDetailObj);
    }

    if (this.is_edit == true) {

      this.leadRepo.editlead(this.leadModel).subscribe({
        next: data => (this.successMsg = data, this.IsSucess = true, this.onSaveCompleted()),
        error: error => (this.errorMsg = error.error, this.IsError = true, this.onSaveCompleted())
      });

    }
    else {

      this.leadModel.createdBy = this.currentUser.userId;
      this.leadRepo.createLead(this.leadModel).subscribe({
        next: data => (this.successMsg = data, this.IsSucess = true, this.onSaveCompleted()),
        error: error => (this.errorMsg = error.error, this.IsError = true, this.onSaveCompleted())
      });
    }
  }

  closeAlert() {
    this.IsSucess = false;
    this.IsError = false;
  }

  onSaveCompleted() {
    setTimeout(() => { this.closeAlert(); }, 5000);
    if (!this.is_edit && !this.IsError)
      this.form.reset();
    this.is_progress = false;
    this.saveBtnTxt = "Save";
  }

  onDateChange(value: NgbDateStruct) {
    if (value != null && this.isOnDatePickerLoad == false) {
      if (value.day == this.minDate.day && value.month == this.minDate.month && value.year == this.minDate.year) {
        var setTime = new Date();
        setTime.setMinutes(setTime.getMinutes() + 15);
        let hr = setTime.getHours();
        let min = (Math.round(setTime.getMinutes() / 5) * 5) % 60;

        this.appointmentTime = this.nextCallTimeNg = {
          hour: hr,
          minute: min,
          second: 0
        };

      }
      else {
        this.appointmentTime = this.nextCallTimeNg = {
          hour: 9,
          minute: 30,
          second: 0
        };

      }
    }
    this.isOnDatePickerLoad = false;
  }

  onStateChange(value: any) {
    if (value != null && this.selectedCity != null && value.stateId != this.selectedCity.stateId)
      this.selectedCity = null;
  }

  onStatusChange(value: any) {
    let outComeId = value.currentTarget.value;
    if (outComeId != 4)
      this.leadModel.nextCallDate = null;
  }
}

