using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Timers;
using UpamtiMe.Controllers;


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

        public void Application_Error(Object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            Server.ClearError();

            var routeData = new RouteData();
            routeData.Values.Add("controller", "ErrorPage");
            routeData.Values.Add("action", "Error");
            routeData.Values.Add("exception", exception);

            if (exception.GetType() == typeof(HttpException))
            {
                routeData.Values.Add("statusCode", ((HttpException)exception).GetHttpCode());
            }
            else
            {
                routeData.Values.Add("statusCode", 500);
            }

            Response.TrySkipIisCustomErrors = true;
            IController controller = new ErrorPageController();
            controller.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
            Response.End();
        }

        private static void SetTimer()
        {
            DateTime tomorow = DateTime.Now.AddDays(1);
            DateTime midnight = new DateTime(tomorow.Year, tomorow.Month, tomorow.Day, 0, 0, 0);
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
            if (UserSession.GetUser() != null)
                UserSession.ReloadSidebar();
        }


    }
}
