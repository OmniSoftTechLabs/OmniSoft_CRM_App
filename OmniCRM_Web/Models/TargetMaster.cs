﻿using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace OmniCRM_Web.Models
{
    public partial class TargetMaster
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

        public virtual UserMaster Telecaller { get; set; }
    }
}
