using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace OmniCRM_Web.Models
{
    public partial class CallDetailArchive
    {
        public int CallId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedBy { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string FirmName { get; set; }
        public string EmailId { get; set; }
        public int? ProductId { get; set; }
        public string Address { get; set; }
        public int? StateId { get; set; }
        public int? CityId { get; set; }
        public DateTime LastChangedDate { get; set; }
        public int OutComeId { get; set; }
        public DateTime? NextCallDate { get; set; }
        public string Remark { get; set; }
        public bool? IsDeleted { get; set; }
        public Guid CompanyId { get; set; }
    }
}
