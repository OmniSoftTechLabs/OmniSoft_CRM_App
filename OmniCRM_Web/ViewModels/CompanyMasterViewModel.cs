using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OmniCRM_Web.ViewModels
{
    public class CompanyMasterViewModel
    {
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; }
        public byte[] CompanyLogo { get; set; }
        public string EmailId { get; set; }
        public int? UserSubscription { get; set; }
    }
}
