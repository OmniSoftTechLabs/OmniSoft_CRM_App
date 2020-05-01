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

    }
}
