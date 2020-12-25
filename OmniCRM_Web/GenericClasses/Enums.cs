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

        public enum AppoinmentStatus
        {
            FirstMeeting = 1,
            SecondMeeting = 2,
            Sold = 3,
            Dropped = 4,
            Hold = 5,
            NotInterested = 6,
            Pending = 7,
            Dismissed = 8,
        }

        public enum CallOutcome
        {
            NoResponse = 1,
            NotInterested = 2,
            AppoinmentTaken = 3,
            CallLater = 4,
            WrongNumber = 5,
            None = 6,
            Dropped = 7,
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
