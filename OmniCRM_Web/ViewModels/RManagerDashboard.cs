using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OmniCRM_Web.ViewModels
{
    public class RManagerDashboard
    {
        public int TotalAllocatedLeads { get; set; }
        public int SoldLeads { get; set; }
        public int HoldLeads { get; set; }
        public int DroppedLeads { get; set; }
        public List<ChartDataMnager> CollChartData { get; set; }

        public int MonthlySold { get; set; }
        public int MonthlyHold { get; set; }
        public int MonthlyDropped { get; set; }

        public List<EventCalendar> CollCalendarEvents { get; set; }
    }

    public class ChartDataMnager
    {
        public int MonthNumber { get; set; }
        public string Month { get; set; }
        public int Sold { get; set; }
        public int Dropped { get; set; }
    }

    public class EventCalendar
    {
        public DateTime AppointmentTime { get; set; }
        public string ClientName { get; set; }
        public string AppointStatus { get; set; }
        public int AppointStatusId { get; set; }
    }
}
