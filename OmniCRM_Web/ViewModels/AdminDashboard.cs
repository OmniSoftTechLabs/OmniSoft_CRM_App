using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OmniCRM_Web.ViewModels
{
    public class AdminDashboard
    {
        public List<TeleCallerChartData> CollTeleChartData { get; set; }
        public List<ManagerChartData> CollMangerChartData { get; set; }
    }

    public class TeleCallerChartData
    {
        public string Telecaller { get; set; }
        public int NoResponse { get; set; }
        public int NotInterested { get; set; }
        public int AppoinmentTaken { get; set; }
        public int CallLater { get; set; }
        public int WrongNumber { get; set; }
        public int None { get; set; }
        public int Dropped { get; set; }
    }

    public class ManagerChartData
    {
        public string Manager { get; set; }
        public int FirstMeeting { get; set; }
        public int SecondMeeting { get; set; }
        public int Sold { get; set; }
        public int Dropped { get; set; }
        public int Hold { get; set; }
        public int NotInterested { get; set; }
        public int Pending { get; set; }
    }
}
