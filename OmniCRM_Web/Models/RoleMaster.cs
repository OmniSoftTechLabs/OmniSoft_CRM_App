using System;
using System.Collections.Generic;

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
