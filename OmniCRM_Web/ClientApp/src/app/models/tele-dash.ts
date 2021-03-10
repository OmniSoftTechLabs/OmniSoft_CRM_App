export class TeleDash {
  totalLeads: number;
  notInterested: number;
  appoinmentTaken: number;
  noResponse: number;

  target: number;
  achievement: number;

  collChartData: ChartData[] = [];

  monthlyTotalLeads: number;
  monthlyNotInterested: number;
  monthlyAppoinmentTaken: number;
  monthlyNoResponse: number;

  lastMonthTotalLeads: number;
  lastMonthNotInterested: number;
  lastMonthAppoinmentTaken: number;
  lastMonthNoResponse: number;
}


export class ChartData {
  month: string;
  appoinTaken: number;
  notInterest: number;
}
