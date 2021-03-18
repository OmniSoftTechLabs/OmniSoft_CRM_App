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
        public int TargetWeek1 { get; set; }
        public int TargetWeek2 { get; set; }
        public int TargetWeek3 { get; set; }
        public int TargetWeek4 { get; set; }
        public int TargetWeek5 { get; set; }
        public int TargetWeek6 { get; set; }

        public string TelecallerName { get; set; }
    }

    public class TargetMatrix
    {
        public List<string> Header { get; set; }
        public List<TargetMasterViewModel> RowDataTargetMaster { get; set; }
    }

    //public class RowsData
    //{
    //    public int TagetId { get; set; }
    //    public string TCName { get; set; }
    //    public Guid TCid { get; set; }
    //    public int Week1 { get; set; } = 0;
    //    public int Week2 { get; set; } = 0;
    //    public int Week3 { get; set; } = 0;
    //    public int Week4 { get; set; } = 0;
    //    public int Week5 { get; set; } = 0;
    //}
}
