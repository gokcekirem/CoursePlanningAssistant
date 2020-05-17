
using CoursePlanner.Models;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using CoursePlanner.Scheduler;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
                List<Tuple<String, int>> choices = new List<Tuple<String, int>>();
                choices = scheduler.getChoices();
                //List<Tuple<String, int>> choicesForRecursion = new List<Tuple<String, int>>(choices);
                foreach (Tuple<string, int> choice in choices)
                {
                    RemoveFaultySections(choice);
                }
                //List<Section> chosenClassAllSectionsList = RemoveFaultySections(choicesForRecursion[0]);
                //var groupedSections = chosenClassAllSectionsList.GroupBy(sect => sect.Type);
                //choicesForRecursion.Remove(choicesForRecursion[0]);
                List<List<Section>> resultingTables = new List<List<Section>>();

                    foreach (Section section in sectionsDividedToGroups[0])
                    {
                            List<Section> currentTable = new List<Section>();
                            currentTable.Add(section);
                            //if (choicesForRecursion.Count() > 0)
                            //{
                            MakeTimetables(currentTable, 1, resultingTables);
                            //}
                            //else
                            //{
                            //    resultingTables.Add(currentTable);
                            //}
                        
                    }
                
                ViewData["timetables"] = resultingTables;
                ViewData["flag"] = "flag";
            }
           
        }

        public void MakeTimetables(List<Section> currentTable, int depth, List<List<Section>> resultingTables)
        {
            //List<Section> chosenClassAllSectionsList = RemoveFaultySections(choices[depth]);
            //var groupedSections = chosenClassAllSectionsList.GroupBy(sect => sect.Type);
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
                                //Console.WriteLine("---------------");
                                //foreach (Section selected in currentTableToPassDown)
                                //{
                                //    Console.WriteLine(selected.ClassId + ": " + selected.Times);
                                //}
                            }
                        }
                    
                    
                }
            

        }

        public void RemoveFaultySections(Tuple<String, int> choice)
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




