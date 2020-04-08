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
            var currentChoice = Tuple.Create("COMP", 317);
            ArrayList choices = new ArrayList();
            choices.Add(currentChoice);
            //Console.WriteLine(currentChoice);

            var chosenClassID = from m in _context.Class
                              where m.Subject == currentChoice.Item1
                              where m.Code == currentChoice.Item2
                              select m.ClassId;
            
            var chosenClassAllSectionsID = from m in _context.Section
                              where m.ClassId == chosenClassID.ToList()[0]
                              select m.SectionId;

            var chosenClassAllSections = from m in _context.Section
                               where chosenClassAllSectionsID.ToList().Contains(m.SectionId)
                               select m;

            //foreach (Section sect in chosenClassAllSections)
            //{
            //    Console.WriteLine(sect.SectionId);
            //    Console.WriteLine(sect.Type);
            //}

            //for (int i=0; i<chosenClassSectionTimes.ToList().Count(); i++)
            //{
            //    Console.WriteLine(chosenClassSectionTimes.Count());
            //    Console.WriteLine(chosenClassSectionTimes.ToList()[i]);
            //}

            //Initially we need the allSections list, however, as we make choices we will only need to use the noConflict list
            var allSections = from m in _context.Section
                              select m;
            List<Section> allSectionsList = new List<Section>(allSections);
            List<Section> chosenClassAllSectionsList = new List<Section>(chosenClassAllSections);

            Section section1 = allSectionsList[385];
            Section section2 = allSectionsList[214];

            var groupedSections = chosenClassAllSectionsList.GroupBy(sect => sect.Type);

            //foreach (var group in groupedSections)
            //{
            //    Console.WriteLine("Sections from " + group.Key + ":");
            //    foreach (var sect in group)
            //    {
            //        Console.WriteLine("* " + sect.SectionId);
            //    }
            //}



            foreach (Section remainingSection in allSectionsList)
            {
                foreach (var group in groupedSections)
                {
                    bool oneNoCollision = false;
                    foreach (var sect in group)
                    {
                        if (!Collides(sect, remainingSection))
                        {
                            oneNoCollision = true;
                        }
                    }
                }
            }

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

            //Console.WriteLine(Collides(section1, section2));
            Console.WriteLine(Collides(section1, section2));
            //var allAvailableSections = from m in _context.Section
            //                           where m.ClassId == 

        }

        public bool Collides(Section section1, Section section2)
        {
            var split1 = section1.Times.Split(" ").ToList();

            var hours1 = split1.Last();

            split1.Remove(hours1);

            var days1 = split1;

            var split2 = section2.Times.Split(" ").ToList();

            var hours2 = split2.Last();

            split2.Remove(hours2);

            var days2 = split2;

            var splithours1 = hours1.Split("-").ToList();
            var start1 = DateTime.Parse(splithours1[0]);
            var end1 = DateTime.Parse(splithours1[1]);

            var splithours2 = hours2.Split("-").ToList();
            var start2 = DateTime.Parse(splithours2[0]);
            var end2 = DateTime.Parse(splithours2[1]);

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
