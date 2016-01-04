using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTOs
{
    public class LeaderboardEntryDTO
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string FristName { get; set; }
        public string LastName { get; set; }
        public double AllTimeScore { get; set; }
        public double WeekScore { get; set; }
        public double MonthScore { get; set; }
    }
}
