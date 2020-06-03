
using CoursePlanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using Highsoft.Web.Mvc.Charts;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace CoursePlanner.Pages
{

    public class IndexModel : PageModel
    {
        private readonly CoursePlanner.Data.CoursePlannerContext _context;


        private readonly ILogger<IndexModel> _logger;

        public JsonResult json;

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

        public JsonResult OnGetBubble()
        {
            // 0: start with the lecture sections
            var lecSections = from s in _context.Section
                              where s.Type == "LEC"
                              where s.Capacity > 0
                              select s.ClassId;

            // 1st part 

            var subjects = from c in _context.Class
                           where lecSections.ToList().Contains(c.ClassId)
                           select c.Subject;

            List<string> sortedClassList = new List<string>(subjects);
            sortedClassList.Sort();
            HashSet<string> unique_subjects = new HashSet<string>(sortedClassList);
            /*foreach (string s in unique_subjects)
            {
                Console.WriteLine(s);
            }
            Console.WriteLine(unique_subjects.Count());*/

            // 2nd part

            var classes = from c in _context.Class
                          where lecSections.ToList().Contains(c.ClassId)
                          select c; // subject, code, capacity, prerequisite

            List<Class> classesList = new List<Class>(classes);
            HashSet<Class> unique_classes = new HashSet<Class>(classes);
            Console.WriteLine(unique_classes.Count());

            // 3rd part : the combination
            List<Bubble> bubble_data = new List<Bubble>();

            // 3.1 add the subjects to bubble data
            foreach (string s in unique_subjects)
            {
                Bubble bubble = new Bubble();
                bubble.name = s;

                BubbleClass subjectBubble = new BubbleClass();
                subjectBubble.name = s;
                subjectBubble.value = 100;
                subjectBubble.prereq = null;
                bubble.data = new List<BubbleClass>();

                bubble.data.Add(subjectBubble);

                bubble_data.Add(bubble);
            }

            // 3.2 add the bubble classes to designated subjects
            foreach (Class c in unique_classes)
            {
                BubbleClass bubbleClass = new BubbleClass();
                bubbleClass.name = c.Code.ToString();
                bubbleClass.value = _context.Section.Where(s => s.ClassId.Equals(c.ClassId)).FirstOrDefault<Section>().Capacity;
                bubbleClass.prereq = c.Prerequisite;

                string designated_subject = c.Subject;

                foreach(Bubble b in bubble_data)
                {
                    if (designated_subject.Equals(b.name))
                    {
                        b.data.Add(bubbleClass);
                    }
                }
            }

            // 4th part: out

            /* JSON WRITER WORKS AS WELL
             * 
             * JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;

            using (StreamWriter sw = new StreamWriter(@"Pages/bubble_json.json"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, bubble_data);
            }*/

            return new JsonResult(bubble_data);


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
                    RemoveFaultySectionsAndGroup(choice);
                }
                List<List<Section>> resultingTables = new List<List<Section>>();

                    foreach (Section section in sectionsDividedToGroups[0])
                    {
                            List<Section> currentTable = new List<Section>();
                            currentTable.Add(section);
                            if (sectionsDividedToGroups.Count() > 1)
                            {
                                MakeTimetables(currentTable, 1, resultingTables);
                            }
                            else
                            {
                                resultingTables.Add(currentTable);
                            }
                    }
                TimetableFormatting(resultingTables);
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

        public void RemoveFaultySectionsAndGroup(Tuple<string, int> choice)
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

        public void TimetableFormatting(List<List<Section>> resultingTable)
        {
            List<string> times = new List<string>() { "8:30-9:45", "10:0-11:15", "11:30-12:45", "13:0-14:15", "14:30-15:45", "16:0-17:15", "17:30-18:45" };
            List<List<List<string>>> formattedTimetable = new List<List<List<string>>>();
            foreach (List<Section> table in resultingTable)
            {
                List<List<string>> formattedTable = new List<List<string>>();
                foreach (string time in times)
                {
                    List<string> timetableRow = new List<string> { time, " ", " ", " ", " ", " " };
                    foreach (Section section in table)
                    {
                        if (section.Times.Contains(time.Substring(0, 5)) || section.Times.Contains(time.Substring(time.Length - 5, 5)))
                        {
                            if (section.Times.Contains("Mon"))
                            {
                                timetableRow[1] = getClassName(section.ClassId) + "-" + section.Type + "-" + section.SectionId;
                            }
                            if (section.Times.Contains("Tue"))
                            {
                                timetableRow[2] = getClassName(section.ClassId) + "-" + section.Type + "-" + section.SectionId;
                            }
                            if (section.Times.Contains("Wed"))
                            {
                                timetableRow[3] = getClassName(section.ClassId) + "-" + section.Type + "-" + section.SectionId;
                            }
                            if (section.Times.Contains("Thu"))
                            {
                                timetableRow[4] = getClassName(section.ClassId) + "-" + section.Type + "-" + section.SectionId;
                            }
                            if (section.Times.Contains("Fri"))
                            {
                                timetableRow[5] = getClassName(section.ClassId) + "-" + section.Type + "-" + section.SectionId;
                            }
                        }
                    }
                    formattedTable.Add(timetableRow);
                }
                formattedTimetable.Add(formattedTable);
            }
            ViewData["tables"] = formattedTimetable;
        }

        public string getClassName(int classID)
        {
            var chosenClass = from m in _context.Class
                              where classID == m.ClassId
                              select m;
            Class myClass = chosenClass.First();
            return myClass.Subject + " " + myClass.Code;
        }

        public void OnPostSelectedCourse([FromBody]string sc)
        {
            Console.WriteLine("\n\n\n\nWriting sc:");
            
            Console.WriteLine("STUFF: "+sc);

            if (sc != null)
            {
                Console.WriteLine(sc);
                scheduler.ClassSelected(sc, _context);
            }
        }

        public JsonResult OnGetDict()
        {
            return new JsonResult(scheduler.getCollisionDictionary());
        }
    }

    public class Bubble
    {
        public string name { get; set; }
        public List<BubbleClass> data { get; set; }
    }

    public class BubbleClass
    {
        public string name { get; set; }
        public int value { get; set; }
        public string prereq { get; set; }
    }
}




