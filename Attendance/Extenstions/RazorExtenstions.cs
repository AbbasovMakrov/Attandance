using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Attendance.Extenstions
{
    public static class RazorExtenstions
    {
        public static IEnumerable<SelectListItem> ToSelectList<TModel>(this IEnumerable<TModel> items ,  Func<TModel , SelectListItem> conversation)
        {
            return items.Select(conversation);
        }
    }
}
