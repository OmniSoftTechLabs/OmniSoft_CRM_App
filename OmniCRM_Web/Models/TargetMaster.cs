using System;
using System.Collections.Generic;

namespace OmniCRM_Web.Models
{
    public partial class TargetMaster
    {
        public int TagetId { get; set; }
        public Guid TelecallerId { get; set; }
        public DateTime MonthYear { get; set; }
        public int Target { get; set; }

        public virtual UserMaster Telecaller { get; set; }
    }
}
