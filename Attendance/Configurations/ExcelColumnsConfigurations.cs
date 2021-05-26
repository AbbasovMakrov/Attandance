using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Attendance.Configurations
{
    public class ExcelColumnsConfigurations
    {
        public int IdIndex { get; set; }
        public int DateIndex { get; set; }
        public int SignInTimeIndex { get; set; }
        public int SignOutTimeIndex { get; set; }
        public int IsAbsentIndex { get; set; }
        public int RowStartIndex { get; set; }
    }
}
