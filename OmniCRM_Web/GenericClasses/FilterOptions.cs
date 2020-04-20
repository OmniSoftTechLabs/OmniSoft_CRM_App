using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OmniCRM_Web.GenericClasses
{
    public class FilterOptions
    {
        public int Status { get; set; }
        public string CreatedBy { get; set; }
        public string AllocatedTo { get; set; }
        public int DateFilterBy { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime Todate { get; set; }
    }
}
