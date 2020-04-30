import { Component, OnInit, ViewChild } from '@angular/core';
import { UserMaster } from '../models/user-master';
import { AuthenticationService } from '../services/authentication.service';
import { ChangePwd } from '../models/change-pwd';
import { NgForm } from '@angular/forms';
import { GeneralRepositoryService } from '../services/general-repository.service';
import { roles } from '../services/generic-enums';
import { Router } from '@angular/router';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.css']
})
export class ChangePasswordComponent implements OnInit {

  currentUser: UserMaster;
  chngPwd: ChangePwd = new ChangePwd();
  errorMsg: string = "";
  successMsg: string = "";
  is_progress: boolean = false;
  IsSucess: boolean = false;
  IsError: boolean = false;
  saveBtnTxt: string = "Save";
  txtOldPassword: string;
  txtNewPassword: string;
  txtCnfPassword: string;
  @ViewChild('changePwd') form: NgForm;


  constructor(private auth: AuthenticationService, private generalRepository: GeneralRepositoryService, private router: Router) {
    this.auth.currentUser.subscribe(x => this.currentUser = x);
  }

  ngOnInit(): void {

  }

  onSave() {

    this.is_progress = true;
    this.saveBtnTxt = "Saving...";

    this.chngPwd.oldPassword = this.txtOldPassword;
    this.chngPwd.newPassword = this.txtNewPassword;
    this.chngPwd.userId = this.currentUser.userId;

    this.generalRepository.changePassword(this.chngPwd).subscribe(
      data => (this.IsSucess = true, this.successMsg = data, this.onSaveCompleted()),
      error => (this.IsError = true, this.errorMsg = error.error, this.onSaveCompleted()));
  }

  closeAlert() {
    this.IsSucess = false;
    this.IsError = false;
  }

  onSaveCompleted() {
    setTimeout(() => { this.closeAlert(); }, 5000);
    this.form.reset();
    this.is_progress = false;
    this.saveBtnTxt = "Save";
  }

  onCloseWindow() {
    if (this.currentUser.roleId == roles["Tele Caller"])
      this.router.navigate(['/dash-tele']);
    else if (this.currentUser.roleId == roles["Relationship Manager"])
      this.router.navigate(['/dash-manager']);
    else
      this.router.navigate(['/dashboard']);

  }
}
