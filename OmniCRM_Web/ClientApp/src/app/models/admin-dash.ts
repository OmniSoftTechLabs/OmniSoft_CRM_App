export class AdminDash {
  collTeleChartData: TeleCallerChartData[] = [];
}

export class TeleCallerChartData {
  telecaller: string;
  noResponse: number;
  notInterested: number;
  appoinmentTaken: number;
  callLater: number;
  wrongNumber: number;
  none: number;
}
