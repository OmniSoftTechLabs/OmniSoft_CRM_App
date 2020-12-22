import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { CompanyMaster } from '../models/company-master';
import { UserMaster } from '../models/user-master';
import { GeneralRepositoryService } from '../services/general-repository.service';
import { roles } from '../services/generic-enums';

@Component({
  selector: 'app-company-master',
  templateUrl: './company-master.component.html',
  styleUrls: ['./company-master.component.css']
})
export class CompanyMasterComponent implements OnInit {

  @ViewChild('companyAdd') form: NgForm;
  IsSucess: boolean = false;
  IsError: boolean = false;
  is_progress: boolean = false;
  errorMsg: string;
  successMsg: string;
  saveBtnTxt: string = "Save";
  companyModel: CompanyMaster = new CompanyMaster();
  userModel: UserMaster = new UserMaster();

  constructor(private generalRepository: GeneralRepositoryService) { }

  ngOnInit(): void {
  }

  onSaveCompany() {
    this.is_progress = true;
    this.saveBtnTxt = "Saving...";
    this.userModel.roleId = roles.Admin;
    this.companyModel.userMaster.push(this.userModel);

    this.generalRepository.createCompany(this.companyModel).subscribe({
      next: data => (this.successMsg = "Company Created Successfully..", this.IsSucess = true, this.onSaveCompleted()),
      error: error => (this.errorMsg = error.error, this.IsError = true, this.onSaveCompleted())
    });
  }

  onSaveCompleted() {
    setTimeout(() => { this.closeAlert(); }, 5000);
    this.is_progress = false;
    this.saveBtnTxt = "Save";
  }

  closeAlert() {
    this.IsSucess = false;
    this.IsError = false;
  }
}
