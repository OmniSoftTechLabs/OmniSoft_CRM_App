using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace OmniCRM_Web.Models
{
    public partial class CallTransactionDetail
    {
        public int CallTransactionId { get; set; }
        public int CallId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedBy { get; set; }
        public int OutComeId { get; set; }
        public string Remarks { get; set; }

        public virtual CallDetail Call { get; set; }
        public virtual UserMaster CreatedByNavigation { get; set; }
        public virtual CallOutcomeMaster OutCome { get; set; }
    }
}
