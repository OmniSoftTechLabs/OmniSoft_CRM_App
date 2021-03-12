using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OmniCRM_Web.ViewModels
{
    public class TargetMasterViewModel
    {
        public int TagetId { get; set; }
        public Guid TelecallerId { get; set; }
        public DateTime MonthYear { get; set; }
        public int WeekNumber { get; set; }
        public int Target { get; set; }
        public string TelecallerName { get; set; }

    }

    public class TargetMatrix
    {
        public List<string> Header { get; set; }
        public List<RowsData> RowData { get; set; }

        public class RowsData
        {
            public string TCName { get; set; }
            public int Week1 { get; set; }
            public int Week2 { get; set; }
            public int Week3 { get; set; }
            public int Week4 { get; set; }
            public int Week5 { get; set; }
        }
    }
}
