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

        public static bool firstTime = true;
        public static int tableLength;
        public static int looped = 0;
        public static List<Tuple<String, int>> choicesUntouched = new List<Tuple<String, int>>();
        public static List<List<Section>> allTables = new List<List<Section>>();

        public void OnGet()
        {
     
            var currentChoice = Tuple.Create("COMP", 491);
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
            List<Section> availableSectionsList = new List<Section>(allSectionsList);
            List<Section> chosenClassAllSectionsList = new List<Section>(chosenClassAllSections);

            //Section section1 = allSectionsList[385];
            //Section section2 = allSectionsList[214];

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
            var allClasses = from m in _context.Class
                             select m;
            List<Class> allClassesList = new List<Class>(allClasses);
            List<Class> availableClassesList = new List<Class>(allClassesList);
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
                        availableClassesList.Remove(remainingClass);
                        break;
                    }
                }
            }

            Console.WriteLine("After one iterative selection, the available class count is " + availableClassesList.Count());
            

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


        public void OnPostMakeTimetables()
        {
            //Console.WriteLine("This button works: ");

            //List<List<Section>> allTables = new List<List<Section>>();
            //List<List<Section>> solution = new List<List<Section>>();
            //List<Section> aTable = new List<Section>();
            //List<Tuple<String, int>> choices = new List<Tuple<String, int>>();
            //choices.Add(Tuple.Create("COMP", 491));
            //choices.Add(Tuple.Create("PHIL", 338));
            //choicesUntouched = choices.ToList();
            //MakeTimetablesHelper(choices, allTables, aTable);
            //foreach (List<Section> table in allTables)
            //{
            //    foreach(Section sect in table)
            //    {
            //        Console.WriteLine("Section's class: " + sect.Class + "\nSection Times:" + sect.Times);
            //    }
            //}


            //List<Tuple<String, int>> choices = new List<Tuple<String, int>>();
            //choices.Add(Tuple.Create("COMP", 491));
            //choices.Add(Tuple.Create("PHIL", 338));
            //List<Section> allSections = new List<Section>();
            //List<Section> allTables = new List<Section>();
            //foreach (Tuple<String, int> choice in choices)
            //{
            //    AddSectionFor(allSections, choice);
            //}
            //foreach (Section section in allSections)
            //{
            //    List<Section> aTable = new List<Section>();
            //    aTable.Add(section);
            //    foreach(Section otherSection in allSections)
            //    {
            //        if(section.ClassId == otherSection.ClassId)
            //        {
            //            continue;
            //        }
            //        else
            //        {
            //            if(!Collides(section, otherSection))
            //            {
            //                aTable.Add(otherSection);
            //            }
            //        }
            //    }
            //}



            List<Tuple<String, int>> choices = new List<Tuple<String, int>>();
            choices.Add(Tuple.Create("COMP", 491));
            choices.Add(Tuple.Create("PHIL", 338));

            var chosenClassID = from m in _context.Class
                                where m.Subject == choices[0].Item1
                                where m.Code == choices[0].Item2
                                select m.ClassId;

            var chosenClassAllSectionsID = from m in _context.Section
                                           where m.ClassId == chosenClassID.ToList()[0]
                                           select m.SectionId;

            var chosenClassAllSections = from m in _context.Section
                                         where chosenClassAllSectionsID.ToList().Contains(m.SectionId)
                                         select m;

            List<Section> aTable = new List<Section>();

            foreach (Section section in chosenClassAllSections)
            {
                aTable.Add(section);
                Helper(choices, aTable);
            }

            Console.WriteLine(allTables.Count);

        }

        //public void AddSectionFor(List<Section> sections, Tuple<String, int> choice)
        //{
        //    var chosenClassID = from m in _context.Class
        //                        where m.Subject == choice.Item1
        //                        where m.Code == choice.Item2
        //                        select m.ClassId;

        //    var chosenClassAllSectionsID = from m in _context.Section
        //                                   where m.ClassId == chosenClassID.ToList()[0]
        //                                   select m.SectionId;

        //    var chosenClassAllSections = from m in _context.Section
        //                                 where chosenClassAllSectionsID.ToList().Contains(m.SectionId)
        //                                 select m;
        //    foreach(Section section in chosenClassAllSections)
        //    {
        //        sections.Add(section);
        //    }
        //}


        public void Helper(List<Tuple<String, int>> choices, List<Section> aTable)
        {
            //if (firstTime)
            //{
            //    firstTime = false;
            //    tableLength = choices.Count();
            //}

            List<Section> aTableCopy = new List<Section>(aTable);

            var chosenClassID = from m in _context.Class
                                where m.Subject == choices[0].Item1
                                where m.Code == choices[0].Item2
                                select m.ClassId;

            var chosenClassAllSectionsID = from m in _context.Section
                                           where m.ClassId == chosenClassID.ToList()[0]
                                           select m.SectionId;

            var chosenClassAllSections = from m in _context.Section
                                         where chosenClassAllSectionsID.ToList().Contains(m.SectionId)
                                         select m;

            
            foreach (Section section in chosenClassAllSections)
            {

                    foreach(Section otherSection in aTableCopy)
                    {
                        if(!Collides(section, otherSection))
                        {
                            aTable.Add(section);
                            choices.Remove(choices[0]);
                            if (!choices.Any())
                            {
                                Helper(choices, aTable);
                            }
                            
                        }
                    }
                }
            if (aTable.Count() == tableLength)
            {
                allTables.Add(aTable);
            }


        }



    }




        //public void MakeTimetablesHelper(List<Tuple<String, int>> choices, List<List<Section>> allTables, List<Section> aTable)
       // {
            




            //List<Section> aTableCopy = new List<Section>(aTable);
            //looped++;
            //Console.WriteLine("visitations:" + looped);
            //if (firstTime)
            //{
            //    firstTime = false;
            //    tableLength = choices.Count();
            //    Console.WriteLine("a");
            //}




            //var chosenClassID = from m in _context.Class
            //                    where m.Subject == choices[0].Item1
            //                    where m.Code == choices[0].Item2
            //                    select m.ClassId;

            //var chosenClassAllSectionsID = from m in _context.Section
            //                               where m.ClassId == chosenClassID.ToList()[0]
            //                               select m.SectionId;

            //var chosenClassAllSections = from m in _context.Section
            //                             where chosenClassAllSectionsID.ToList().Contains(m.SectionId)
            //                             select m;

            //Console.WriteLine(choices.Count() + "out");
            //if(!aTable.Any())
            //{
            //    foreach (var oneClassSect in chosenClassAllSections)
            //    {
            //        Console.WriteLine("t");
            //        if (!choices.Any())
            //        {
            //            choices = choicesUntouched.ToList();
            //        }
            //        aTable.Add(oneClassSect);
            //        choices.Remove(choices[0]);
            //        Console.WriteLine(choices.Count() + "if");
            //        MakeTimetablesHelper(choices, allTables, aTable);
            //    }
            //}
            //else
            //{
            //    foreach (var sect in aTableCopy)
            //    {
            //        Console.WriteLine("b");
            //        foreach (var oneClassSect in chosenClassAllSections)
            //        {
            //            Console.WriteLine("c");
            //            if (!Collides(oneClassSect, sect))
            //            {
            //                Console.WriteLine("d");
            //                aTable.Add(oneClassSect);
            //                choices.Remove(choices[0]);
            //                Console.WriteLine(choices.Count() + "else");
            //                if (!choices.Any())
            //                {
            //                    if (aTable.Count() == tableLength)
            //                    {
            //                        allTables.Add(aTable);
            //                    }
            //                }
            //                else
            //                {
            //                    MakeTimetablesHelper(choices, allTables, aTable);
            //                }
            //            }
            //        }

            //    }
            //}


            //Console.WriteLine("e");
        //}


    }




