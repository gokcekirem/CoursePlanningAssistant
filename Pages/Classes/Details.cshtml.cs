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
    public class DetailsModel : PageModel
    {
        private readonly CoursePlanner.Data.CoursePlannerContext _context;

        public DetailsModel(CoursePlanner.Data.CoursePlannerContext context)
        {
            _context = context;
        }

        public Class Class { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Class = await _context.Class.FirstOrDefaultAsync(m => m.ClassId == id);

            if (Class == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
