export class FilterOptions {
  status: number[] = [];
  createdBy: string;
  allocatedTo: string;
  dateFilterBy: number;
  fromDate: Date;
  todate: Date;
  toSkip: number = 0;
  toTake: number = 30;
}
