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
    }
}