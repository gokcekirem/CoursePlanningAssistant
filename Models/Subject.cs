using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoursePlanner.Models
{
    public class Subject
    {
        public string SubjectId { get; set; } // Comp
        public string Description { get; set; } //  Computer Engineering

        public ICollection<Class> Classes { get; set; }
    }
}
