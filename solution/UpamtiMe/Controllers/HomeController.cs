using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Data;
using Data.DTOs;
using Data.Entities;
using UpamtiMe.Models;

namespace UpamtiMe.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index(Boolean logOut = false)
        {
            if (logOut)
            {
                Session["user"] = null;
            }

            if (Session["user"] != null)
            {
                return RedirectToAction("Profile", "Users", new { id = UserSession.GetUser().UserID });
            }
            else if(!logOut)
            {
                Login(new HomeIndexModel
                {
                    Login = new LoginTransporterDTO { Username = "masa", Password = "plavusha", RememberMe = true }
                });
                return RedirectToAction("Index", "Users", new { id = 1 });
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(Models.HomeIndexModel model)
        {
            Data.DTOs.LoginDTO ld = Data.Entities.Login.CreateLoginDTO(model.Login);
            if (ld.LoginRegisterStatus == Enumerations.LoginRegisterStatus.IncorrectPassword)
            {
                return Json(new {success = false, message = "incorrect password"});
            }
            if(ld.LoginRegisterStatus == Enumerations.LoginRegisterStatus.Failed)
            {
                return Json(new { success = false, message = "failed" });
            }

            if (ld.LoginRegisterStatus == Enumerations.LoginRegisterStatus.Successful)
            {
                UserSession.SetUser(ld);
                UserSession.ReloadSidebar();

                return RedirectToAction("Profile", "Users", new { id = ld.UserID });
            }

            return RedirectToAction("Error");
        }

        [HttpPost]
        public ActionResult Register(Models.HomeIndexModel model)
        {
            Data.DTOs.LoginDTO ld = Data.Entities.Login.CreateRegisterLoginDTO(model.Register);
            if (ld.LoginRegisterStatus == Enumerations.LoginRegisterStatus.UsernameExists)
            {
                return Json(new { success = false, message = "username exists" });
            }
            if (ld.LoginRegisterStatus == Enumerations.LoginRegisterStatus.EmailExists)
            {
                return Json(new { success = false, message = "email exists" });
            }

            Session.Timeout = ld.RememberMe ? 525600 : 20;
            if (ld.LoginRegisterStatus == Enumerations.LoginRegisterStatus.Successful)
            {
                Session["user"] = ld;
                

                return RedirectToAction("Profile", "Users", new {id = ld.UserID});
            }

            return RedirectToAction("Error");
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}