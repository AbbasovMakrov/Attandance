using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Attendance.Data.Entities
{
    public class ExcelFile : Entity
    {
        public string FilePath { get; set; }
        [Display(Name = "Is Parsed?")]
        public bool IsParsed { get; set; } = false;
        public DateTime UploadedAt { get; set; }
    }
}
