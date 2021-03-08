using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace OmniCRM_Web.Models
{
    public partial class CallOutcomeMaster
    {
        public CallOutcomeMaster()
        {
            CallDetail = new HashSet<CallDetail>();
            CallTransactionDetail = new HashSet<CallTransactionDetail>();
        }

        public int OutComeId { get; set; }
        public string OutCome { get; set; }

        public virtual ICollection<CallDetail> CallDetail { get; set; }
        public virtual ICollection<CallTransactionDetail> CallTransactionDetail { get; set; }
    }
}
