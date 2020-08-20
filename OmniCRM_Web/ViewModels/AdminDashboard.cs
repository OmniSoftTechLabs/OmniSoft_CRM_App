using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OmniCRM_Web.ViewModels
{
    public class AdminDashboard
    {
        public List<AdminChartData> CollAdminChartData { get; set; }
    }

    public class AdminChartData
    {
        public string Telecaller { get; set; }
        public int NoResponse { get; set; }
        public int NotInterested { get; set; }
        public int AppoinmentTaken { get; set; }
        public int CallLater { get; set; }
        public int WrongNumber { get; set; }
        public int None { get; set; }
    }
}
