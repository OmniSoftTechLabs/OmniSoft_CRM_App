import { Component, OnInit } from '@angular/core';
import { AppoinmentStatusMaster, LeadMaster, AppointmentDetail, FollowupHistory } from '../models/lead-master';
import { LeadRepositoryService } from '../services/lead-repository.service';
import { AuthenticationService } from '../services/authentication.service';
import { UserMaster } from '../models/user-master';
import { GenericEnums } from '../services/generic-enums';
import { NgbDateStruct, NgbTimeStruct } from '@ng-bootstrap/ng-bootstrap';
import { DatePipe } from '@angular/common';
import { FormControl } from '@angular/forms';
import { AdminSetting } from '../models/admin-setting';

@Component({
  selector: 'app-lead-follow-up',
  templateUrl: './lead-follow-up.component.html',
  styleUrls: ['./lead-follow-up.component.css']
})
export class LeadFollowUpComponent implements OnInit {

  appoinmentStatusList: AppoinmentStatusMaster[] = [];
  leadModel: LeadMaster = new LeadMaster();
  appointmentDetailObj: AppointmentDetail = new AppointmentDetail();
  folloupHistoryObj: FollowupHistory = new FollowupHistory();
  appointmentDate: NgbDateStruct;
  appointmentTime: NgbTimeStruct;
  currentUser: UserMaster;
  followupTypeList: any[];
  callId: number;
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

  timeCtrl = new FormControl('', (control: FormControl) => {
    const value = control.value;

    if (!value) {
      return null;
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

    return null;
  });

  constructor(private leadRepo: LeadRepositoryService, private auth: AuthenticationService, private datePipe: DatePipe) {
    this.auth.currentUser.subscribe(x => this.currentUser = x);
    this.minDate = { day: new Date().getDate(), month: new Date().getMonth() + 1, year: new Date().getFullYear() }
    this.adminSetting = <AdminSetting>JSON.parse(localStorage.getItem('adminSetting'));
    this.minuteStep = this.adminSetting.appoinTimeInterval;
  }

  ngOnInit(): void {
    this.fillAppoinmentStatus();
    this.fillFollowupType();
    this.callId = Number(localStorage.getItem("callIdFollowUp"));

    this.leadRepo.getLeadById(this.callId).subscribe(
      data => (this.leadModel = data,
        this.appointmentDetailObj = data.appointmentDetail[0],
        this.lastAppoinDate = this.appointmentDetailObj.appointmentDateTime
        //this.appointmentDate = {
        //  day: new Date(data.appointmentDetail[0].appointmentDateTime).getDate(),
        //  month: new Date(data.appointmentDetail[0].appointmentDateTime).getMonth(),
        //  year: new Date(data.appointmentDetail[0].appointmentDateTime).getFullYear()
        //}
      ), error => console.error('Error!', error));
    localStorage.removeItem("callIdFollowUp");

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

  onSavelead() {
    this.is_progress = true;
    this.saveBtnTxt = "Saving...";

    if (this.appointmentDate != null) {
      this.appointmentDetailObj.appointmentDateTime = new Date(this.appointmentDate.year, this.appointmentDate.month - 1, this.appointmentDate.day, this.appointmentTime.hour, this.appointmentTime.minute, 0, 0);
      this.datePipe.transform(this.appointmentDetailObj.appointmentDateTime, "dd-MM-yyyy HH:mm a");
      //this.appointmentDetailObj.appointmentDateTime.setDate(this.appointmentDate.day);
      //this.appointmentDetailObj.appointmentDateTime.setMonth(this.appointmentDate.month);
      //this.appointmentDetailObj.appointmentDateTime.setFullYear(this.appointmentDate.year);
    }
    this.appointmentDetailObj.remarks = this.leadModel.remark;

    this.folloupHistoryObj.appoinStatusId = this.appointmentDetailObj.appoinStatusId;
    this.folloupHistoryObj.callId = this.leadModel.callId;
    this.folloupHistoryObj.appoinDate = this.lastAppoinDate;
    this.folloupHistoryObj.createdByRmanagerId = this.currentUser.userId;
    this.folloupHistoryObj.remarks = this.leadModel.remark;

    this.leadModel.appointmentDetail[0] = this.appointmentDetailObj;
    this.leadModel.followupHistory.push(this.folloupHistoryObj);

    this.leadRepo.createFollowup(this.leadModel).subscribe({
      next: data => (this.successMsg = data, this.IsSucess = true, this.onSaveCompleted()),
      error: error => (this.errorMsg = error.error, this.IsError = true, this.onSaveCompleted())
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
