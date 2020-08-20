export class AdminDash {
  collAdminChartData: AdminChartData[] = [];
}

export class AdminChartData {
  telecaller: string;
  noResponse: number;
  notInterested: number;
  appoinmentTaken: number;
  callLater: number;
  wrongNumber: number;
  none: number;
}
