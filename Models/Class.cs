using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CoursePlanner.Models
{
    public class Class
    {
        public int ClassId { get; set; } // 1020

        [ForeignKey("Instructor")]
        public int InstructorId { get; set; }
        public Instructor Instructor { get; set; }

        [ForeignKey("Subject")]
        public string SubjectId { get; set; } // Comp
        public Subject Subject { get; set; }

        public int Code { get; set; } // 130

        [ForeignKey("Career")]
        public int CareerId { get; set; } 
        public Career Career { get; set; }

        public string Term { get; set; }
        public int Units{ get; set; }
        public string Description { get; set; } //  Introduction to Programming
        public string Prerequisite { get; set; } // Comp 200 and Comp 106

        public ICollection<Section> Sections { get; set; }
    }
}
