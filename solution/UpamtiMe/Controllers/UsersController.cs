using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UpamtiMe.Controllers
{
    public class UsersController : Controller
    {
        // GET: Users
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(Models.UserIndexModel model)
        {
            if (ModelState.IsValid)
            {
                Data.Users.addUser(model.Name, model.Username, model.Password, model.Email);
                return View(model);
            }
            else
            {
                // TODO redirect
            }

            return View(model);
        }

        public ActionResult Profile(int id)
        {
            try
            {
                Models.ProfilePageModel model = Models.ProfilePageModel.Load(id);
                return View(model);
            }
            catch(Exception e)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public ActionResult TestPage(Data.Test model)
        {
            int a = model.a;
            int b = model.lazar;
            //return RedirectToAction("Profile", "1");
            return Json(new { success = true });
        }
    }
}