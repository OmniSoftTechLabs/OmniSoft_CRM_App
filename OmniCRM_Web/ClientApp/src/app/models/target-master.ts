import { UserMaster } from "./user-master";

export class TargetMaster {
  tagetId: number;
  telecallerId: any;
  monthYear: Date;
  targetWeek1: number;
  targetWeek2: number;
  targetWeek3: number;
  targetWeek4: number;
  targetWeek5: number;
  telecallerName: string;
}

export class TargetMatrix {
  header: string[] = [];
  rowDataTargetMaster: TargetMaster[] = [];
}

//export class RowsData {
//  tCName: string;
//  tCid: any;
//  week1: string;
//  week2: string;
//  week3: string;
//  week4: string;
//  week5: string;
//}
