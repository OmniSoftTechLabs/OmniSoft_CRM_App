import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { RoleMaster } from '../models/role-master';
import { UserMaster } from '../models/user-master';
import { CreatePwd } from '../models/create-pwd'
import { AuthenticationService } from './authentication.service';

@Injectable({
  providedIn: 'root'
})
export class GeneralRepositoryService {

  http: HttpClient;
  baseUrl: any;
  token: string;

  constructor(_http: HttpClient, @Inject('BASE_URL') _baseUrl: string, private auth: AuthenticationService) {
    this.http = _http;
    this.baseUrl = _baseUrl;
    //this.auth.currentUser.subscribe(x => this.token = x.token);
  }

  loadRolesList() {
    //return this.http.get<RoleModel[]>("/api/Role/ListRolesWithClaims", {
    //  headers: new HttpHeaders({ "Authorization": "Bearer " + this.auth.authorisationToken })
    //}).pipe();

    return this.http.get<RoleMaster[]>(this.baseUrl + 'api/RoleMasters').pipe();
  }

  loadUserList() {
    return this.http.get<UserMaster[]>(this.baseUrl + 'api/UserMasters').pipe();
  }

  createUser(userModel: UserMaster) {
    return this.http.post<UserMaster>(this.baseUrl + 'api/UserMasters', userModel).pipe();
  }

  editUser(userModel: UserMaster) {
    return this.http.put<UserMaster>(this.baseUrl + 'api/UserMasters/' + userModel.userId, userModel).pipe();
  }

  getUserToResetPwd(id) {
    return this.http.get<UserMaster>(this.baseUrl + 'api/UserMasters/' + id).pipe();
  }

  getUser(id) {
    return this.http.get<UserMaster>(this.baseUrl + 'api/UserMasters/' + id).pipe();
  }

  resetPwd(pwd: CreatePwd) {
    return this.http.post<CreatePwd>(this.baseUrl + 'api/UserMasters/ResetPassword', pwd, {
      //headers: new HttpHeaders({ "Authorization": "Bearer " + this.auth.authorisationToken })
      responseType: 'json'
    }).pipe();
  }
}
