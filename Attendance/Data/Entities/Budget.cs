using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Attendance.Data.Entities.Helpers;

namespace Attendance.Data.Entities
{
    public class Budget : Entity , ITimestampableEntity
    {
        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public decimal Amount { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
