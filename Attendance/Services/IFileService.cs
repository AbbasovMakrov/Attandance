using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Attendance.Services
{
    public record FileOptions(string FileName , string Directory);
    public interface IFileService
    {

        public Task Upload(IFormFile file , FileOptions options);
    }
}
