using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OmniCRM_Web.ViewModels
{
    public class TeleCallerDashboard
    {
        public int TotalLeads { get; set; }
        public int NotInterested { get; set; }
        public int AppoinmentTaken { get; set; }
        public int NoResponse { get; set; }

        public int Target { get; set; }
        public int Achievement { get; set; }

        public List<ChartData> CollChartData { get; set; }

        public int MonthlyTotalLeads { get; set; }
        public int MonthlyNotInterested { get; set; }
        public int MonthlyAppoinmentTaken { get; set; }
        public int MonthlyNoResponse { get; set; }

        public int LastMonthTotalLeads { get; set; }
        public int LastMonthNotInterested { get; set; }
        public int LastMonthAppoinmentTaken { get; set; }
        public int LastMonthNoResponse { get; set; }
    }

    public class ChartData
    {
        public string Month { get; set; }
        public int AppoinTaken { get; set; }
        public int NotInterest { get; set; }
    }

}
