using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoursePlanner.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;


namespace CoursePlanner.Pages
{

    public class IndexModel : PageModel
    {
        private readonly CoursePlanner.Data.CoursePlannerContext _context;
        

        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger, CoursePlanner.Data.CoursePlannerContext context)
        {
            _context = context;
            _logger = logger;
        }

        public void OnGet()
        {
            var currentChoice = Tuple.Create("COMP", 131);
            ArrayList choices = new ArrayList();
            choices.Add(currentChoice);
            Console.WriteLine(currentChoice);

            var chosenClassID = from m in _context.Class
                              where m.Subject == currentChoice.Item1
                              where m.Code == currentChoice.Item2
                              select m.ClassId;
            
            var allSectionsID = from m in _context.Section
                              where m.ClassId == chosenClassID.ToList()[0]
                              select m.SectionId;

            var chosenClassSectionTimes = from m in _context.Section
                               where allSectionsID.ToList().Contains(m.SectionId)
                               select m.Times;

            for (int i=0; i<chosenClassSectionTimes.ToList().Count(); i++)
            {
                Console.WriteLine(chosenClassSectionTimes.Count());
                Console.WriteLine(chosenClassSectionTimes.ToList()[i]);
            }


            List<Tuple<Section, String, DateTime, DateTime>> times;
            var allRemainingSectionsID = from m in _context.Section
                                       select m.SectionId;

            var start1 = DateTime.Parse("10:00");
            var end1 = DateTime.Parse("11:30");

            var start2 = DateTime.Parse("10:0");
            var end2 = DateTime.Parse("11:45");

            var section1 = Tuple.Create(101, "Sat Thu", start1, end1);
            var section2 = Tuple.Create(101, "Mon Wed", start2, end2);

            Console.WriteLine(Collides(section1, section2));
            //var allAvailableSections = from m in _context.Section
            //                           where m.ClassId == 
        }

        public bool Collides(Tuple<int, String, DateTime, DateTime> section1, Tuple<int, String, DateTime, DateTime> section2)
        {
            var intersection = section1.Item2.Split(" ").Intersect(section2.Item2.Split(" "));
            if (intersection.Count() == 0)
            {
                return false;
            }
            if ((section1.Item3 < section2.Item4) && (section1.Item4 > section2.Item3))
            {
                return true;
            }
            return false;
        }

        //public async void OnPostScheduleAsync()
        //{
        //    var allSections = from m in _context.Section
        //                      where m.StatusId == 1
        //                      select m;
        //    var chosenSections = from m in _context.Section
        //                         where m.StatusId == 0
        //                         select m;

        //    List<Section> noConflict = new List<Section>(allSections);
        //    TimeSpan elapsed = DateTime.Parse(endTime).Subtract(DateTime.Parse(startTime));
        //    //t1.start_time < t2.end_time AND t1.end_time > t2.start_time

        //    Console.WriteLine($"Time elapsed: {elapsed}");
        //    foreach (Section section in chosenSections)
        //    {
        //        DateTime start1 = DateTime.Parse(section.Times.Substring(0, 5));
        //        DateTime end1 = DateTime.Parse(section.Times.Substring(6, 5));
        //        Console.WriteLine("SectionId: " + section.SectionId + " Section Times: " + start1 + " " + end1);
        //        foreach (Section section2 in allSections)
        //        {
        //            DateTime start2 = DateTime.Parse(section2.Times.Substring(0, 5));
        //            DateTime end2 = DateTime.Parse(section2.Times.Substring(6, 5));
        //            Console.WriteLine("SectionId: " + section2.SectionId + " section times: " + start2 + " " + end2);
        //            if ((start1 < end2) && (end1 > start2))
        //            {
        //                noConflict.Remove(section2);
        //            }
        //        }
        //    }
        //    Console.WriteLine("Printing all non-conflicting section selections...");
        //    foreach (Section section in noConflict)
        //    {
        //        Console.WriteLine("SectionId: " + section.SectionId + " Section Times: " + section.Times);
        //    }

        //    var classes = from m in _context.Class
        //                  select m;
        //    Class = await classes.ToListAsync();
        //}
    }
}
