using System;
using System.Collections.Generic;

namespace OmniCRM_Web.Models
{
    public partial class CompanyMaster
    {
        public CompanyMaster()
        {
            CallDetail = new HashSet<CallDetail>();
            UserMaster = new HashSet<UserMaster>();
        }

        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; }
        public byte[] CompanyLogo { get; set; }
        public string LogoBase64 { get; set; }
        public int? UserSubscription { get; set; }
        public string Address { get; set; }
        public string PhoneNo { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual ICollection<CallDetail> CallDetail { get; set; }
        public virtual ICollection<UserMaster> UserMaster { get; set; }
    }
}
