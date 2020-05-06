using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoursePlanner.Models;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;

namespace CoursePlanner.Scheduler
{
    public class Scheduler
    {
        private Dictionary<string, List<string>> collisionDictionary;

        private List<Section> allSectionsList;
        private List<Section> availableSectionsList;
        private List<Class> allClassesList;
        private List<Class> availableClassesList;
        private ArrayList choices;

        private static Scheduler instance = null;
       
        private Scheduler(CoursePlanner.Data.CoursePlannerContext context)
        {

            InitializeLists(context);
        }

        public static Scheduler SchedulerInstance(CoursePlanner.Data.CoursePlannerContext context)
        {
            if (instance == null)
            {
                instance = new Scheduler(context);
            }
            return instance;
        }

        public void InitializeLists(CoursePlanner.Data.CoursePlannerContext context)
        {
          
            choices = new ArrayList();
            collisionDictionary = new Dictionary<string, List<string>>();
            var allSections = from m in context.Section
                              select m;
            allSectionsList = new List<Section>(allSections);
            availableSectionsList = new List<Section>(allSectionsList);

            var allClasses = from m in context.Class
                             select m;
            allClassesList = new List<Class>(allClasses);
            availableClassesList = new List<Class>(allClassesList);
        }

        public void ClassSelected(string classChoice, CoursePlanner.Data.CoursePlannerContext context)
        {
            var currentChoice = Tuple.Create(classChoice.Substring(0, classChoice.Length - 3), Int32.Parse(classChoice.Substring(classChoice.Length - 3)));

            var currentChoiceCode = classChoice;

            choices.Add(currentChoice);

            //Console.WriteLine(currentChoice);

            var chosenClassID = from m in context.Class
                                where m.Subject == currentChoice.Item1
                                where m.Code == currentChoice.Item2
                                select m.ClassId;

            var chosenClassAllSectionsID = from m in context.Section
                                           where m.ClassId == chosenClassID.ToList()[0]
                                           select m.SectionId;

            var chosenClassAllSections = from m in context.Section
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

                var classAllSectionsID = from m in context.Section
                                         where m.ClassId == remainingClass.ClassId
                                         select m.SectionId;
                var classAllSections = from m in context.Section
                                       where classAllSectionsID.ToList().Contains(m.SectionId)
                                       select m;
                List<Section> classAllSectionsList = new List<Section>(classAllSections);
              
                var groupedClassSections = classAllSectionsList.GroupBy(sect => sect.Type);
                
               

                foreach (var group in groupedClassSections)
                {
                    bool validSectionForGroup = false;
                    foreach (var sect in group)
                    {
                        
                        if (availableSectionsList.Any(t => t.SectionId == sect.SectionId))
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


        public void ResetChoices()
        {
            instance = null;
        }

    }
}
