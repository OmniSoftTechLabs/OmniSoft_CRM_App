import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { OutcomeMaster, LeadMaster, AppoinmentStatusMaster, CallTransactionDetail, FollowupHistory, StateMaster, CityMaster, AppointmentDetail } from '../models/lead-master';
import { RmanagerMaster } from '../models/rmanager-master';
import { FilterOptions } from '../models/filter-options';
import { TeleDash } from '../models/tele-dash';
import { ManagerDash } from '../models/manager-dash';
import { AdminDash } from '../models/admin-dash';

@Injectable({
  providedIn: 'root'
})
export class LeadRepositoryService {

  http: HttpClient;
  baseUrl: string;

  constructor(_http: HttpClient, @Inject('BASE_URL') _baseUrl: string) {
    this.http = _http;
    this.baseUrl = _baseUrl;
  }

  loadOutcomeList() {
    return this.http.get<OutcomeMaster[]>(this.baseUrl + 'api/CallOutcomeMasters').pipe();
  }

  loadAppoinmentStatusList() {
    return this.http.get<AppoinmentStatusMaster[]>(this.baseUrl + 'api/AppoinmentStatusMasters').pipe();
  }

  createLead(leadModel: LeadMaster) {
    return this.http.post(this.baseUrl + 'api/CallDetails', leadModel, { responseType: 'text' }).pipe();
  }

  editlead(leadModel: LeadMaster) {
    return this.http.put(this.baseUrl + 'api/CallDetails/' + leadModel.callId, leadModel, { responseType: 'text' }).pipe();
  }

  createFollowup(leadModel: LeadMaster) {
    return this.http.put(this.baseUrl + 'api/CallDetails/PutFollowupDetail/' + leadModel.callId, leadModel, { responseType: 'text' }).pipe();
  }

  dismissLeads(collLeads: LeadMaster[]) {
    return this.http.put(this.baseUrl + 'api/CallDetails/DismissLeads/', collLeads, { responseType: 'text' }).pipe();
  }

  deleteLead(collLeads: LeadMaster[]) {
    return this.http.put(this.baseUrl + 'api/CallDetails/DeleteCallDetail/', collLeads, { responseType: 'text' }).pipe();
  }

  remindMelater(collLeads: LeadMaster[], strDate: string) {
    return this.http.put(this.baseUrl + 'api/CallDetails/RemindMeLater/' + strDate, collLeads, { responseType: 'text' }).pipe();
  }

  getLeadById(id) {
    return this.http.get<LeadMaster>(this.baseUrl + 'api/CallDetails/GetLeadById/' + id).pipe();
  }

  loadRManagerList() {
    return this.http.get<RmanagerMaster[]>(this.baseUrl + 'api/CallDetails/GetRelationshipManagerList').pipe();

  }

  loadTeleCallerList() {
    return this.http.get<RmanagerMaster[]>(this.baseUrl + 'api/CallDetails/GetTeleCallerList').pipe();
  }

  loadStateMaster(term: string) {
    return this.http.get<StateMaster[]>(this.baseUrl + 'api/StateMasters/GetStateMasterByName/' + term).pipe();
  }

  loadCityMaster(id: number, term: string) {
    return this.http.get<CityMaster[]>(this.baseUrl + 'api/StateMasters/GetCityMaster/' + id + '/' + term).pipe();
  }

  getSelectedState(id: number) {
    return this.http.get<StateMaster>(this.baseUrl + 'api/StateMasters/' + id).pipe();
  }

  getSelectedCity(id: number) {
    return this.http.get<CityMaster>(this.baseUrl + 'api/StateMasters/GetCityMaster/' + id).pipe();
  }

  async loadLeadListByCreatedBy(id, filterOption: FilterOptions) {
    let response = await this.http.post<LeadMaster[]>(this.baseUrl + 'api/CallDetails/GetCallDetailByCreatedBy/' + id, filterOption).toPromise();
    return response;
  }

  async loadLeadListByRM(id, filterOption: FilterOptions) {
    let response = await this.http.post<LeadMaster[]>(this.baseUrl + 'api/CallDetails/GetCallDetailByRM/' + id, filterOption).toPromise();
    return response;
  }

  async loadCallTransById(id) {
    let response = await this.http.get<CallTransactionDetail[]>(this.baseUrl + 'api/CallDetails/GetCallTransDetail/' + id).toPromise();
    return response;
  }

  async loadFollowupHistoryById(id) {
    let response = await this.http.get<FollowupHistory[]>(this.baseUrl + 'api/CallDetails/GetFollowupHistory/' + id).toPromise();
    return response;
  }

  uploadExcelData(id, formData: FormData) {
    //let headers = new HttpHeaders();

    //headers.append('Content-Type', 'multipart/form-data');
    //headers.append('Accept', 'application/json');

    //const httpOptions = { headers: headers };
    return this.http.post(this.baseUrl + 'api/CallDetails/UploadExcelData/' + id, formData, { responseType: 'text' }).pipe();
  }

  async loadTeleDash(id) {
    let response = await this.http.get<TeleDash>(this.baseUrl + 'api/CallDetails/GetTeleCallerDashboard/' + id).toPromise();
    return response;
  }

  async loadManagerDash(id) {
    let response = await this.http.get<ManagerDash>(this.baseUrl + 'api/CallDetails/GetRManagerDashboard/' + id).toPromise();
    return response;
  }

  async loadAdminDash(filterOption: FilterOptions) {
    let response = await this.http.post<AdminDash>(this.baseUrl + 'api/CallDetails/GetAdminDashboard/', filterOption).toPromise();
    return response;
  }

  reallocatedToRM(appointmentDetail: AppointmentDetail) {
    return this.http.post(this.baseUrl + 'api/CallDetails/PostReAllocateRM/', appointmentDetail, { responseType: 'text' }).pipe();
  }
}
