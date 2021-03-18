using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace OmniCRM_Web.GenericClasses
{
    struct Range
    {
        public DateTime Start { get; private set; }
        public DateTime End { get { return Start.AddDays(6); } }

        public Range(DateTime start)
        {
            Start = start;
        }
    }

    public static class GetWeekRange
    {
        //public static IEnumerable<Range> GetRange(int year, int month)
        //{
        //    DateTime start = new DateTime(year, month, 1).AddDays(-6);
        //    DateTime end = new DateTime(year, month, 1).AddMonths(1).AddDays(-1);
        //    for (DateTime date = start; date <= end; date = date.AddDays(1))
        //    {
        //        if (date.DayOfWeek == DayOfWeek.Sunday)
        //        {
        //            yield return new Range(date);
        //        }
        //    }
        //}


        public static IEnumerable<object> GetListofWeeks(int year, int month)
        {
            var calendar = CultureInfo.CurrentCulture.Calendar;
            var firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            var weekPeriods = Enumerable.Range(1, calendar.GetDaysInMonth(year, month))
                      .Select(d =>
                      {
                          var date = new DateTime(year, month, d);
                          var weekNumInYear = calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, firstDayOfWeek);
                          return new { date, weekNumInYear };
                      })
                      .GroupBy(x => x.weekNumInYear)
                      .Select(x => new { DateFrom = x.First().date, To = x.Last().date })
                      .ToList();

            return weekPeriods;
        }
    }
}
