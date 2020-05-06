
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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoursePlanner.Pages
{

    public class IndexModel : PageModel
    {
        private readonly CoursePlanner.Data.CoursePlannerContext _context;


        private readonly ILogger<IndexModel> _logger;

        public List<Course> selectedCourses;

        Dictionary<string, List<string>> collisionDictionary = new Dictionary<string, List<string>>();

        bool listsInitialized = false;

        List<Section> allSectionsList;
        List<Section> availableSectionsList;
        List<Class> allClassesList;
        List<Class> availableClassesList;
        ArrayList choices;

        public IndexModel(ILogger<IndexModel> logger, CoursePlanner.Data.CoursePlannerContext context)
        {
            _context = context;
            _logger = logger;
        }
        public JsonResult json;

        public JArray jarray;


        public static bool firstTime = true;
        public static int tableLength;
        public static int looped = 0;
        public static List<Tuple<String, int>> choicesUntouched = new List<Tuple<String, int>>();
        public static List<List<Section>> allTables = new List<List<Section>>();

        public void OnGet()
        {

            //Dictionary<string, List<string>> collisionDictionary = new Dictionary<string, List<string>>();

            //var currentChoice = Tuple.Create("COMP", 491);
            //var currentChoiceCode = currentChoice.Item1 + currentChoice.Item2;

            //ArrayList choices = new ArrayList();
            //choices.Add(currentChoice);

            //Console.WriteLine(currentChoice);



            //var chosenClassID = from m in _context.Class
            //                    where m.Subject == currentChoice.Item1
            //                    where m.Code == currentChoice.Item2
            //                    select m.ClassId;

            //var chosenClassAllSectionsID = from m in _context.Section
            //                               where m.ClassId == chosenClassID.ToList()[0]
            //                               select m.SectionId;

            //var chosenClassAllSections = from m in _context.Section
            //                             where chosenClassAllSectionsID.ToList().Contains(m.SectionId)
            //                             select m;

            //Initially we need the allSections list, however, as we make choices we will only need to use the availableSectionsList list
            //var allSections = from m in _context.Section
            //                  select m;
            //List<Section> allSectionsList = new List<Section>(allSections);
            //List<Section> availableSectionsList = new List<Section>(allSectionsList);
            //List<Section> chosenClassAllSectionsList = new List<Section>(chosenClassAllSections);

            //Section section1 = allSectionsList[385];
            //Section section2 = allSectionsList[214];

            //var groupedSections = chosenClassAllSectionsList.GroupBy(sect => sect.Type);
            //List<Section> availableSectionsListCopy = new List<Section>(availableSectionsList);
            //Console.WriteLine("Before selection, the available section count was " + availableSectionsList.Count());
            //foreach (Section remainingSection in availableSectionsListCopy)
            //{
            //    foreach (var group in groupedSections)
            //    {
            //        bool validSectionForGroup = false;
            //        foreach (var sect in group)
            //        {
            //            Console.WriteLine("Looking for collision in " + remainingSection.SectionId);
            //            if (!Collides(sect, remainingSection))
            //            {
            //                validSectionForGroup = true;
            //                break;
            //            }
            //        }
            //        if (!validSectionForGroup)
            //        {
            //            Console.WriteLine("All the " + group.Key + " sections of the chosen class and this section collided.");
            //            Console.WriteLine("Deleting sectionID " + remainingSection.SectionId + " because of collision");
            //            availableSectionsList.Remove(remainingSection);
            //            break;
            //        }
            //    }
            //}
            //Console.WriteLine("After one iterative selection of " + currentChoice.Item1 + " " + currentChoice.Item2 + " " + ", the available section count is " + availableSectionsList.Count());

            //Console.WriteLine(Collides(section1, section2));

            //Initially we need the allClasses list, however, as we make choices we will only need to use the availableClassesList list
            //var allClasses = from m in _context.Class
            //                 select m;
            //List<Class> allClassesList = new List<Class>(allClasses);
            //List<Class> availableClassesList = new List<Class>(allClassesList);
            //List<Class> availableClassesListCopy = new List<Class>(availableClassesList);


            //Console.WriteLine("Before selection, the available class count was " + availableClassesList.Count());

            //foreach (Class remainingClass in availableClassesListCopy)
            //{

            //    var classAllSectionsID = from m in _context.Section
            //                             where m.ClassId == remainingClass.ClassId
            //                             select m.SectionId;
            //    var classAllSections = from m in _context.Section
            //                           where classAllSectionsID.ToList().Contains(m.SectionId)
            //                           select m;
            //    List<Section> classAllSectionsList = new List<Section>(classAllSections);

            //    var groupedClassSections = classAllSectionsList.GroupBy(sect => sect.Type);

            //    foreach (var group in groupedClassSections)
            //    {
            //        bool validSectionForGroup = false;
            //        foreach (var sect in group)
            //        {
            //            if (availableSectionsList.Contains(sect))
            //            {
            //                validSectionForGroup = true;
            //                break;
            //            }
            //        }
            //        if (!validSectionForGroup)
            //        {
            //            Console.WriteLine("All the " + group.Key + " sections of " + remainingClass.Subject + " " + remainingClass.Code + " was removed as it collided with previous choices.");
            //            Console.WriteLine("Deleting classID " + remainingClass.ClassId + " with Code: " + remainingClass.Subject + " " + remainingClass.Code + " from the available classes list...");
            //            var remainingClassCode = remainingClass.Subject + remainingClass.Code;
            //            if (collisionDictionary.ContainsKey(remainingClassCode))
            //            {
            //                collisionDictionary[remainingClassCode].Add(currentChoiceCode);
            //            }
            //            else
            //            {
            //                collisionDictionary.Add(remainingClassCode, new List<string>(new string[] { currentChoiceCode }));
            //            }
            //            availableClassesList.Remove(remainingClass);
            //            break;
            //        }
            //    }
            //}

            //Console.WriteLine("After one iterative selection, the available class count is " + availableClassesList.Count());
            //Console.WriteLine("Printing remaining classes...");
            //foreach (Class avail in availableClassesList)
            //{
            //    Console.WriteLine(avail.Subject + " " + avail.Code);
            //}

            //foreach (KeyValuePair<string, List<string>> kvp in collisionDictionary)
            //{
            //    Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            //    foreach (string asd in kvp.Value)
            //    {
            //        Console.WriteLine(asd);
            //    }
            //}

        }

        public bool Collides(Section section1, Section section2)
        {
            if (section2.Times == "" || section2.Times == "0:0-0:0" || section2.Times == "Fri ")  //These are bad data
            {
                return false;
            }
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
                Console.WriteLine("There was a collision.");
                return true;
            }
            return false;
        }

        public void ClassSelected(string classChoice)
        {
            var currentChoice = Tuple.Create(classChoice.Substring(0, classChoice.Length-3), Int32.Parse(classChoice.Substring(classChoice.Length-3)));

            var currentChoiceCode = classChoice;

            if (!listsInitialized)
            {
                ArrayList choices = new ArrayList();
                var allSections = from m in _context.Section
                                  select m;
                List<Section> allSectionsList = new List<Section>(allSections);
                List<Section> availableSectionsList = new List<Section>(allSectionsList);

                var allClasses = from m in _context.Class
                                 select m;
                List<Class> allClassesList = new List<Class>(allClasses);
                List<Class> availableClassesList = new List<Class>(allClassesList);

                listsInitialized = true;
            }

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

            List<Section> chosenClassAllSectionsList = new List<Section>(chosenClassAllSections);


            var groupedSections = chosenClassAllSectionsList.GroupBy(sect => sect.Type);
            List<Section> availableSectionsListCopy = new List<Section>(availableSectionsList);
            Console.WriteLine("Before selection, the available section count was " + availableSectionsList.Count());
            foreach (Section remainingSection in availableSectionsListCopy)
            {
                foreach (var group in groupedSections)
                {
                    bool validSectionForGroup = false;
                    foreach (var sect in group)
                    {
                        Console.WriteLine("Looking for collision in " + remainingSection.SectionId);
                        if (!Collides(sect, remainingSection))
                        {
                            validSectionForGroup = true;
                            break;
                        }
                    }
                    if (!validSectionForGroup)
                    {
                        Console.WriteLine("All the " + group.Key + " sections of the chosen class and this section collided.");
                        Console.WriteLine("Deleting sectionID " + remainingSection.SectionId + " because of collision");
                        availableSectionsList.Remove(remainingSection);
                        break;
                    }
                }
            }
            Console.WriteLine("After one iterative selection of " + currentChoice.Item1 + " " + currentChoice.Item2 + " " + ", the available section count is " + availableSectionsList.Count());

            //Console.WriteLine(Collides(section1, section2));

            //Initially we need the allClasses list, however, as we make choices we will only need to use the availableClassesList list
           
            List<Class> availableClassesListCopy = new List<Class>(availableClassesList);


            Console.WriteLine("Before selection, the available class count was " + availableClassesList.Count());

            foreach (Class remainingClass in availableClassesListCopy)
            {

                var classAllSectionsID = from m in _context.Section
                                         where m.ClassId == remainingClass.ClassId
                                         select m.SectionId;
                var classAllSections = from m in _context.Section
                                       where classAllSectionsID.ToList().Contains(m.SectionId)
                                       select m;
                List<Section> classAllSectionsList = new List<Section>(classAllSections);

                var groupedClassSections = classAllSectionsList.GroupBy(sect => sect.Type);

                foreach (var group in groupedClassSections)
                {
                    bool validSectionForGroup = false;
                    foreach (var sect in group)
                    {
                        if (availableSectionsList.Contains(sect))
                        {
                            validSectionForGroup = true;
                            break;
                        }
                    }
                    if (!validSectionForGroup)
                    {
                        Console.WriteLine("All the " + group.Key + " sections of " + remainingClass.Subject + " " + remainingClass.Code + " was removed as it collided with previous choices.");
                        Console.WriteLine("Deleting classID " + remainingClass.ClassId + " with Code: " + remainingClass.Subject + " " + remainingClass.Code + " from the available classes list...");
                        var remainingClassCode = remainingClass.Subject + remainingClass.Code;
                        if (collisionDictionary.ContainsKey(remainingClassCode))
                        {
                            collisionDictionary[remainingClassCode].Add(currentChoiceCode);
                        }
                        else
                        {
                            collisionDictionary.Add(remainingClassCode, new List<string>(new string[] { currentChoiceCode }));
                        }
                        availableClassesList.Remove(remainingClass);
                        break;
                    }
                }
            }

            Console.WriteLine("After one iterative selection, the available class count is " + availableClassesList.Count());
            Console.WriteLine("Printing remaining classes...");
            foreach (Class avail in availableClassesList)
            {
                Console.WriteLine(avail.Subject + " " + avail.Code);
            }


            foreach (KeyValuePair<string, List<string>> kvp in collisionDictionary)
            {
                Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                foreach (string asd in kvp.Value)
                {
                    Console.WriteLine(asd);
                }
            }
        }

        public void resetChoices()
        {
            listsInitialized = false;
        }

        public void OnPostMakeTimetables()
        {
            List<Tuple<String, int>> choices = new List<Tuple<String, int>>();
            choices.Add(Tuple.Create("ECON", 100));
            choices.Add(Tuple.Create("INDR", 100));
            choices.Add(Tuple.Create("ASIU", 102));

            List<Tuple<String, int>> choicesForRecursion = new List<Tuple<String, int>>(choices);
            List<Section> chosenClassAllSectionsList = RemoveFaultySections(choicesForRecursion[0]);
            choicesForRecursion.Remove(choicesForRecursion[0]);
            foreach (Section section in chosenClassAllSectionsList)
            {
                List<Section> currentTable = new List<Section>();
                currentTable.Add(section);
                MakeTimetables(choicesForRecursion, currentTable, 0, choicesForRecursion.Count() - 1);
            }
        }

        public void MakeTimetables(List<Tuple<String, int>> choices, List<Section> currentTable, int depth, int choicesLastIndex)
        {
            List<Section> chosenClassAllSectionsList = RemoveFaultySections(choices[depth]);
            List<Section> currentTableForLoop = new List<Section>(currentTable);
            foreach (Section section in chosenClassAllSectionsList)
            {
                bool intersects = false;
                foreach (Section tableSection in currentTableForLoop)
                {
                    if (Collides(section, tableSection))
                    {
                        intersects = true;
                        break;
                    }
                }
                if (!intersects)
                {
                    List<Section> currentTableToPassDown = new List<Section>(currentTable);
                    currentTableToPassDown.Add(section);
                    if (depth < choicesLastIndex)
                    {
                        MakeTimetables(choices, currentTableToPassDown, depth + 1, choicesLastIndex);
                    }
                    else
                    {
                        Console.WriteLine("---------------");
                        foreach (Section selected in currentTableToPassDown)
                        {
                            Console.WriteLine(selected.ClassId + ": " + selected.Times);
                        }
                    }
                }
            }
        }

        public List<Section> RemoveFaultySections(Tuple<String, int> choice)
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
            return chosenClassAllSectionsList;
        }

        public void OnPostSelectedCourses([FromBody]List<Course> sc)
        {
            Console.WriteLine("\n\n\n\nWriting sc:");
            selectedCourses = sc;
            if (selectedCourses.Count > 0)
                Console.WriteLine(selectedCourses[0].Name + selectedCourses[0].Code);
        }
    }

    public class Course
    {
        public string Name { get; set; }
        public int Code { get; set; }
        public string Color { get; set; }
    }
}




