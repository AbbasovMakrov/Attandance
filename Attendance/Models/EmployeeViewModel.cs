using Attendance.Data;
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
    public class EmployeeViewModel : IValidatableObject
    {
        public Job[] Jobs { get; set; }
        [BindProperty]
        public Employee EmployeeModel { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EmployeeModel.ExcelId <= 0)
                yield return new ValidationResult("Excel Id can not be zero or less");
            Guid guid = Guid.NewGuid();
           
            if(EmployeeModel.JobId != Guid.Empty)
            {
                var dbContext = validationContext.GetService(typeof(ApplicationDbContext)) as ApplicationDbContext;
                var job = dbContext.Find<Job>(EmployeeModel.JobId);
                if (job is null)
                {
                    yield return new ValidationResult("Job Not found");
                }
                if (job.MinSalary > EmployeeModel.Salary || job.MaxSalary < EmployeeModel.Salary)
                {
                    yield return new ValidationResult($"Salary must be in range ({job.MinSalary},{job.MaxSalary})");
                }
                
            }
            
        }
    }
}
