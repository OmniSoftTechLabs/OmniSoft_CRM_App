export class AdminDash {
  collTeleChartData: TeleCallerChartData[] = [];
  collMangerChartData: ManagerChartData[] = [];
  collTargetChartData: TargetChartData[] = [];
}

export class TeleCallerChartData {
  telecaller: string;
  noResponse: number;
  notInterested: number;
  appoinmentTaken: number;
  callLater: number;
  wrongNumber: number;
  none: number;
  dropped: number;
  interested: number;
}

export class ManagerChartData {
  manager: string;
  firstMeeting: number;
  secondMeeting: number;
  sold: number;
  dropped: number;
  hold: number;
  notInterested: number;
  pending: number;
  interested: number;
}

export class TargetChartData {
  week: string;
  achievement: number;
  target: number;
}
