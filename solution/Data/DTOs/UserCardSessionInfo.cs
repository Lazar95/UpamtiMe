using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTOs
{
    public class UserCardSessionInfo
    {
        public int UserCardID { get; set; }
        public int CardID { get; set; }
        public DateTime? LastSeen { get; set; }
        public DateTime? NextSee { get; set; }
        public int NextSeeMinutes { get; set; }
        public int Combo { get; set; }
        public int CorrectAnswers { get; set; }
        public int WrongAnswers { get; set; }
        public double Goodness { get; set; }
    }
}
