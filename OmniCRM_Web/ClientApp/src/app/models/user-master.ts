import { RoleMaster } from "./role-master";

export class UserMaster {
  token: string = "";
  userId: any;
  firstName: string = "";
  lastName: string = "";
  email: string = "";
  status: string = "";
  roleId: number;
  //roleModels: RoleMaster[] = [];
  role: RoleMaster;
}