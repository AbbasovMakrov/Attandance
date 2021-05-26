using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Attendance.Data.Entities.Helpers;

namespace Attendance.Data.Entities
{
    public class Employee : Entity , ITimestampableEntity 
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public int Salary { get; set; }
        [Required,Display(Name = "Excel Id")]
        public int ExcelId { get; set; }
        [Display(AutoGenerateField = false)]
        public Guid? BackendId { get; set; }
        [Required,Display(Name = "Job")]
        public Guid JobId { get; set; }
        public Job Job { get; set; }
        [Required,Display(Name= "Work Days")]
        public int WorkDaysCount { get; set; }

        public ICollection<MoneyTransaction> MoneyTransactions { get; set; } = new HashSet<MoneyTransaction>();
        public Budget Budget { get; set; }
        [ScaffoldColumn(false)]
        public DateTime? CreatedAt { get; set; }
        [Display(Name = "Updated At")]
        public DateTime? UpdatedAt { get; set; }
    }
}
