export class AdminReport {
  telecallerSummaryReport: TeleCallerStatusReport;
}

export class TeleCallerStatusReport {

  header: [] = [];
  tcRowsData: RowsData[] = [];
}

export class RowsData {
  tCName: string;
  noResponse: number;
  notInterested: number;
  appoinmentTaken: number;
  callLater: number;
  wrongNumber: number;
  none: number;
  total: number;
}
