using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OmniCRM_Web.ViewModels
{
    public class CallDetailViewModel
    {
        public int CallId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string FirmName { get; set; }
        public string Address { get; set; }
        public DateTime LastChangedDate { get; set; }
        public int OutComeId { get; set; }
        public string Remark { get; set; }
        public string CreatedByName { get; set; }
        public string OutComeText { get; set; }
        public string AllocatedToName { get; set; }
        public DateTime? AppointmentDateTime { get; set; }
        public int? AppoinStatusId { get; set; }
        public Guid CreatedById { get; set; }
        public Guid AllocatedToId { get; set; }
    }
}
