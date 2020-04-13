using System;
using System.Collections.Generic;

namespace OmniCRM_Web.Models
{
    public partial class CallDetail
    {
        public CallDetail()
        {
            AppointmentDetail = new HashSet<AppointmentDetail>();
            CallTransactionDetail = new HashSet<CallTransactionDetail>();
            FollowupHistory = new HashSet<FollowupHistory>();
        }

        public int CallId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedBy { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string FirmName { get; set; }
        public string Address { get; set; }
        public DateTime LastChangedDate { get; set; }
        public string Remark { get; set; }

        public virtual UserMaster CreatedByNavigation { get; set; }
        public virtual ICollection<AppointmentDetail> AppointmentDetail { get; set; }
        public virtual ICollection<CallTransactionDetail> CallTransactionDetail { get; set; }
        public virtual ICollection<FollowupHistory> FollowupHistory { get; set; }
    }
}
