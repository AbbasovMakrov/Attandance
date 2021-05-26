using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Attendance.Data.Entities.Helpers;

namespace Attendance.Data.Entities
{
    public class Discount : Entity, ITimestampableEntity
    {
        public string Type { get; set; }
        public int Amount { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ICollection<Job> Jobs { get; set; } = new HashSet<Job>();

        public Discount()
        {
            UpdatedAt = CreatedAt ?? DateTime.Now;

            CreatedAt = DateTime.Now;
        }
    }
}
