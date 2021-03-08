using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace OmniCRM_Web.Models
{
    public partial class StateMaster
    {
        public StateMaster()
        {
            CallDetail = new HashSet<CallDetail>();
            CityMaster = new HashSet<CityMaster>();
        }

        public int StateId { get; set; }
        public string StateName { get; set; }

        public virtual ICollection<CallDetail> CallDetail { get; set; }
        public virtual ICollection<CityMaster> CityMaster { get; set; }
    }
}
