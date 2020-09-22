using System;
using System.Collections.Generic;

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
