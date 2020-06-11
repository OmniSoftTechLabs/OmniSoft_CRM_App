using System;
using System.Collections.Generic;

namespace OmniCRM_Web.Models
{
    public partial class CityMaster
    {
        public CityMaster()
        {
            CallDetail = new HashSet<CallDetail>();
        }

        public int CityId { get; set; }
        public int StateId { get; set; }
        public string CityName { get; set; }

        public virtual StateMaster State { get; set; }
        public virtual ICollection<CallDetail> CallDetail { get; set; }
    }
}
