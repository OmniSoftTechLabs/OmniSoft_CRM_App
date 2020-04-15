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
  objOutCome: OutcomeMaster;

  appointmentDetail: AppointmentDetail[] = [];
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
