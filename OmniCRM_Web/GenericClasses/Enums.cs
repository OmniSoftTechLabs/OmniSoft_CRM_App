using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OmniCRM_Web.GenericClasses
{
    public class Enums
    {
        public enum LogType
        {
            ErrorLog,
            ActivityLog,
            HistoryLog,
        }

        public enum Roles
        {
            Admin = 1,
            TeleCaller = 2,
            RelationshipManager = 3
        }
    }
}
