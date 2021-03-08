using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace OmniCRM_Web.Models
{
    public partial class RoleMaster
    {
        public RoleMaster()
        {
            UserMaster = new HashSet<UserMaster>();
        }

        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual ICollection<UserMaster> UserMaster { get; set; }
    }
}
