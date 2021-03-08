using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace OmniCRM_Web.Models
{
    public partial class FollowupHistory
    {
        public int FollowupId { get; set; }
        public int CallId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedByRmanagerId { get; set; }
        public string FollowupType { get; set; }
        public DateTime? AppoinDate { get; set; }
        public int AppoinStatusId { get; set; }
        public string Remarks { get; set; }

        public virtual AppoinmentStatusMaster AppoinStatus { get; set; }
        public virtual CallDetail Call { get; set; }
        public virtual UserMaster CreatedByRmanager { get; set; }
    }
}
