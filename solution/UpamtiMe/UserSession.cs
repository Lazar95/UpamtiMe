using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Data;
using Data.DTOs;

namespace UpamtiMe
{
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