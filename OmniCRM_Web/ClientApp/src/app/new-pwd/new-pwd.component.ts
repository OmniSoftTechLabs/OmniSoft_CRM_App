import { Component, OnInit } from '@angular/core';
import { CreatePwd } from '../models/create-pwd';
import { ActivatedRoute } from '@angular/router';
import { GeneralRepositoryService } from '../services/general-repository.service';

@Component({
  selector: 'app-new-pwd',
  templateUrl: './new-pwd.component.html',
  styleUrls: ['./new-pwd.component.css']
})
export class NewPwdComponent implements OnInit {

  txtUserName: string;
  txtPassword: string;
  txtCnfPassword: string;
  userId: string = "";
  public IsSucess: boolean = false;
  public IsError: boolean = false;
  pwd: CreatePwd = new CreatePwd();
  user: any;
  errorMsg: string;
  IsLoginBtn: boolean = false;
  //        Validators.pattern('(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&].{8,}')

  constructor(private generalRepository: GeneralRepositoryService, private route: ActivatedRoute) { }

  ngOnInit(): void {
    if (String(localStorage.getItem("userId") == ""))
      this.userId = this.route.snapshot.params.userId;
    else
      this.userId = String(localStorage.getItem("userId"));
    localStorage.removeItem("userId");


    this.generalRepository.getUserToResetPwd(this.userId).subscribe(
      data => (this.txtUserName = data.email), error => (this.errorMsg = error.error, this.IsError = true, this.IsLoginBtn = true));
  }

  onSavePwd() {
    this.pwd.password = this.txtPassword;
    this.pwd.userId = this.userId;
    this.generalRepository.resetPwd(this.pwd).subscribe(data => (this.IsSucess = true, this.IsLoginBtn = true),
      error => (this.errorMsg = error.error, this.IsError = true, setTimeout(() => { this.closeAlert(); }, 5000)));
  }

  closeAlert() {
    this.IsSucess = false;
    this.IsError = false;
    //this.router.navigate(['/login/']);
  }
}
