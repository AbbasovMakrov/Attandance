using Attendance.Extenstions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Attendance.Models
{
    public class UploadExcelViewModel : IValidatableObject
    {
        private readonly string[] _acceptedExenstions = new[]
            {
                ".xls",
                ".xlsx",
                ".csv"
            };
        [Required]
        public IFormFile File { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var fileExtenstion = File.GetExtenstion();
            if (!_acceptedExenstions.Any(e => e == fileExtenstion))
               yield return new ValidationResult($"Accepted extenstions for file are : {string.Join(',',_acceptedExenstions)}");
            //if ((DateTimeExtenstions.CurrentDaysOfMonth() - DateTime.Today.Day) > 3)
            //    yield return new ValidationResult("You can upload excels in last three days of month");

        }
    }
}
