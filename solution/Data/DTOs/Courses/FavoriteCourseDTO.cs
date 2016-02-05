using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTOs
{
    public class FavoriteCourseDTO
    {
        public int CourseID { get; set; }
        public string Name { get; set; }
        public byte[] Image { get; set; }
        public LearningStatisticsDTO LearningStatistics { get; set; }
    }
}
