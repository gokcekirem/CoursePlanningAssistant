using CoursePlanner.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


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

            //Initially we need the allSections list, however, as we make choices we will only need to use the availableSectionsList list
            var allSections = from m in _context.Section
                              select m;
            List<Section> allSectionsList = new List<Section>(allSections);
            List<Section> availableSectionsList = allSectionsList;
            List<Section> chosenClassAllSectionsList = new List<Section>(chosenClassAllSections);

            Section section1 = allSectionsList[385];
            Section section2 = allSectionsList[214];

            var groupedSections = chosenClassAllSectionsList.GroupBy(sect => sect.Type);

            foreach (Section remainingSection in availableSectionsList)
            {
                foreach (var group in groupedSections)
                {
                    bool validSectionForGroup = false;
                    foreach (var sect in group)
                    {
                        if (!Collides(sect, remainingSection))
                        {
                            validSectionForGroup = true;
                            break;
                        }
                    }     
                    if (!validSectionForGroup)
                    {
                        availableSectionsList.Remove(remainingSection);
                    }
                }
            }

            Console.WriteLine(Collides(section1, section2));

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
    }
}
