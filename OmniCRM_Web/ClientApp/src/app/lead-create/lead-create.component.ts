import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { LeadMaster, OutcomeMaster } from '../models/lead-master';
import { LeadRepositoryService } from '../services/lead-repository.service';
import { UserMaster } from '../models/user-master';
import { AuthenticationService } from '../services/authentication.service';

@Component({
  selector: 'app-lead-create',
  templateUrl: './lead-create.component.html',
  styleUrls: ['./lead-create.component.css']
})
export class LeadCreateComponent implements OnInit {

  @ViewChild('leadAdd') form: NgForm;
  IsSucess: boolean = false;
  IsError: boolean = false;
  formTitle: string = "Create Lead";
  saveBtnTxt: string = "Save";
  is_progress: boolean = false;
  leadModel: LeadMaster = new LeadMaster();
  outcomeList: OutcomeMaster[] = [];
  is_edit: boolean;
  errorMsg: string;
  successMsg: string;
  currentUser: UserMaster;


  constructor(private leadRepo: LeadRepositoryService, private auth: AuthenticationService) {
    this.auth.currentUser.subscribe(x => this.currentUser = x);
  }

  ngOnInit(): void {
    this.fillOutCome();
  }

  fillOutCome() {
    this.leadRepo.loadOutcomeList().subscribe(
      outCome => {
        this.outcomeList = outCome;
      }, error => console.error(error)
    );
  }

  onSavelead() {
    this.is_progress = true;
    this.saveBtnTxt = "Saving...";

    if (this.is_edit == true) {
      //this.leadRepo.editlead(this.userModel).subscribe({
      //  next: data => (this.successMsg = "User Updated Successfully.", this.IsSucess = true, this.onSaveCompleted()),
      //  error: error => (this.errorMsg = error.error, this.IsError = true, this.onSaveCompleted())
      //});

    }
    else {
      this.leadModel.createdBy = this.currentUser.userId
      this.leadRepo.createLead(this.leadModel).subscribe({
        next: data => (this.successMsg = data.toString(), this.IsSucess = true, this.onSaveCompleted()),
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
