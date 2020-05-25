export class ManagerDash {
  totalAllocatedLeads: number;
  soldLeads: number;
  holdLeads: number;
  droppedLeads: number;
  collChartData: ChartDataMnager[] = [];

  monthlySold: number;
  monthlyHold: number;
  monthlyDropped: number;

  collCalendarEvents: EventCalendar[] = [];
}

export class ChartDataMnager {
  month: string;
  sold: number;
  dropped: number;
}

export class EventCalendar {
  appointmentTime: Date;
  clientName: string;
  appointStatus: string;
  appointStatusId: number;
}
