using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Data;
using Data.DTOs;
using UpamtiMe.Models;

namespace UpamtiMe.Controllers
{
    public class UsersController : InfinateScroll
    {
        // GET: Users
        public ActionResult Index()
        {
            UserIndexModel uim = UserIndexModel.Load(UserSession.GetUserID());
            return View(uim);
        }
      

        public ActionResult Profile(int id)
        {
           
            Models.UserProfileModel model = Models.UserProfileModel.Load(id);
            if (UserSession.GetUser() == null)
            {
                ViewBag.friends = Data.Enumerations.FollowStatus.NotLoggedIn;
            }
            else
            {
                if (UserSession.GetUserID() == id)
                {
                    ViewBag.friends = Data.Enumerations.FollowStatus.Myself;
                }
                else if (Data.Users.follows(UserSession.GetUserID(), id))
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

        
        public ActionResult Follow(int secondID)
        {
            Data.Users.follow(UserSession.GetUserID(), secondID);
            return RedirectToAction("Profile", new {id = secondID});
        }

        public ActionResult Unfollow(int secondID)
        {
            Data.Users.unfollow(UserSession.GetUserID(), secondID);
            return RedirectToAction("Profile", new { id = secondID });
        }

        [HttpPost]
        public ActionResult UploadAvatar(HttpPostedFileBase file)
        {
            if (file != null)
            {
                byte[] array;
                using (MemoryStream ms = new MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    array = ms.GetBuffer();
                }
                Data.Users.editAvatar(UserSession.GetUserID(), array);

            }
            
            //da bi se promenila slika u sidebaru
            UserSession.ReloadSidebar();

            return RedirectToAction("Profile", "Users", new { id = UserSession.GetUserID()});
        }

        [ChildActionOnly]
        public ActionResult GetCoursesChunk(List<UserCourseDTO> model)
        {
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult InfinateScroll(int BlockNumber)
        {
            int initBlockSize = ConfigurationParameters.UserIndexStartCourseNumber;
            int BlockSize = ConfigurationParameters.CoursesUserIndexInfiniteScrollBlockSize;

            JsonModel jsonModel = new JsonModel();

            List<Course> allCourses = UserSession.GetUserCourses();
            List<Course> courses;
            List<UserCourseDTO> returnValue;

            int baseNo = initBlockSize + BlockSize * BlockNumber;
            jsonModel.min = baseNo + 1;
            int limit = baseNo + BlockSize;
            if (limit >= allCourses.Count)
            {
                jsonModel.NoMoreData = true;
                courses = allCourses.GetRange(baseNo, allCourses.Count - baseNo);
                jsonModel.max = baseNo + allCourses.Count - baseNo;
            }
            else
            {
                jsonModel.NoMoreData = false;
                courses = allCourses.GetRange(baseNo, BlockSize);
                jsonModel.max = baseNo + BlockSize ;
            }

            

            returnValue = Data.Courses.CreateUserCourseDTOs(UserSession.GetUserID(), courses);
            jsonModel.HTMLString = RenderPartialViewToString("GetCoursesChunk", returnValue);
            return Json(jsonModel);
        }


        public ActionResult GetAvatar(int id)
        {
            byte[] image = Data.Users.GetUser(id).avatar.ToArray();
            return File(image, "image/jpg");
        }
    }
}