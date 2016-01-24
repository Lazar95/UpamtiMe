using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTOs
{
    public class StatisctisByDays
    {
        public string Scores { get; set; }
        public string Learned { get; set; }
        public string Reviewed { get; set; }
        public string Sessions { get; set; }
        public string Times { get; set; }
        public string LearnCorrect { get; set; }
        public string LearnwWrong { get; set; }
        public string ReviewCorrect { get; set; }
        public string ReviewWrong { get; set; }

        public string Dates { get; set; }

        public void AddValues(double score, int learned, int reviewed, int sessions, int times, int learnCorrect,
            int learnWrong, int ReviewCorrect, int reviewWrong)
        {
            this.Scores += score.ToString() + "|";
            this.Learned += learned.ToString() + "|";
            this.Reviewed += reviewed.ToString() + "|";
            this.Sessions += sessions.ToString() + "|";
            this.Times += times.ToString() + "|";
            this.LearnCorrect += learnCorrect.ToString() + "|";
            this.LearnwWrong += learnWrong.ToString() + "|";
            this.ReviewCorrect += ReviewCorrect.ToString() + "|";
            this.ReviewWrong += reviewWrong.ToString() + "|";
        }

        public IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }

        public void SetDates(DateTime startDate)
        {
            foreach (DateTime day in EachDay(startDate, startDate.AddDays(30)))
            {
                this.Dates += day.Day;
                this.Dates += "|";
            }
            this.Dates.Remove(this.Dates.Count() - 1);
        }
       
    }
}
