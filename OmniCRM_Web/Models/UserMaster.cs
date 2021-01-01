using System;
using System.Collections.Generic;

namespace OmniCRM_Web.Models
{
    public partial class UserMaster
    {
        public UserMaster()
        {
            AdminSetting = new HashSet<AdminSetting>();
            AppointmentDetail = new HashSet<AppointmentDetail>();
            CallDetail = new HashSet<CallDetail>();
            CallTransactionDetail = new HashSet<CallTransactionDetail>();
            FollowupHistory = new HashSet<FollowupHistory>();
        }

        public Guid UserId { get; set; }
        public string EmployeeCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public bool Status { get; set; }
        public string PasswordSalt { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LinkExpiryDate { get; set; }
        public Guid CompanyId { get; set; }

        public virtual CompanyMaster Company { get; set; }
        public virtual RoleMaster Role { get; set; }
        public virtual ICollection<AdminSetting> AdminSetting { get; set; }
        public virtual ICollection<AppointmentDetail> AppointmentDetail { get; set; }
        public virtual ICollection<CallDetail> CallDetail { get; set; }
        public virtual ICollection<CallTransactionDetail> CallTransactionDetail { get; set; }
        public virtual ICollection<FollowupHistory> FollowupHistory { get; set; }
    }
}
