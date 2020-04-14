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
}


export class OutcomeMaster {
  outComeId: number = 0;
  outCome: string;
}
