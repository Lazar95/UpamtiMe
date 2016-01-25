using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTOs
{
    public class UserCourseDTO
    {
        public int CourseID { get; set; }
        //slika kursa
        public string Name { get; set; }
        public int CategoryID { get; set; }
        public int? SubcategoryID { get; set; }
        public LearningStatisticsDTO LearningStatistics { get; set; }
        //poeni u zadnjih nedelju dana

    }
}
