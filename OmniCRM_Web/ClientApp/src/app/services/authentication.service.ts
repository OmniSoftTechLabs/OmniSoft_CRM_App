import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { UserMaster } from '../models/user-master';

@Injectable({ providedIn: 'root' })
export class AuthenticationService {
  private currentUserSubject: BehaviorSubject<any>;
  public currentUser: Observable<any>;
  baseUrl: any;
  userMaster: UserMaster;

  constructor(private http: HttpClient, @Inject('BASE_URL') _baseUrl: string) {
    this.currentUserSubject = new BehaviorSubject<any>(JSON.parse(localStorage.getItem('currentUser')));
    this.currentUser = this.currentUserSubject.asObservable();
    this.baseUrl = _baseUrl;
  }

  public get currentUserValue() {
    return this.currentUserSubject.value;
  }

  login(credentials) {
    return this.http.post<UserMaster>(this.baseUrl + 'api/UserMasters/CheckLogin', credentials)
      .pipe(map(user => {
        // store user details and jwt token in local storage to keep user logged in between page refreshes
        localStorage.setItem('currentUser', JSON.stringify(user));
        this.userMaster = user;
        this.currentUserSubject.next(user);
        return user;
      }));
  }

  roleMatch(allowedRoles): boolean {
    this.currentUser.subscribe(x => this.userMaster = x);
    var isMatch = false;
    var payLoad = JSON.parse(window.atob(this.userMaster.token.split('.')[1]));
    var userRole = payLoad.role;
    allowedRoles.forEach(element => {
      if (userRole == element)
        isMatch = true;
      return false;
    });
    return isMatch;
  }

  logout() {
    // remove user from local storage and set current user to null
    localStorage.removeItem('currentUser');
    localStorage.removeItem('adminSetting');
    this.currentUserSubject.next(null);
  }
}
