using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CoursePlanner.Data;
using CoursePlanner.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }
        // Requires using Microsoft.AspNetCore.Mvc.Rendering;
        public SelectList Subjects { get; set; }
        [BindProperty(SupportsGet = true)]
        public string ClassSubject { get; set; }

        public async Task OnGetAsync()
        {
            IQueryable<string> subjectQuery = from m in _context.Class
                                            orderby m.Subject
                                            select m.Subject;

            var classes = from m in _context.Class
                         select m;
            if (!string.IsNullOrEmpty(SearchString))
            {
                classes = classes.Where(s => s.Description.Contains(SearchString));
                // ex: SearchString= Opera & s.Description= Operating System
            }

            if (!string.IsNullOrEmpty(ClassSubject))
            {
                classes = classes.Where(x => x.Subject == ClassSubject);
            }

            Subjects = new SelectList(await subjectQuery.Distinct().ToListAsync());
            Class = await classes.ToListAsync();
        }
        public IActionResult OnGetSearch(string term)
        {
            var classes = _context.Class.Where(c => c.Description.Contains(term)).Select(c => c.Description).ToList();
            return new JsonResult(classes);
        }
    }
}