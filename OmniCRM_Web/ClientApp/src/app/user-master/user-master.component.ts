import { Component, OnInit, ViewChild } from '@angular/core';
import { UserMaster } from '../models/user-master';
import { RoleMaster } from '../models/role-master';
import { GeneralRepositoryService } from '../services/general-repository.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-user-master',
  templateUrl: './user-master.component.html',
  styleUrls: ['./user-master.component.css']
})
export class UserMasterComponent implements OnInit {

  @ViewChild('userAdd') form: any;
  IsSucess: boolean = false;
  IsError: boolean = false;
  is_edit: boolean;
  is_progress: boolean = false;
  errorMsg: string;
  successMsg: string;
  saveBtnTxt: string = "Save";
  userModel: UserMaster = new UserMaster();
  roleList: RoleMaster[] = [];

  constructor(private generalRepository: GeneralRepositoryService, private router: Router) { }

  ngOnInit(): void {
    this.fillRoles();
  }

  fillRoles() {
    this.generalRepository.loadRolesList().subscribe(
      roles => {
        this.roleList = roles;
      }, error => console.error(error)
    );
  }

  closeAlert() {
    this.IsSucess = false;
    this.IsError = false;
  }

  onSaveUser() {
    //this.generalRepository.createUser(this.userModel).subscribe(
    //  data => (this.IsSucess = true, setTimeout(() => { this.closeAlert(); }, 5000)),
    //  error => (this.errorMsg = error.error, this.IsError = true, setTimeout(() => { this.closeAlert(); }, 5000))
    //);
    this.is_progress = true;
    this.saveBtnTxt = "Saving...";

    this.generalRepository.createUser(this.userModel).subscribe({
      next: data => (this.successMsg = "User Saved Successfully. Activation link has been sent to the given email.", this.IsSucess = true, this.onSaveCompleted()),
      error: error => (this.errorMsg = error.error, this.IsError = true, this.onSaveCompleted())
    });

  }

  onSaveCompleted() {
    setTimeout(() => { this.closeAlert(); }, 5000);
    this.form.reset();
    this.is_progress = false;
    this.saveBtnTxt = "Save";
  }
}
