import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { OutcomeMaster, LeadMaster, AppoinmentStatusMaster } from '../models/lead-master';
import { RmanagerMaster } from '../models/rmanager-master';
import { FilterOptions } from '../models/filter-options';

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

  getLeadById(id) {
    return this.http.get<LeadMaster>(this.baseUrl + 'api/CallDetails/' + id).pipe();
  }

  loadRManagerList() {
    return this.http.get<RmanagerMaster[]>(this.baseUrl + 'api/CallDetails/GetRelationshipManagerList').pipe();

  }

  loadLeadListByCreatedBy(id, filterOption: FilterOptions) {
    return this.http.post<LeadMaster[]>(this.baseUrl + 'api/CallDetails/GetCallDetailByCreatedBy/' + id, filterOption).pipe();
  }

  loadLeadListByRM(id) {
    return this.http.get<LeadMaster[]>(this.baseUrl + 'api/CallDetails/GetCallDetailByRM/' + id).pipe();
  }
}
