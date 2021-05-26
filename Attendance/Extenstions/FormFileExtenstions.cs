using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Attendance.Extenstions
{
    public static class FormFileExtenstions
    {
        public static string GetExtenstion(this IFormFile file)
        {
            return Path.GetExtension(file.FileName.ToLower());
        }
    }
}
