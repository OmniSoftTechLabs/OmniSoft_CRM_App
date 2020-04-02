import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';
import { AuthenticationService } from '../services/authentication.service';
import { UserMaster } from '../models/user-master';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {
  isExpanded = false;
  currentUser: UserMaster;
  userName: string;

  constructor(private router: Router, private auth: AuthenticationService) {

  }

  get isLoggedIn(): boolean {
    this.auth.currentUser.subscribe(x => this.currentUser = x);
    if (this.currentUser != null) {
      this.userName = this.currentUser.firstName;
      return true;
    }
    return false;
  }

  get isAdminRole(): boolean {
    this.auth.currentUser.subscribe(x => this.currentUser = x);
    if (this.currentUser != null) {
      if (this.currentUser.roleId == roles.Admin)
        return true;
    }
    return false;
  }

  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }


}

enum roles {
  'Admin' = 1,
  'Tele Caller' = 2,
  'Relationship Manager' = 3

}