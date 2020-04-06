import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { GeneralRepositoryService } from '../services/general-repository.service';

@Component({
  selector: 'app-forget-password',
  templateUrl: './forget-password.component.html',
  styleUrls: ['./forget-password.component.css']
})
export class ForgetPasswordComponent implements OnInit {

  @ViewChild('forgotPwd') form: NgForm;

  isError: boolean = false;
  isSucess: boolean = false;
  isProgress: boolean = false;
  message: string = "";
  emailTxt: string = "";

  constructor(private generalRepository: GeneralRepositoryService) { }

  ngOnInit(): void {
  }

  sendEmail(email: string) {
    this.isError = false;
    this.isSucess = false;
    if (email != '') {
      this.isProgress = true;
      this.form.reset();
      this.generalRepository.forgotPassword(email).subscribe(
        //data => (this.toastr.info(data)),
        //error => (this.toastr.error(error.error))
        data => (this.isSucess = true, this.message = data, this.onComplete()),
        error => (this.isError = true, this.message = error.error, this.onComplete()),
      );
    }
  }

  onComplete() {
    this.isProgress = false;
    this.form.reset();
  }
}
