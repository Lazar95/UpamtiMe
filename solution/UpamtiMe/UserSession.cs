using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Data.DTOs;

namespace UpamtiMe
{
    public class UserSession
    {
        public static LoginDTO GetUser()
        {
            return (LoginDTO) HttpContext.Current.Session["user"];
        }

        public static void SetTime()
        {
            HttpContext.Current.Session["timeSpent"] = DateTime.Now;
        }

        public static DateTime GetTime()
        {
            return (DateTime) HttpContext.Current.Session["timeSpent"];
        }
    }
}