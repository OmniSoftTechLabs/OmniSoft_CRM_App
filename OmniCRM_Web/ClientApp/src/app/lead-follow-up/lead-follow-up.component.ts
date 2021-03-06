import { Component, OnInit } from '@angular/core';
import { AppoinmentStatusMaster, LeadMaster, AppointmentDetail, FollowupHistory } from '../models/lead-master';
import { LeadRepositoryService } from '../services/lead-repository.service';
import { AuthenticationService } from '../services/authentication.service';
import { UserMaster } from '../models/user-master';
import { GenericEnums } from '../services/generic-enums';
import { NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { DatePipe } from '@angular/common';

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


  constructor(private leadRepo: LeadRepositoryService, private auth: AuthenticationService, private datePipe: DatePipe) {
    this.auth.currentUser.subscribe(x => this.currentUser = x);
    this.minDate = { day: new Date().getDate(), month: new Date().getMonth() + 1, year: new Date().getFullYear() }

  }

  ngOnInit(): void {
    this.fillAppoinmentStatus();
    this.fillFollowupType();
    this.callId = Number(localStorage.getItem("callIdFollowUp"));

    this.leadRepo.getLeadById(this.callId).subscribe(
      data => (this.leadModel = data,
        this.appointmentDetailObj = data.appointmentDetail[0]
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
      this.appointmentDetailObj.appointmentDateTime = new Date(this.appointmentDate.year, this.appointmentDate.month - 1, this.appointmentDate.day, 10, 0, 0, 0);
      this.datePipe.transform(this.appointmentDetailObj.appointmentDateTime, "dd-MM-yyyy HH:mm a");
      //this.appointmentDetailObj.appointmentDateTime.setDate(this.appointmentDate.day);
      //this.appointmentDetailObj.appointmentDateTime.setMonth(this.appointmentDate.month);
      //this.appointmentDetailObj.appointmentDateTime.setFullYear(this.appointmentDate.year);
    }
    this.appointmentDetailObj.remarks = this.leadModel.remark;

    this.folloupHistoryObj.appoinStatusId = this.appointmentDetailObj.appoinStatusId;
    this.folloupHistoryObj.callId = this.leadModel.callId;
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
}
