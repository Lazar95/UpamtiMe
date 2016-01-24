using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UpamtiMe.Models;

namespace UpamtiMe.Controllers
{
    public class UsersController : Controller
    {
        // GET: Users
        public ActionResult Index(int id)
        {
            UserIndexModel uim = UserIndexModel.Load(id);
            return View(uim);
        }

        [HttpPost]
        public ActionResult Index(Models.UserIndexModel model)
        {
            if (ModelState.IsValid)
            {
                //Data.Users.addUser(model.Name, model.Username, model.Password, model.Email);
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
                if (UserSession.GetUser() == null)
                {
                    ViewBag.friends = Data.Enumerations.FollowStatus.NotLoggedIn;
                }
                else
                {
                    if (UserSession.GetUser().UserID == id)
                    {
                        ViewBag.friends = Data.Enumerations.FollowStatus.Myself;
                    }
                    else if (Data.Users.follows(UserSession.GetUser().UserID, id))
                    {
                        ViewBag.friends = Data.Enumerations.FollowStatus.Following;
                    }
                    else
                    {
                        ViewBag.friends = Data.Enumerations.FollowStatus.NotFollowing;
                    }

                }
                return View(model);
            }
            catch(Exception e)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        
        public ActionResult Follow(int firstID, int secondID)
        {
            Data.Users.follow(firstID, secondID);
            return RedirectToAction("Profile", new {id = secondID});
        }

        public ActionResult Unfollow(int firstID, int secondID)
        {
            Data.Users.unfollow(firstID, secondID);
            return RedirectToAction("Profile", new { id = secondID });
        }

        [HttpPost]
        public ActionResult UploadAvatar(HttpPostedFileBase file, int userID)
        {
            if (file != null)
            {
                byte[] array;
                using (MemoryStream ms = new MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    array = ms.GetBuffer();
                }
                Data.Users.editAvatar(userID, array);

            }
            
            return RedirectToAction("Profile", "Users", new { id = userID});
        }

        [ChildActionOnly]
        public ActionResult UserIndexCoursesChunk(List<int> Model)
        {
            Model = new List<int>();
            for(int i=0; i < 8; i++)
                Model.Add(i);
            return PartialView(Model);
        }

        [HttpPost]
        public ActionResult InfinateScroll(int BlockNumber)
        {
            int BlockSize = 8;

            List<int> brojke = new List<int>();
            for (int i = 0; i < BlockSize; i++)
                brojke.Add(i + BlockNumber*10);

            JsonModel jsonModel = new JsonModel();
            jsonModel.NoMoreData = brojke.Count < BlockSize;
            jsonModel.HTMLString = RenderPartialViewToString("UserIndexCoursesChunk", brojke);
            return Json(jsonModel);
        }

        public class JsonModel
        {
            public string HTMLString { get; set; }
            public bool NoMoreData { get; set; }
        }

        protected string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult =
                ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext
                (ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

    }
}