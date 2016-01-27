using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UpamtiMe
{
    //imam dva puta klasu extentions zato sto u data ne mogu da pristupim konfiugracionim parametrima
    public static class Extentions
    {
        public static DateTime MyToday()
        {
            int h = ConfigurationParameters.MidnightHours;
            return new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, h, 0, 0);
        }
    }
}