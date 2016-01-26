using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTOs
{
    public class LevelWithStatisticsDTO : LevelBasicDTO
    {
        public int CardNumber { get; set; }
        public LearningStatisticsDTO LearningStatistics { get; set; }

    }
}
