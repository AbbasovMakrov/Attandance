using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Attendance.Extenstions
{
    public static class DateTimeExtenstions
    {
        public static TimeSpan CurrentTime(this DateTime dt)
        {
            return new TimeSpan(dt.Hour, dt.Minute, dt.Second);
        }

        public static int CurrentDaysOfMonth()
        {
            return DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month);
        }
    }
}
