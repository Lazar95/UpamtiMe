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

        public LearningStatisticsDTO Add(LearningStatisticsDTO ls)
        {
            return new LearningStatisticsDTO
            {
                Total = this.Total + ls.Total,
                Review = this.Review + ls.Review,
                Learned = this.Learned + ls.Learned,
                Unseen = this.Unseen + ls.Unseen
            };
        }
    }

    
}
