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
        public CoursePlannerContext (DbContextOptions<CoursePlannerContext> options)
            : base(options)
        {
            String filePath = @"KUSIS_Class_Data\dataWithCourseCode.xlsx";
            FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            IExcelDataReader reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            int counter = 0;
            while (reader.Read())
            {
                counter++;

                if (counter >= 2 && counter <= 4)
                {
                    Instructor newInst = new Instructor();
                    newInst.Name = reader.GetString(29);
                    if(string.Compare(reader.GetString(28), "PI") == 0)
                    {
                        newInst.IsPrimary = true;
                    }
                    else
                    {
                        newInst.IsPrimary = false;
                    }
                    Instructor.Add(newInst);
                    /*
                    Class newClass = new Class();
                    newClass.InstructorId = 1;
                    newClass.Subject = reader.GetString(1);
                    Console.WriteLine(reader.GetString(2));
                    newClass.Code = 1;
                    newClass.CareerId = 1;
                    newClass.Term = reader.GetString(31);
                    newClass.Units = 1;
                    newClass.Description = reader.GetString(5);
                    newClass.Prerequisite = "5";
                    Class.Add(newClass);
                    */
                }
            }

            reader.Close();
            SaveChanges();
        }

        public DbSet<CoursePlanner.Models.Class> Class { get; set; }
        public DbSet<CoursePlanner.Models.Career> Career { get; set; }
        public DbSet<CoursePlanner.Models.Instructor> Instructor { get; set; }
        public DbSet<CoursePlanner.Models.Section> Section { get; set; }
        public DbSet<CoursePlanner.Models.Status> Status { get; set; }

    }
}
