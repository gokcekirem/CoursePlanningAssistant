using System;
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
                if (counter >= 2 && counter <= 3788)
                {
                    // getting all the careers
                    getAllCareers(false, reader);

                    // getting all instructors
                    getAllInstructors(false, reader);

                }
            }

            reader.Close();
        }

        public DbSet<CoursePlanner.Models.Class> Class { get; set; }
        public DbSet<CoursePlanner.Models.Career> Career { get; set; }
        public DbSet<CoursePlanner.Models.Instructor> Instructor { get; set; }
        public DbSet<CoursePlanner.Models.Section> Section { get; set; }
        public DbSet<CoursePlanner.Models.Status> Status { get; set; }

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

        private void getAllClasses(bool boo)
        {
            if (boo)
            {
                Console.WriteLine("Im not implemented yet");
            }
        }

        private void getAllSections(bool boo)
        {
            if (boo)
            {
                Console.WriteLine("Im not implemented yet");
            }
        }
    }
}

