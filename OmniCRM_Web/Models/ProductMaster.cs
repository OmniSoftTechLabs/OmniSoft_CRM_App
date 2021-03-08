using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace OmniCRM_Web.Models
{
    public partial class ProductMaster
    {
        public ProductMaster()
        {
            CallDetail = new HashSet<CallDetail>();
        }

        public int ProductId { get; set; }
        public string ProductName { get; set; }

        public virtual ICollection<CallDetail> CallDetail { get; set; }
    }
}
