import { UserMaster } from "./user-master";

export class CompanyMaster {
  companyId: any;
  companyName: string;
  companyLogo: any[];
  logoBase64: string;
  userSubscription: number;
  address: string;
  phoneNo: string;
  userMaster: UserMaster[] = [];
}
