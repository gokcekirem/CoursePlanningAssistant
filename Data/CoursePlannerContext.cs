using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CoursePlanner.Models;

namespace CoursePlanner.Data
{
    public class CoursePlannerContext : DbContext
    {
        public CoursePlannerContext (DbContextOptions<CoursePlannerContext> options)
            : base(options)
        {
            /*Models.Instructor entity = new Models.Instructor();
            entity.Name = "Ozakar, Baris";
            entity.IsPrimary = false;
            Instructor.Add(entity);
            SaveChanges();*/
        }

        public DbSet<CoursePlanner.Models.Class> Class { get; set; }
        public DbSet<CoursePlanner.Models.Career> Career { get; set; }
        public DbSet<CoursePlanner.Models.Instructor> Instructor { get; set; }
        public DbSet<CoursePlanner.Models.Section> Section { get; set; }
        public DbSet<CoursePlanner.Models.Status> Status { get; set; }

    }
}
