using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OmniCRM_Web.ViewModels
{
    public class AdminReport
    {
        public TeleCallerStatusReport TelecallerSummaryReport { get; set; }
    }

    public class TeleCallerStatusReport
    {

        public List<string> Header { get; set; }
        public List<RowsData> TCRowsData { get; set; }

        public class RowsData
        {
            public string TCName { get; set; }
            public int NoResponse { get; set; }
            public int NotInterested { get; set; }
            public int AppoinmentTaken { get; set; }
            public int CallLater { get; set; }
            public int WrongNumber { get; set; }
            public int None { get; set; }
            public int Dropped { get; set; }
            public int Interested { get; set; }
            public int Total { get; set; }

        }
    }

    public class RelaManagerStatusReport
    {

        public List<string> Header { get; set; }
        public List<RowsDataRM> RMRowsData { get; set; }

        public class RowsDataRM
        {
            public string RMName { get; set; }
            public int FirstMeeting { get; set; }
            public int SecondMeeting { get; set; }
            public int Sold { get; set; }
            public int Dropped { get; set; }
            public int Hold { get; set; }
            public int NotInterested { get; set; }
            public int AppointTaken { get; set; }
            public int Interested { get; set; }
            public int Total { get; set; }
            public int Interested { get; set; }

        }
    }
}
