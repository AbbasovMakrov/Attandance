using Attendance.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Attendance.Models
{
    public class JobViewModel
    {
        public Discount[] Discounts { get; set; }
        [BindProperty]
        public Job JobModel { get; set; } = new ();
        [BindProperty]

        public Guid[] DiscountsIds { get; set; }
    }


    
}
