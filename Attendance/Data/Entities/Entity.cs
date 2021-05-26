using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Attendance.Data.Entities
{
    public abstract class Entity
    {
        public Guid Id { get; set; }
    }
}
