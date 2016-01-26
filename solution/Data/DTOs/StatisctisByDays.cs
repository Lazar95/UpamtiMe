using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        //public StatisctisByDays(int timespan = 30)
        //{
        //    PropertyInfo[] properties = typeof(StatisctisByDays).GetProperties();
        //    foreach (PropertyInfo property in properties)
        //    {
        //        property.SetValue(this, "");
        //    }
        //    this.Dates = "";

        //    this.SetZeros(timespan);
        //}

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

        public void SetDates(DateTime startDate, int timeSpan)
        {
            foreach (DateTime day in EachDay(startDate, startDate.AddDays(timeSpan-1)))
            {
                this.Dates += day.Day;
                this.Dates += "|";
            }
        }

        public void SetZeros(int count)
        {
            for (int i = 0; i < count; i++)
            {
                this.AddValues(0, 0, 0, 0, 0, 0, 0, 0, 0);
            }
        }

        public void TrimStrings()
        {
            PropertyInfo[] properties = typeof(StatisctisByDays).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                string pom = (string)property.GetValue(this, null);
                pom = pom.removeExtraSeparator();
                property.SetValue(this, pom);
            }
        }

        public StatisctisByDays Add(StatisctisByDays stat)
        {
            if (this.Dates == null) //ovim pretpostavljam da je ceo objekat prazan
            {
                return new StatisctisByDays
                {
                    Learned = stat.Learned,
                    Dates = stat.Dates,
                    Scores = stat.Scores,
                    Reviewed = stat.Reviewed,
                    LearnCorrect = stat.LearnCorrect,
                    LearnwWrong = stat.LearnwWrong,
                    ReviewCorrect = stat.ReviewCorrect,
                    ReviewWrong = stat.ReviewWrong,
                    Sessions = stat.Sessions,
                    Times = stat.Times
                };
            }

            StatisctisByDays returnValue = new StatisctisByDays();

            if (this.Dates != stat.Dates)
                return null;
            returnValue.Dates = this.Dates;

            PropertyInfo[] properties = typeof(StatisctisByDays).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                //da ne bi sabirao i datume
                if(property == properties[properties.Length-1])
                    continue;
                
                string prop1 = (string)property.GetValue(this, null);
                string prop2 = (string) property.GetValue(stat, null);

                string[] array1 = prop1.Split('|');
                string[] array2 = prop2.Split('|');

                string concat = "";

                if (array1.Length != array2.Length)
                    return null;//baci neki bolji excepion

                for (int i = 0; i < array1.Length; i++)
                {
                    double a = Double.Parse(array1[i]) + double.Parse(array2[i]);
                    concat += a.ToString() + "|";
                }

                concat = concat.Remove(concat.Length - 1);
               
                property.SetValue(returnValue, concat);
            }

            return returnValue;
        }

    }
}
