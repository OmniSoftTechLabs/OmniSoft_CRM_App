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
            public int Total { get; set; }

        }
    }
}
