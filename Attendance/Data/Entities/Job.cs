using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Attendance.Data.Entities.Helpers;

namespace Attendance.Data.Entities
{
    public class Job : Entity , ITimestampableEntity
    {
        [Required]
        public string Name { get; set; }
        [Required,Display(Name = "Min Salary")]
        public int MinSalary { get; set; }
        [Required,Display(Name = "Max Salary")]
        public int MaxSalary { get; set; }
        [DataType(DataType.Time)]
        [Required]
        public TimeSpan StartTime { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Discount> Discounts { get; set; } = new HashSet<Discount>();
    }
}
