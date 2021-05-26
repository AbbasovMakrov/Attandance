using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Attendance.Services
{
    public class FileService : IFileService
    {
        public async Task Upload(IFormFile file, FileOptions options)
        {
            if (!Directory.Exists(options.Directory))
                Directory.CreateDirectory(options.Directory);
            using(var fs = new FileStream(Path.Combine(options.Directory , options.FileName), FileMode.Create))
            {
               await file.CopyToAsync(fs);
            }
        }
    }
}
