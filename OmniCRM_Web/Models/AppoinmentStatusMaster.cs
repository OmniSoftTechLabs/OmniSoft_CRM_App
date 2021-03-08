using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace OmniCRM_Web.Models
{
    public partial class AppoinmentStatusMaster
    {
        public AppoinmentStatusMaster()
        {
            AppointmentDetail = new HashSet<AppointmentDetail>();
            FollowupHistory = new HashSet<FollowupHistory>();
        }

        public int AppoinStatusId { get; set; }
        public string Status { get; set; }

        public virtual ICollection<AppointmentDetail> AppointmentDetail { get; set; }
        public virtual ICollection<FollowupHistory> FollowupHistory { get; set; }
    }
}
