using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTOs
{
    public class UserCourseDTO
    {
        public int CourseID { get; set; }
        public string Name { get; set; }
        public byte[] Image { get; set; }
        public int CategoryID { get; set; }
        public int? SubcategoryID { get; set; }
        public LearningStatisticsDTO LearningStatistics { get; set; }
        public StatisctisByDays StatisctisByDays { get; set; }

    }
}
