import { Component, OnInit } from '@angular/core';
import { AdminSetting } from '../models/admin-setting';
import { NgbTimeStruct } from '@ng-bootstrap/ng-bootstrap';
import { GeneralRepositoryService } from '../services/general-repository.service';
import { UserMaster } from '../models/user-master';
import { AuthenticationService } from '../services/authentication.service';
import { Time, DatePipe } from '@angular/common';

@Component({
  selector: 'app-admin-setting',
  templateUrl: './admin-setting.component.html',
  styleUrls: ['./admin-setting.component.css']
})
export class AdminSettingComponent implements OnInit {

  IsSucess: boolean = false;
  IsError: boolean = false;
  is_progress: boolean = false;
  settingModel: AdminSetting = new AdminSetting();
  oldSetting: AdminSetting = new AdminSetting();
  minuteStep: number = 15;
  dailyEmailTime: NgbTimeStruct;

  saveBtnTxt: string = "Save";
  errorMsg: string;
  successMsg: string;
  currentUser: UserMaster;

  constructor(private generalRepository: GeneralRepositoryService, private auth: AuthenticationService, private datePipe: DatePipe) {
    this.auth.currentUser.subscribe(x => this.currentUser = x);

  }

  ngOnInit(): void {
    this.generalRepository.getAdminSetting().subscribe(

      data => (this.settingModel = data, this.oldSetting = Object.assign({}, data), this.dailyEmailTime = { hour: new Date(data.dailyEmailTime).getHours(), minute: new Date(data.dailyEmailTime).getMinutes(), second: 0 }),
      error => (console.error('Error!', error), this.errorMsg = error.statusText, this.IsError = true)
    );


  }

  onSaveSetting() {
    this.is_progress = true;
    this.saveBtnTxt = "Saving...";
    this.settingModel.createdBy = this.currentUser.userId;
    this.settingModel.dailyEmailTime = new Date(2000, 0, 1, this.dailyEmailTime.hour, this.dailyEmailTime.minute, 0, 0);
    this.oldSetting.dailyEmailTime = new Date(this.oldSetting.dailyEmailTime);

    let oldJson = JSON.stringify(this.oldSetting);
    let newJson = JSON.stringify(this.settingModel);

    if (newJson != oldJson) {

      this.generalRepository.postAdminSetting(this.settingModel).subscribe({
        next: data => (this.successMsg = data, this.IsSucess = true, this.onSaveCompleted()),
        error: error => (this.errorMsg = error.error, this.IsError = true, this.onSaveCompleted())
      });
    }
    else {
      this.errorMsg = "This setting is already exist!";
      this.IsError = true;
      this.onSaveCompleted();
    }
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
}
