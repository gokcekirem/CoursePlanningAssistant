
using CoursePlanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace CoursePlanner.Pages
{

    public class IndexModel : PageModel
    {
        private readonly CoursePlanner.Data.CoursePlannerContext _context;


        private readonly ILogger<IndexModel> _logger;

        public List<Course> selectedCourses;
        List<List<Section>> sectionsDividedToGroups = new List<List<Section>>();

        public CoursePlanner.Scheduler.Scheduler scheduler;
        

        public IndexModel(ILogger<IndexModel> logger, CoursePlanner.Data.CoursePlannerContext context)
        {
            _context = context;
            _logger = logger;
            scheduler = Scheduler.Scheduler.SchedulerInstance(context); ;
        }


        public void OnGet()
        {

        }

        public void OnPostRefresh()
        {
            scheduler.ResetChoices();
        }
      
        public void OnPostMakeTimetables()
        {
            if (scheduler.getChoices().Any())
            {
                List<Tuple<string, int>> choices = new List<Tuple<string, int>>();
                choices = scheduler.getChoices();
                foreach (Tuple<string, int> choice in choices)
                {
                    RemoveFaultySections(choice);
                }
                List<List<Section>> resultingTables = new List<List<Section>>();

                    foreach (Section section in sectionsDividedToGroups[0])
                    {
                            List<Section> currentTable = new List<Section>();
                            currentTable.Add(section);
                            MakeTimetables(currentTable, 1, resultingTables);
                    }
                List<string> times = new List<string>() {"8.30-9.45", "10.00-11.15", "11.30-12.45", "13.00-14.15", "14.30-15.45", "16.00-17.15", "17.30-18.45"};
                ViewData["timetables"] = resultingTables;
                ViewData["times"] = times;
                ViewData["flag"] = "flag";
            }
           
        }

        public void MakeTimetables(List<Section> currentTable, int depth, List<List<Section>> resultingTables)
        {
            List<Section> currentTableForLoop = new List<Section>(currentTable);
                
                foreach (Section section in sectionsDividedToGroups[depth])
                {
                        bool intersects = false;
                        foreach (Section tableSection in currentTableForLoop)
                        {
                            if (scheduler.Collides(section, tableSection))
                            {
                                intersects = true;
                                break;
                            }
                        }
                        if (!intersects)
                        {
                            List<Section> currentTableToPassDown = new List<Section>(currentTable);
                            currentTableToPassDown.Add(section);
                            if (depth < sectionsDividedToGroups.Count() - 1)
                            {
                                MakeTimetables(currentTableToPassDown, depth + 1, resultingTables);
                            }
                            else
                            {
                                resultingTables.Add(currentTableToPassDown);
                            }
                        }
                }
        }

        public void RemoveFaultySections(Tuple<string, int> choice)
        {
            var chosenClassID = from m in _context.Class
                                where m.Subject == choice.Item1
                                where m.Code == choice.Item2
                                select m.ClassId;

            var chosenClassAllSectionsID = from m in _context.Section
                                           where m.ClassId == chosenClassID.ToList()[0]
                                           select m.SectionId;

            var chosenClassAllSections = from m in _context.Section
                                         where chosenClassAllSectionsID.ToList().Contains(m.SectionId)
                                         select m;
            List<Section> chosenClassAllSectionsList = new List<Section>(chosenClassAllSections);
            foreach (Section section in chosenClassAllSections)
            {
                if (section.Times == "" || section.Times == "0:0-0:0" || section.Times == "Fri ")  //These are bad data
                {
                    chosenClassAllSectionsList.Remove(section);
                    Console.WriteLine("Removed: " + section);
                }
            }
            var groupedSections = chosenClassAllSectionsList.GroupBy(section => section.Type);
            foreach (var group in groupedSections)
            {
                List<Section> sectionByGroups = new List<Section>();
                foreach (Section section in chosenClassAllSectionsList)
                {
                    if(group.Key == section.Type)
                    {
                        sectionByGroups.Add(section);
                    }
                }
                sectionsDividedToGroups.Add(sectionByGroups);
            }
            Console.WriteLine(sectionsDividedToGroups);
        }

        public string getClassName(int classID)
        {
            var chosenClass = from m in _context.Class
                              where classID == m.ClassId
                              select m;
            Class myClass = chosenClass.First();
            return myClass.Subject + " " + myClass.Code;
        }

        public void OnPostSelectedCourses([FromBody]List<Course> sc)
        {
            Console.WriteLine("\n\n\n\nWriting sc:");
            selectedCourses = sc;
            
            int l = selectedCourses.Count();
            if (selectedCourses.Count() > 0)
            {
                Console.WriteLine(selectedCourses[l - 1].Name + selectedCourses[l - 1].Code);
                scheduler.ClassSelected(selectedCourses[l - 1].Name + selectedCourses[l - 1].Code, _context);
            }
        }

        public JsonResult OnGetDict()
        {
            return new JsonResult(scheduler.getCollisionDictionary());
        }
    }

    public class Course
    {
        public string Name { get; set; }
        public int Code { get; set; }
        public string Color { get; set; }
    }
}




