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
        public int Target { get; set; }
        public string TelecallerName { get; set; }
    }
}
