using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Data;
using Data.DTOs;

namespace UpamtiMe
{
    public class SessionExpireFilterAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;

            // check if session is supported
            if (ctx.Session != null)
            {

                // check if a new session id was generated
                if (ctx.Session.IsNewSession)
                {

                    // If it says it is a new session, but an existing cookie exists, then it must
                    // have timed out
                    string sessionCookie = ctx.Request.Headers["Cookie"];
                    if ((null != sessionCookie) && (sessionCookie.IndexOf("ASP.NET_SessionId") >= 0))
                    {
                        //string redirectOnSuccess = filterContext.HttpContext.Request.Url.PathAndQuery;
                        //string redirectUrl = string.Format("?ReturnUrl={0}", redirectOnSuccess);
                        //string loginUrl = FormsAuthentication.LoginUrl + redirectUrl;
                        //if (ctx.Request.IsAuthenticated)
                        //{
                        //    FormsAuthentication.SignOut();
                        //}
                        //RedirectResult rr = new RedirectResult(loginUrl);
                        //filterContext.Result = rr;
                        ctx.Response.Redirect("~/Home/Index/?logout=true");

                    }
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }

    public class UserSession
    {
        public static LoginDTO GetUser()
        {
            //mozda bi ovde mogla da bacis exception ako nije ulogovan
            return (LoginDTO) HttpContext.Current.Session["user"];
        }

        public static int GetUserID()
        {
            LoginDTO usr = GetUser();
            if(usr == null)
                throw new Exception("nije ulogovan");
            return usr.UserID;
        }

        public static void SetUser(LoginDTO u)
        {
            HttpContext.Current.Session["user"] = u;
        }

        public static void SetTime()
        {
            HttpContext.Current.Session["timeSpent"] = DateTime.Now;
        }

        public static DateTime GetTime()
        {
            return (DateTime) HttpContext.Current.Session["timeSpent"];
        }

        public static int GetTimeSpent()
        {
            return (int)DateTime.Now.Subtract(GetTime()).TotalMinutes + 1;
        }

        public static void SetSearchCourses(List<CourseDTO> list)
        {
            HttpContext.Current.Session["searchCourses"] = list;
        }

        public static List<CourseDTO> GetSearchCourses()
        {
            return (List<CourseDTO>) HttpContext.Current.Session["searchCourses"];
        }

        public static void ReloadSidebar()
        {
            HttpContext.Current.Session["sidebar"] = Models.PartialModel.Load(GetUserID());
        }

        public static Models.PartialModel GetSidebar()
        {
            return (Models.PartialModel) HttpContext.Current.Session["sidebar"];
        }

        public static void SetUserCourses(List<Course> list)
        {
            HttpContext.Current.Session["userCourses"] = list;
        }

        public static List<Course> GetUserCourses()
        {
            return (List<Course>)HttpContext.Current.Session["userCourses"];
        }

    }
}