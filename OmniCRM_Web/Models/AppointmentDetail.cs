using System;
using System.Collections.Generic;

namespace OmniCRM_Web.Models
{
    public partial class AppointmentDetail
    {
        public int AppintmentId { get; set; }
        public int CallId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public Guid RelationshipManagerId { get; set; }
        public int AppoinStatusId { get; set; }
        public string Remarks { get; set; }

        public virtual AppoinmentStatusMaster AppoinStatus { get; set; }
        public virtual CallDetail Call { get; set; }
        public virtual UserMaster CreatedByNavigation { get; set; }
    }
}
