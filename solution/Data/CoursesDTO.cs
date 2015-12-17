using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class CoursesDTO
    {
        public int CourseID { get; set; }
        public string Name { get; set; }
        public int CategoryID { get; set; }
        public int SubcategoryID { get; set; }
        public int ParticipantCount { get; set; }
        public int CreatorID { get; set; }
        public List<Data.LevelsDTO> Levels { get; set; }
    }
}
