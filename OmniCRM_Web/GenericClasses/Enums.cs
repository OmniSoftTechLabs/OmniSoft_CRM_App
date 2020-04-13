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
            RelationshipManager = 3,
            SuperUser = 101,
        }
    }

    public class StringConstant
    {
        public const string SuperUser = "Super User";
        public const string Admin = "Admin";
        public const string TeleCaller = "Tele Caller";
        public const string RelationshipManager = "Relationship Manager";
    }
}
