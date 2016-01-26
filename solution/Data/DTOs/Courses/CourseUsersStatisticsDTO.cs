using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTOs
{
    public class CourseUsersStatisticsDTO
    {
        public LearningStatisticsDTO LearningStatistics { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? LastPlayed { get; set; }
    }
}
