using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PastPapers.Helpers
{
    public class DateHelper
    {
        public static IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }
    }
}
