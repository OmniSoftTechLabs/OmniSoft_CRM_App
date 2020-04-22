import { Data } from "@angular/router";
import { RmanagerMaster } from "./rmanager-master";

export class LeadMaster {
  callId: number = 0;
  createdDate: Date;
  createdBy: string;
  firstName: string;
  lastName: string;
  mobileNumber: string;
  firmName: string;
  address: string;
  lastChangedDate: Date;
  outComeId: number = 0;
  remark: string;
  outCome: OutcomeMaster;
  outComeText: string;
  createdByName: string;
  allocatedToName: string;
  appointmentDateTime: Date;
  appointmentDetail: AppointmentDetail[] = [];
  followupHistory: FollowupHistory[] = [];
}


export class OutcomeMaster {
  outComeId: number = 0;
  outCome: string;
}

export class AppointmentDetail {
  appintmentId: number = 0;
  callId: number;
  createdDate: Date;
  createdBy: string;
  appointmentDateTime: Date = new Date();
  relationshipManagerId: string;
  appoinStatusId: number;
  remarks: string;
}

export class AppoinmentStatusMaster {
  appoinStatusID: number = 0;
  status: string;
}

export class FollowupHistory {
  followupId: number = 0;
  callId: number;
  createdDate: Date;
  createdByRmanagerId: string;
  followupType: string = "";
  appoinStatusId: number;
  remarks: string;
  appoinStatus: AppoinmentStatusMaster;
  createdByRmanager: RmanagerMaster;
}

export class CallTransactionDetail {
  callTransactionId: number;
  callId: number;
  createdDate: Date;
  createdBy: any;
  outComeId: number;
  remarks: string;
  outCome: OutcomeMaster;
  createdByNavigation: RmanagerMaster;
}
