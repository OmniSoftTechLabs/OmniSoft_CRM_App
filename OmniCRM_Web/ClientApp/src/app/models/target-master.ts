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
  targetWeek6: number;

  telecallerName: string;

  achieveWeek1: number;
  achieveWeek2: number;
  achieveWeek3: number;
  achieveWeek4: number;
  achieveWeek5: number;
  achieveWeek6: number;

}

export class TargetMatrix {
  header: string[] = [];
  rowDataTargetMaster: TargetMaster[] = [];
}

