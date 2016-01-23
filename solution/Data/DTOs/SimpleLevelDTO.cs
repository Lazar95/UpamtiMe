using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTOs
{
    public class SimpleLevelDTO
    {
        public int LevelID { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        public int CardNumber { get; set; }
        public int Icon { get; set; }
        public int Color { get; set; }
        public LearningStatisticsDTO LearningStatistics { get; set; }

    }
}
