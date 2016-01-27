using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Timers;



namespace UpamtiMe
{
    public class MvcApplication : System.Web.HttpApplication
    {
        static Timer sTimer = new Timer();
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
          
            
            SetTimer();
        }

        private static void SetTimer()
        {
            DateTime midnight = Extentions.MyToday().AddDays(1);
            
            TimeSpan d = midnight - DateTime.Now;

           
            sTimer.Enabled = true;
            sTimer.Interval = d.TotalMilliseconds;
            sTimer.Elapsed += new System.Timers.ElapsedEventHandler(sTimer_Elapsed);
            sTimer.Start();
        }

        static void sTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            sTimer.Interval = TimeSpan.FromHours(24).TotalMilliseconds;
            Data.Users.resetStatisticsForAllUsers();

        }
    }
}
