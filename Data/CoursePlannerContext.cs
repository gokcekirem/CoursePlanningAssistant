﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CoursePlanner.Models;
using System.IO;
using ExcelDataReader;

namespace CoursePlanner.Data
{
    public class CoursePlannerContext : DbContext
    {
        public CoursePlannerContext(DbContextOptions<CoursePlannerContext> options)
            : base(options)
        {
            bool permisson = false; // keep it false if you are not going to write anything to the db

            if (permisson)
            {
                String filePath = @"KUSIS_Class_Data\dataWithCourseCode.xlsx";
                FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                IExcelDataReader reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                int counter = 0;

                // getting all status
                getAllStatus(false);

                while (reader.Read())
                {
                    counter++;
                    if (counter >= 2 && counter <= 3788) // from 2 to 3788
                    {
                        // getting all the careers
                        getAllCareers(false, reader);

                        // getting all instructors
                        getAllInstructors(false, reader);

                        // get all classes
                        getAllClasses(false, reader);

                        //get all sections 
                        getAllSections(false, reader);

                    }
                }

                reader.Close();

                // get prerequisites


                String filePath2 = @"KUSIS_Class_Data\prerequisites.xlsx";
                FileStream stream2 = File.Open(filePath2, FileMode.Open, FileAccess.Read);
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                IExcelDataReader reader2 = ExcelReaderFactory.CreateOpenXmlReader(stream2);
                int counter2 = 0;

                while (reader2.Read())
                {
                    counter2++;
                    if (counter2 >= 2 && counter2 <= 1087) // from 2 to 1087
                    {
                        // getting all prerequsites
                        getAllPrerequisites(false, reader2);

                    }
                }
                reader2.Close();
            }
            
        }
        


        public DbSet<CoursePlanner.Models.Class> Class { get; set; }
        public DbSet<CoursePlanner.Models.Career> Career { get; set; }
        public DbSet<CoursePlanner.Models.Instructor> Instructor { get; set; }
        public DbSet<CoursePlanner.Models.Section> Section { get; set; }
        public DbSet<CoursePlanner.Models.Status> Status { get; set; }

        private void getAllPrerequisites(bool boo, IExcelDataReader reader2)
        {
            if (boo)
            {
                string type = reader2.GetString(9); //must be equal to 'PRE'

                if (reader2.GetValue(3) != null && string.Compare(type, "PRE") == 0)
                {
                    string subject = reader2.GetString(3); // must not be empty
                    int code = Int32.Parse(reader2.GetString(4));

                    var classId = from m in Class
                                  where m.Subject == subject
                                  where m.Code == code
                                  select m.ClassId;



                    if (classId.ToList().Count > 0)
                    {
                        Class c = Class.Where(s => s.ClassId.Equals(classId.ToList()[0])).FirstOrDefault<Class>();

                        string old = c.Prerequisite;

                        string prereq_subject = reader2.GetString(5);
                        int prereq_code = Int32.Parse(reader2.GetString(6));

                        string conn = reader2.GetString(13);
                        string parenth = reader2.GetString(14);

                        string prereq_sentence = " " + conn + " " + prereq_subject + prereq_code;

                        c.Prerequisite = old + prereq_sentence;

                        SaveChanges();

                        //Console.WriteLine("Line:" + counter2 + "\t" + subject + code + "\t" + c.Prerequisite + "\n");

                    }
                }
            }
        }

        private void getAllCareers(bool boo, IExcelDataReader reader)
        {
            if (boo)
            {
                string career_name = reader.GetString(10);
                if (!Career.Any(o => o.Description.Equals(career_name)))
                {
                    Career c = new Career();
                    c.Description = career_name;
                    Career.Add(c);
                    SaveChanges();
                }
            }
        }

        private void getAllInstructors(bool boo, IExcelDataReader reader)
        {
            if (boo)
            {
                string instructor_name = reader.GetString(29);
                if (!Instructor.Any(o => o.Name.Equals(instructor_name)))
                {
                    Instructor newInst = new Instructor();
                    newInst.Name = instructor_name;
                    if (string.Compare(reader.GetString(28), "PI") == 0)
                    {
                        newInst.IsPrimary = true;
                    }
                    else
                    {
                        newInst.IsPrimary = false;
                    }
                    Instructor.Add(newInst);
                    SaveChanges();
                }
            }
        }

        private void getAllStatus(bool boo)
        {
            if (boo)
            {
                // At the moment, all the sections are avaiblable
                Status s1 = new Status();
                Status s2 = new Status();
                Status s3 = new Status();
                Status s4 = new Status();
                s1.Description = "A"; // Available status: this one exists like this in our db
                s2.Description = "Not offered";
                s3.Description = "Full";
                s4.Description = "Waitlist";
                Status.Add(s1);
                Status.Add(s2);
                Status.Add(s3);
                Status.Add(s4);
                SaveChanges();
            }
        }

        private void getAllClasses(bool boo, IExcelDataReader reader)
        {
            if (boo)
            {
                string class_description = reader.GetString(5);

                if (!Class.Any(o => o.Description.Equals(class_description)))

                {
                    Class newClass = new Class();

                    string instructor = reader.GetString(29);
                    Instructor i = Instructor.Where(s => s.Name.Equals(instructor)).FirstOrDefault<Instructor>();
                    newClass.InstructorId = i.InstructorId;

                    newClass.Subject = reader.GetString(1);
                    newClass.Code = Int32.Parse(reader.GetString(2));

                    string career = reader.GetString(10);
                    Career c = Career.Where(s => s.Description.Equals(career)).FirstOrDefault<Career>();
                    newClass.CareerId = c.CareerId;

                    newClass.Term = "Fall 2019";
                    newClass.Description = class_description;

                    Class.Add(newClass);
                    SaveChanges();
                }
            }
        }

        private void getAllSections(bool boo, IExcelDataReader reader)
        {
            if (boo)
            {
                Section newSection = new Section();

                string class_description = reader.GetString(5);
                Class c = Class.Where(s => s.Description.Equals(class_description)).FirstOrDefault<Class>();

                newSection.ClassId = c.ClassId;
                newSection.Type = reader.GetString(0);
                newSection.StatusId = 1; // 'A'
                newSection.Capacity = Convert.ToInt32((reader.GetDouble(22)));
                newSection.RemainingSeats = Convert.ToInt32((reader.GetDouble(22))) - Convert.ToInt32((reader.GetDouble(8)));

                string times = "";

                if (reader.GetValue(12) != null)
                {
                    if (reader.GetString(12).Equals("Y"))
                    {
                        times = times + "Mon ";
                    }
                    if (reader.GetString(13).Equals("Y"))
                    {
                        times = times + "Tue ";
                    }
                    if (reader.GetString(14).Equals("Y"))
                    {
                        times = times + "Wed ";
                    }
                    if (reader.GetString(15).Equals("Y"))
                    {
                        times = times + "Thu ";
                    }
                    if (reader.GetString(16).Equals("Y"))
                    {
                        times = times + "Fri ";
                    }
                }


                //

                if (reader.GetValue(23) != null || reader.GetValue(24) != null)
                {
                    times = times + reader.GetDateTime(23).Hour + ":" + reader.GetDateTime(23).Minute + "-" + reader.GetDateTime(24).Hour + ":" + reader.GetDateTime(24).Minute;

                }
                newSection.Times = times;

                newSection.Room = reader.GetString(20);

                Section.Add(newSection);
                SaveChanges();
            }
        }
    }
}

