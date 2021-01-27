export class AdminReport {
  telecallerSummaryReport: TeleCallerStatusReport;
  releManagerSummaryReport: RelaManagerStatusReport;
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

export class RelaManagerStatusReport {

  header: [] = [];
  rmRowsData: RowsDataRM[] = [];
}

export class RowsDataRM {
  rMName: string;
  firstMeeting: number;
  secondMeeting: number;
  sold: number;
  dropped: number;
  hold: number;
  notInterested: number;
  appointTaken: number;
  total: number;
}
