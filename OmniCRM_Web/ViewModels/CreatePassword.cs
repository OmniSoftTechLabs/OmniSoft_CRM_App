using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OmniCRM_Web.ViewModels
{
    public class CreatePassword
    {
        public Guid UserId { get; set; }
        public string Password { get; set; }
    }
}
