using System;
using System.Collections.Generic;

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
