using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTOs
{
    public class UserCourseDTO : Data.DTOs.CourseDTO
    {
        public int CourseID { get; set; }
      
        public int? Favorite { get; set; }
       
        public CourseUsersStatisticsDTO LearningStatistics { get; set; }
        public StatisctisByDays StatisctisByDays { get; set; }

       
    }
}
