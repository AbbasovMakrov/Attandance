using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Attendance.Extenstions
{
    public static class StringExtensions
    {
        public static string Random(int length = 16)
        {
            var random = new Random();
            var chars = "ASDFGHJKLPOIUYTREWQZXCVBNM1234567890asdfghjklpoiuytrewqzxcvbnm".ToCharArray();
            var str = string.Empty;
            Enumerable.Range(0, length).ToList().ForEach(_ =>
            {
                str += chars[random.Next(0, chars.Length - 1)];
            });
            return str;
        }
    }
}
