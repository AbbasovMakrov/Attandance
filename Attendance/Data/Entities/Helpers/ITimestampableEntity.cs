using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Attendance.Data.Entities.Helpers
{
    interface ITimestampableEntity
    {
        
        [Timestamp,Display(Name = "Created At")]
        public DateTime? CreatedAt { get; set; }

        [Timestamp,Display(Name = "Modified At")]
        public DateTime? UpdatedAt { get; set; } 

    }
}
