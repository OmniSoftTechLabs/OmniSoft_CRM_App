import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm, FormControl } from '@angular/forms';
import { LeadMaster, OutcomeMaster, AppointmentDetail } from '../models/lead-master';
import { LeadRepositoryService } from '../services/lead-repository.service';
import { UserMaster } from '../models/user-master';
import { AuthenticationService } from '../services/authentication.service';
import { RmanagerMaster } from '../models/rmanager-master';
import { NgbDateStruct, NgbTimeStruct } from '@ng-bootstrap/ng-bootstrap';
import { AppoinmentStatus, LeadOutCome } from '../services/generic-enums';

@Component({
  selector: 'app-lead-create',
  templateUrl: './lead-create.component.html',
  styleUrls: ['./lead-create.component.css']
})
export class LeadCreateComponent implements OnInit {

  @ViewChild('leadAdd') form: NgForm;
  callId: number;
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
  minDate: NgbDateStruct;
  minuteStep: number = 15;
  isOnDatePickerLoad: boolean = true;
  placement = 'left';

  timeCtrl = new FormControl('', (control: FormControl) => {
    const value = control.value;

    if (!value) {
      return null;
    }

    if (value.hour <= 9) {
      if (value.minute < 30 || value.hour <= 9)
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
  });


  constructor(private leadRepo: LeadRepositoryService, private auth: AuthenticationService) {
    this.auth.currentUser.subscribe(x => this.currentUser = x);
    this.minDate = { day: new Date().getDate(), month: new Date().getMonth() + 1, year: new Date().getFullYear() }
  }

  ngOnInit(): void {
    this.fillOutCome();
    this.fillRManagerList();
    this.callId = Number(localStorage.getItem("callIdEdit"));

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
        }, error => console.error('Error!', error));
      localStorage.removeItem("callIdEdit");
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

  onSavelead() {
    this.is_progress = true;
    this.saveBtnTxt = "Saving...";
    const newAppDateTime = new Date(this.appointmentDate.year, this.appointmentDate.month - 1, this.appointmentDate.day, this.appointmentTime.hour, this.appointmentTime.minute, 0, 0);
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

