using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoursePlanner.Models
{
    public class Instructor
    {
        public int InstructorId { get; set; }
        public string Name { get; set; }
        public Boolean IsPrimary { get; set; }

    }
}
