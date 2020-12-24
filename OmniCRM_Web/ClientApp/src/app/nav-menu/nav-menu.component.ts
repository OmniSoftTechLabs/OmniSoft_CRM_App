import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';
import { AuthenticationService } from '../services/authentication.service';
import { UserMaster } from '../models/user-master';
import { roles } from '../services/generic-enums';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {
  isExpanded = false;
  currentUser: UserMaster;
  userName: string;
  logoSrc: string;
  constructor(private router: Router, private auth: AuthenticationService) {
    this.logoSrc = "../../assets/OmniCRM-Logo.png";
  }

  get isLoggedIn(): boolean {
    this.auth.currentUser.subscribe(x => this.currentUser = x);
    if (this.currentUser != null) {
      this.userName = this.currentUser.firstName;
      this.logoSrc = this.currentUser.logoImage;
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

  get isTeleCaller(): boolean {
    this.auth.currentUser.subscribe(x => this.currentUser = x);
    if (this.currentUser != null) {
      if (this.currentUser.roleId == roles["Tele Caller"])
        return true;
    }
    return false;
  }

  get isRManager(): boolean {
    this.auth.currentUser.subscribe(x => this.currentUser = x);
    if (this.currentUser != null) {
      if (this.currentUser.roleId == roles["Relationship Manager"])
        return true;
    }
    return false;
  }

  get isSuperUser(): boolean {
    this.auth.currentUser.subscribe(x => this.currentUser = x);
    if (this.currentUser != null) {
      if (this.currentUser.roleId == roles["Super User"])
        return true;
    }
    return false;
  }

  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
    this.logoSrc = "../../assets/OmniCRM-Logo.png";
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  onBrandClick() {
    this.auth.currentUser.subscribe(x => this.currentUser = x);
    if (this.currentUser != null) {
      if (this.currentUser.roleId == roles["Tele Caller"])
        this.router.navigate(['/dash-tele']);
      else if (this.currentUser.roleId == roles["Relationship Manager"])
        this.router.navigate(['/dash-manager']);
      else
        this.router.navigate(['/dashboard']);
    }
  }
}


