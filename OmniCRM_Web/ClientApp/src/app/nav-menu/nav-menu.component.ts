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
    this.auth.currentUser.subscribe(x => this.currentUser = x);
    this.logoSrc = "../../assets/LogoUgam.png";
  }

  get isLoggedIn(): boolean {
    if (this.currentUser != null) {
      this.userName = this.currentUser.firstName;
      if (this.currentUser.logoImage != null)
        this.logoSrc = this.currentUser.logoImage;
      return true;
    }
    return false;
  }

  get isAdminRole(): boolean {
    if (this.currentUser != null) {
      if (this.currentUser.roleId == roles.Admin)
        return true;
    }
    return false;
  }

  get isTeleCaller(): boolean {
    if (this.currentUser != null) {
      if (this.currentUser.roleId == roles["Tele Caller"])
        return true;
    }
    return false;
  }

  get isRManager(): boolean {
    if (this.currentUser != null) {
      if (this.currentUser.roleId == roles["Relationship Manager"])
        return true;
    }
    return false;
  }

  get isSuperUser(): boolean {
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


