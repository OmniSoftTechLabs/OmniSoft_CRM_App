export class AdminReport {
  telecallerSummaryReport: TeleCallerStatusReport;
}

export class TeleCallerStatusReport {

  header: [] = [];
  tCRowsData: RowsData[] = [];
}

export class RowsData {
  tCName: string;
  noResponse: number;
  notInterested: number;
  appoinmentTaken: number;
  callLater: number;
  wrongNumber: number;
  none: number;
}
