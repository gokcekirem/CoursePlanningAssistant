using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoursePlanner.Models
{
    public class Class
    {
        public int ClassId { get; set; } // 1020
        public int InstructorId { get; set; }
        public string SubjectId { get; set; } // Comp
        public int Code { get; set; } // 130
        public int CareerId { get; set; }
        public string Term { get; set; }
        public int Units{ get; set; }
        public string Description { get; set; } //  Introduction to Programming

    }
}
