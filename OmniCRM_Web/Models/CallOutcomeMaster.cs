using System;
using System.Collections.Generic;

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
