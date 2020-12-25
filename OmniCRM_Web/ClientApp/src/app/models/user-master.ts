import { RoleMaster } from "./role-master";

export class UserMaster {
  token: string = "";
  companyId: any;
  logoImage: string;
  employeeCode: string = "";
  userId: any;
  firstName: string = "";
  lastName: string = "";
  email: string = "";
  status: string = "true";
  roleId: number = 0;
  //roleModels: RoleMaster[] = [];
  role: RoleMaster;
}
