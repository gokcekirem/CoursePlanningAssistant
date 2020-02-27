using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoursePlanner.Models
{
    public class Section
    {
        public int SectionId { get; set; }
        public int ClassId { get; set; }
        public string Type { get; set; }
        public int StatusId { get; set; }
        public string Times { get; set; }
        public string Room { get; set; }
    }
}
