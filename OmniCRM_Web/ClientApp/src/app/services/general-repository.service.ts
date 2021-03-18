import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { RoleMaster } from '../models/role-master';
import { UserMaster } from '../models/user-master';
import { CreatePwd } from '../models/create-pwd'
import { ChangePwd } from '../models/change-pwd';
import { AdminSetting } from '../models/admin-setting';
import { ProductMaster } from '../models/product-master';
import { CompanyMaster } from '../models/company-master';
import { TargetMaster, TargetMatrix } from '../models/target-master';

@Injectable({
  providedIn: 'root'
})
export class GeneralRepositoryService {

  http: HttpClient;
  baseUrl: string;

  constructor(_http: HttpClient, @Inject('BASE_URL') _baseUrl: string) {
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
    return this.http.get<UserMaster>(this.baseUrl + 'api/UserMasters/GetUserToResetPwd/' + id).pipe();
  }

  getUser(id) {
    return this.http.get<UserMaster>(this.baseUrl + 'api/UserMasters/GetUserMaster/' + id).pipe();
  }

  resetPwd(pwd: CreatePwd) {
    return this.http.post<CreatePwd>(this.baseUrl + 'api/UserMasters/ResetPassword', pwd, {
      //headers: new HttpHeaders({ "Authorization": "Bearer " + this.auth.authorisationToken })
      responseType: 'json'
    }).pipe();
  }

  forgotPassword(id) {
    return this.http.get(this.baseUrl + 'api/UserMasters/ForgotPassword/' + id, { responseType: 'text' });
  }

  changePassword(chngPwd: ChangePwd) {
    return this.http.put(this.baseUrl + 'api/UserMasters/ChangePassword', chngPwd, { responseType: 'text' }).pipe();
  }

  getAdminSetting() {
    return this.http.get<AdminSetting>(this.baseUrl + 'api/AdminSettings/GetAdminSettingLast').pipe();
  }

  postAdminSetting(settingModel: AdminSetting) {
    return this.http.post(this.baseUrl + 'api/AdminSettings', settingModel, { responseType: 'text' }).pipe();

  }

  getProductMaster() {
    return this.http.get<ProductMaster[]>(this.baseUrl + 'api/ProductMasters').pipe();
  }

  createCompany(companyModel: CompanyMaster) {
    return this.http.post<CompanyMaster>(this.baseUrl + 'api/CompanyMasters', companyModel).pipe();
  }

  getTargetEntry(month: string) {
    return this.http.get<TargetMaster[]>(this.baseUrl + 'api/TargetMasters/GetTargetByMonth/' + month).pipe();
  }

  GetTargetMatrix(month: string) {
    return this.http.get<TargetMatrix>(this.baseUrl + 'api/TargetMasters/GetTargetMatrix/' + month).pipe();
  }

  postTargetEntry(month: string, collTarget: TargetMaster[]) {
    return this.http.post(this.baseUrl + 'api/TargetMasters/PostTargetEntry/' + month, collTarget, { responseType: 'text' }).pipe();
  }

  postTargetMatrix(month: string, objTarget: TargetMatrix) {
    return this.http.post(this.baseUrl + 'api/TargetMasters/PostTargetMatrix/' + month, objTarget, { responseType: 'text' }).pipe();
  }

  GetTargetAchieveTelecaller(month: string) {
    return this.http.get<TargetMatrix>(this.baseUrl + 'api/TargetMasters/GetTargetVsAchieve/' + month).pipe();
  }
}
