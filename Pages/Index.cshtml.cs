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

            //Initially we need the allSections list, however, as we make choices we will only need to use the noConflict list
            var allSections = from m in _context.Section
                              select m;
            List<Section> allSectionsList = new List<Section>(allSections);

            Console.WriteLine(allSectionsList[0].Times);
            Console.WriteLine(allSectionsList[2].Times);

            Section section1 = allSectionsList[0];
            Section section2 = allSectionsList[2];

            //for (int i = 0; i < allSectionsList.ToList().Count(); i++)
            //{
            //    var days = new String(allSectionsList[i].Times.Where(Char.IsLetter).ToArray());
            //    var startTime = DateTime.Parse("10:00");

            //}            

            //var start1 = DateTime.Parse("10:00");
            //var end1 = DateTime.Parse("11:30");

            //var start2 = DateTime.Parse("10:0");
            //var end2 = DateTime.Parse("11:45");

            //var section1 = Tuple.Create(101, "Sat Thu", start1, end1);
            //var section2 = Tuple.Create(101, "Mon Wed", start2, end2);

            Console.WriteLine(Collides(section1, section2));
            //var allAvailableSections = from m in _context.Section
            //                           where m.ClassId == 

        }

        public bool Collides(Section section1, Section section2)
        {
            var days1 = new String(section1.Times.Where(Char.IsLetter).ToArray());
            var days2 = new String(section2.Times.Where(Char.IsLetter).ToArray());

            var hours1 = section1.Times.Split(" ");
            //var hours1 = section1.Times.ToList().Except(days1.ToList()).ToString().Split("-");
            var hours2 = section2.Times.ToList().Except(days2.ToList()).ToString().Split("-");

            Console.WriteLine(hours1);

            var start1 = DateTime.Parse(hours1[0]);
            var end1 = DateTime.Parse(hours1[1]);
            var start2 = DateTime.Parse(hours2[0]);
            var end2 = DateTime.Parse(hours2[1]);

            var intersection = days1.Intersect(days2);
            if (intersection.Count() == 0)
            {
                return false;
            }
            if ((start1 < end2) && (end1 > start2))
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
