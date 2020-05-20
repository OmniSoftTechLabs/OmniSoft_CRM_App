export class ManagerDash {
  totalAllocatedLeads: number;
  soldLeads: number;
  holdLeads: number;
  droppedLeads: number;
  collChartData: ChartDataMnager[] = [];

  monthlySold: number;
  monthlyHold: number;
  monthlyDropped: number;
}

export class ChartDataMnager {
  month: string;
  sold: number;
  dropped: number;
}
