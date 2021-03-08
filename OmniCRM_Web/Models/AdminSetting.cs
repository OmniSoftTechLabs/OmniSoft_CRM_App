using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace OmniCRM_Web.Models
{
    public partial class AdminSetting
    {
        public int SettingId { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? AppoinTimeInterval { get; set; }
        public DateTime? DailyEmailTime { get; set; }
        public int? OverDueDaysRm { get; set; }

        public virtual UserMaster CreatedByNavigation { get; set; }
    }
}
