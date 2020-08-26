import { Component, OnInit, Input } from '@angular/core';
import { AppoinmentStatusMaster, LeadMaster, AppointmentDetail, FollowupHistory, StateMaster, CityMaster } from '../models/lead-master';
import { LeadRepositoryService } from '../services/lead-repository.service';
import { AuthenticationService } from '../services/authentication.service';
import { UserMaster } from '../models/user-master';
import { GenericEnums } from '../services/generic-enums';
import { NgbDateStruct, NgbTimeStruct, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { DatePipe } from '@angular/common';
import { FormControl } from '@angular/forms';
import { AdminSetting } from '../models/admin-setting';
import { ActivatedRoute } from '@angular/router';
import { Subject, Observable, concat, of } from 'rxjs';
import { distinctUntilChanged, tap, switchMap, catchError } from 'rxjs/operators';

@Component({
  selector: 'app-lead-follow-up',
  templateUrl: './lead-follow-up.component.html',
  styleUrls: ['./lead-follow-up.component.css']
})
export class LeadFollowUpComponent implements OnInit {

  @Input() callId: number;
  appoinmentStatusList: AppoinmentStatusMaster[] = [];
  leadModel: LeadMaster = new LeadMaster();
  appointmentDetailObj: AppointmentDetail = new AppointmentDetail();
  folloupHistoryObj: FollowupHistory = new FollowupHistory();
  appointmentDate: NgbDateStruct;
  appointmentTime: NgbTimeStruct;
  currentUser: UserMaster;
  followupTypeList: any[];
  IsSucess: boolean = false;
  IsError: boolean = false;
  formTitle: string = "Follow up Lead";
  saveBtnTxt: string = "Save";
  is_progress: boolean = false;
  errorMsg: string;
  successMsg: string;
  minDate: NgbDateStruct;
  minuteStep: number = 15;
  isOnDatePickerLoad: boolean = true;
  placement = 'left';
  lastAppoinDate: Date;
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

    if (this.appointmentDate != null) {
      let datetime = new Date(this.appointmentDate.year, this.appointmentDate.month - 1, this.appointmentDate.day, value.hour, value.minute, 0, 0);
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
    if (this.appointmentTime != null) {
      let datetime = new Date(value.year, value.month - 1, value.day, this.appointmentTime.hour, this.appointmentTime.minute, 0, 0);
      this.dateTimeStr = this.datePipe.transform(datetime, "dd/MM/yyyy hh:mm a");
    }
    return null;
  });

  constructor(private leadRepo: LeadRepositoryService, private auth: AuthenticationService, private datePipe: DatePipe, private route: ActivatedRoute, public activeModal: NgbActiveModal) {
    this.auth.currentUser.subscribe(x => this.currentUser = x);
    this.minDate = { day: new Date().getDate(), month: new Date().getMonth() + 1, year: new Date().getFullYear() }
    this.adminSetting = <AdminSetting>JSON.parse(localStorage.getItem('adminSetting'));
    this.minuteStep = this.adminSetting.appoinTimeInterval;
  }

  ngOnInit(): void {
    this.fillAppoinmentStatus();
    this.fillFollowupType();
    this.searchStateMaster();
    this.searchCityMaster();
    //this.callId = Number(localStorage.getItem("callIdFollowUp"));
    if (this.callId == null)
      this.callId = this.route.snapshot.params.callId;

    this.leadRepo.getLeadById(this.callId).subscribe(
      data => (this.leadModel = data,
        this.appointmentDetailObj = data.appointmentDetail[0],
        this.lastAppoinDate = this.appointmentDetailObj.appointmentDateTime,
        this.leadRepo.getSelectedState(data.stateId).subscribe(state => { this.selectedState = state }),
        this.leadRepo.getSelectedCity(data.cityId).subscribe(city => { this.selectedCity = city })
        //this.appointmentDate = {
        //  day: new Date(data.appointmentDetail[0].appointmentDateTime).getDate(),
        //  month: new Date(data.appointmentDetail[0].appointmentDateTime).getMonth(),
        //  year: new Date(data.appointmentDetail[0].appointmentDateTime).getFullYear()
        //}
      ), error => (console.error('Error!', error), this.errorMsg = error.error, this.IsError = true, this.onSaveCompleted()));
    //localStorage.removeItem("callIdFollowUp");

  }

  fillAppoinmentStatus() {
    this.leadRepo.loadAppoinmentStatusList().subscribe(
      appoStatus => {
        this.appoinmentStatusList = appoStatus;
      }, error => console.error(error)
    );
  }

  fillFollowupType() {
    this.followupTypeList = this.getFollowupType();
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

    if (this.appointmentDate != null) {
      this.appointmentDetailObj.appointmentDateTime = new Date(this.appointmentDate.year, this.appointmentDate.month - 1, this.appointmentDate.day, this.appointmentTime.hour, this.appointmentTime.minute, 0, 0);
      this.datePipe.transform(this.appointmentDetailObj.appointmentDateTime, "dd-MM-yyyy HH:mm a");
      //this.appointmentDetailObj.appointmentDateTime.setDate(this.appointmentDate.day);
      //this.appointmentDetailObj.appointmentDateTime.setMonth(this.appointmentDate.month);
      //this.appointmentDetailObj.appointmentDateTime.setFullYear(this.appointmentDate.year);
    }
    else
      this.appointmentDetailObj.appointmentDateTime = null;

    this.appointmentDetailObj.remarks = this.leadModel.remark;

    this.folloupHistoryObj.appoinStatusId = this.appointmentDetailObj.appoinStatusId;
    this.folloupHistoryObj.callId = this.leadModel.callId;
    this.folloupHistoryObj.appoinDate = this.lastAppoinDate;
    this.folloupHistoryObj.createdByRmanagerId = this.currentUser.userId;
    this.folloupHistoryObj.remarks = this.leadModel.remark;

    this.leadModel.appointmentDetail[0] = this.appointmentDetailObj;
    this.leadModel.followupHistory.push(this.folloupHistoryObj);

    this.leadRepo.createFollowup(this.leadModel).subscribe({
      next: data => (console.log('Success!', data), this.successMsg = data, this.IsSucess = true, this.onSaveCompleted()),
      error: error => (console.error('Error!', error), this.errorMsg = error.error, this.IsError = true, this.onSaveCompleted())
    });
  }

  closeAlert() {
    this.IsSucess = false;
    this.IsError = false;
  }

  onSaveCompleted() {
    setTimeout(() => { this.closeAlert(); }, 5000);
    this.is_progress = false;
    this.saveBtnTxt = "Save";
  }

  getFollowupType() {
    return [
      { followupType: 'Call' },
      { followupType: 'Visit' },
      { followupType: 'Email' },
      { followupType: 'WhatsApp' },
    ];
  }

  onStateChange(value: any) {
    if (value != null && this.selectedCity != null && value.stateId != this.selectedCity.stateId)
      this.selectedCity = null;
  }

  onDateChange(value: NgbDateStruct) {
    if (value != null && this.isOnDatePickerLoad == false) {
      if (value.day == this.minDate.day && value.month == this.minDate.month && value.year == this.minDate.year) {
        var setTime = new Date();
        setTime.setMinutes(setTime.getMinutes() + 15);
        let hr = setTime.getHours();
        let min = (Math.round(setTime.getMinutes() / 5) * 5) % 60;

        this.appointmentTime = {
          hour: hr,
          minute: min,
          second: 0
        };

      }
      else {
        this.appointmentTime = {
          hour: 9,
          minute: 30,
          second: 0
        };

      }
    }

    this.isOnDatePickerLoad = false;
  }
}
