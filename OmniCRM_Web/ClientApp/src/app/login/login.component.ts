import { Component, OnInit, ViewChild } from '@angular/core';
import { faUser } from '@fortawesome/free-solid-svg-icons';
import { faLock } from '@fortawesome/free-solid-svg-icons';
import { AuthenticationService } from '../services/authentication.service';
import { Router, ActivatedRoute } from '@angular/router';
import { error } from '@angular/compiler/src/util';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  loginError: string = "";
  returnUrl: string;
  isProgress: boolean = false;

  public credentials = {
    username: "admin@ostechlabs.com",
    password: "ostech#852"
  };

  constructor(private auth: AuthenticationService, private router: Router, private route: ActivatedRoute, ) {
    this.auth.logout();
  }

  ngOnInit(): void {
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/dashboard';
  }

  login(): void {
    this.isProgress = true;
    this.loginError = "";
    //this.auth.login(this.credentials)
    //  .subscribe(success => {
    //    if (success) {
    //      this.router.navigate(["user-detail"]);
    //    }
    //  }, err => { this.loginError = "Login failed!" });

    this.auth.login(this.credentials).subscribe({
      next: success => (this.router.navigate([this.returnUrl]), this.isProgress = false),
      error: error => (this.loginError = error.error, this.isProgress = false)
    })

  }
}
