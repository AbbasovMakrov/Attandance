using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Attendance.Helpers
{
    public struct AttendanceExcel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan SignInTime { get; set; }
        public TimeSpan SignOutTime { get; set; }
        public bool IsAbsent { get; set; }
    }
}
