export class TeleDash {
  totalLeads: number;
  notInterested: number;
  appoinmentTaken: number;
  noResponse: number;
  collChartData: ChartData[] = [];
}


export class ChartData {
  month: string;
  appoinTaken: number;
  notInterest: number;
}
