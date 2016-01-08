using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTOs
{
    public class LearningStatisticsDTO
    {
        public int Total { get; set; }
        public int Learned { get; set; }
        public int Review { get; set; }
        public int Unseen { get; set; }
    }
}
