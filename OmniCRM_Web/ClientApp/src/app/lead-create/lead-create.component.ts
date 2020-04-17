import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { LeadMaster, OutcomeMaster, AppointmentDetail } from '../models/lead-master';
import { LeadRepositoryService } from '../services/lead-repository.service';
import { UserMaster } from '../models/user-master';
import { AuthenticationService } from '../services/authentication.service';
import { RmanagerMaster } from '../models/rmanager-master';
import { NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
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

  constructor(private leadRepo: LeadRepositoryService, private auth: AuthenticationService) {
    this.auth.currentUser.subscribe(x => this.currentUser = x);
  }

  ngOnInit(): void {
    this.fillOutCome();
    this.fillRManagerList();
    this.callId = Number(localStorage.getItem("callIdEdit"));

    if (this.callId > 0) {
      this.is_edit = true;
      this.formTitle = "Edit Lead";
      this.leadRepo.getLeadById(this.callId).subscribe(
        data => (this.leadModel = data,
          this.appointmentDetailObj.relationshipManagerId = data.appointmentDetail[0].relationshipManagerId,
          this.appointmentDate = {
            day: new Date(data.appointmentDetail[0].appointmentDateTime).getDate(),
            month: new Date(data.appointmentDetail[0].appointmentDateTime).getMonth(),
            year: new Date(data.appointmentDetail[0].appointmentDateTime).getFullYear()
          }
        ), error => console.error('Error!', error));
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

    if (this.leadModel.outComeId == LeadOutCome.AppoinmentTaken) {
      this.appointmentDetailObj.callId = this.leadModel.callId;
      this.appointmentDetailObj.appoinStatusId = AppoinmentStatus.Pending;
      this.appointmentDetailObj.appointmentDateTime.setFullYear(this.appointmentDate.year, this.appointmentDate.month, this.appointmentDate.day);
      this.appointmentDetailObj.createdBy = this.currentUser.userId;
      this.appointmentDetailObj.remarks = this.leadModel.remark;
      this.leadModel.appointmentDetail.push(this.appointmentDetailObj);
    }

    if (this.is_edit == true) {

      this.leadRepo.editlead(this.leadModel).subscribe({
        next: data => (this.successMsg = "Lead Updated Successfully.", this.IsSucess = true, this.onSaveCompleted()),
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
    if (!this.is_edit)
      this.form.reset();
    this.is_progress = false;
    this.saveBtnTxt = "Save";
  }
}

