using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CoursePlanner.Data;
using CoursePlanner.Models;

namespace CoursePlanner
{
    public class IndexModel : PageModel
    {
        private readonly CoursePlanner.Data.CoursePlannerContext _context;

        public IndexModel(CoursePlanner.Data.CoursePlannerContext context)
        {
            _context = context;
        }

        public IList<Class> Class { get;set; }

        public async Task OnGetAsync()
        {
            Class = await _context.Class.ToListAsync();
        }
    }
}
