﻿using System;
using System.Collections.Generic;

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
    }
}