using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Data;
using Data.DTOs;
using UpamtiMe.Extensions;
using UpamtiMe.Models;

namespace UpamtiMe.Controllers
{
    public class CoursesController : InfinateScroll
    {
        // GET: Courses
        public ActionResult Index(string search = null, int? categoryID = null, int? subcategoryID = null)
        {
            CourseIndexModel model = CourseIndexModel.Load(search, categoryID, subcategoryID);
            return View(model);
        }
      
        [ChildActionOnly]
        public ActionResult GetCoursesChunk(List<CourseDTO> list)
        {
            return PartialView(list);
        }

        [HttpPost]
        public ActionResult InfinateScroll(int BlockNumber)
        {
            int initBlockSize = ConfigurationParameters.CourseIndexStartCourseNumber;
            int BlockSize = ConfigurationParameters.CoursesSearchInfiniteScrollBlockSize;


            JsonModel jsonModel = new JsonModel();

            List<CourseDTO> allCourses = UserSession.GetSearchCourses();
            List<CourseDTO> courses;

            int baseNo = initBlockSize + BlockSize*BlockNumber;
            int limit = baseNo + BlockSize;
            if (limit >= allCourses.Count)
            {
                jsonModel.NoMoreData = true;
                courses = allCourses.GetRange(baseNo, allCourses.Count - baseNo);
            }
            else
            {
                jsonModel.NoMoreData = false;
                courses = allCourses.GetRange(baseNo, BlockSize);

            }

            jsonModel.HTMLString = RenderPartialViewToString("GetCoursesChunk", courses);
            return Json(jsonModel);
        }

        public ActionResult Profile(int id)
        {
            CourseProfileModel model;
            if ( UserSession.GetUser() != null && Users.enrolled(UserSession.GetUserID(), id))
            {
                model = CourseProfileModel.Load(id, UserSession.GetUserID());
            }
            else
            {
                model = CourseProfileModel.Load(id);
            }
            
            return View(model);
        }

        public ActionResult Enroll(int id)
        {
            int usrID = UserSession.GetUserID();
            Users.enroll(usrID, id);
            return RedirectToAction("Profile", new {id = id});
        }

        public ActionResult CreateNew()
        {
            Models.CreateNewCourseModel model = new CreateNewCourseModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateNew(Models.CreateNewCourseModel model)
        {
            if (ModelState.IsValid)
            {
                int usrID = UserSession.GetUserID();
                Course c = Data.Courses.addCourse(model.Name, model.CategoryID, model.SubcategoryID, model.NumberOfCards, usrID);
                return RedirectToAction("Edit", new {id = c.courseID });
            }
            else
            {
                return Json(new { success = false, result = ModelState.Errors() });
            }
        }

        public ActionResult Edit(int id)
        {
            Models.EditCourseModel model = Models.EditCourseModel.Load(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(EditCourseLevelsCards model)
        {
            try
            {
                int numAdded = 0;
                int numDeleted = 0;

                if (model.DeletedCards != null)
                {
                    Cards.deleteCards(model.DeletedCards);
                    numDeleted += model.DeletedCards.Count;
                }
                    
                if (model.DeletedLevels != null)
                    Levels.deleteLevels(model.DeletedLevels);

                if (model.EditedCards != null)
                    Cards.editCards(model.EditedCards);
                   
                    
                if (model.EditedLevels != null)
                    Levels.editLevels(model.CourseID,model.EditedLevels);

                if (model.AddedCards != null)
                {
                    Cards.addCards(model.AddedCards);
                    numAdded += model.AddedCards.Count;
                }

                if (model.AddedLevels != null)
                {
                    Levels.addLevels(model.CourseID, model.AddedLevels);
                    foreach (LevelWithCardsDTO level in model.AddedLevels)
                    {
                        if (level.Cards == null)
                            continue;
                        numAdded += level.Cards.Count;
                    }
                }

                int oldnum = Courses.getCardNuber(model.CourseID);
                int newnum = oldnum + numAdded - numDeleted;
                

                Courses.updateCourseInfo(model.CourseID, model.Name, model.CategoryID, model.SubcategoryID, newnum, model.Description);

                return Json(new { success = true });
            }
            catch (Exception e)
            { 
                return Json(new { success = false});
            }
        }

        [HttpPost]
        public ActionResult EditImage(HttpPostedFileBase file, int courseID)
        {
            if (file != null)
            {
                byte[] array;
                using (MemoryStream ms = new MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    array = ms.GetBuffer();
                }
                Data.Courses.editImage(courseID, array);
            }

            return RedirectToAction("Profile", "Courses", new { id = courseID });
        }

        public ActionResult Learn(int courseID, int? levelID, int? numberOfCards)
        {
            CheckIfEnrolled(courseID);

            SessionModel model = Models.SessionModel.LoadLearningSession(UserSession.GetUserID(), courseID, levelID, numberOfCards);
            if (model.Cards.Count < 1)
            {
                return RedirectToAction("Error", "Home");
                // neka baci neki bolji exception
            }
            UserSession.SetTime();
            return View("Learn",model);
        }

        [HttpPost]
        public ActionResult Learn(List<CardUserDTO> qaInfo , float score, int courseID)
        {
            int userID = UserSession.GetUserID();


            int timeSpent = UserSession.GetTimeSpent();

            //upisi u usercard
            Data.DTOs.CorrectWrong cw = Data.Cards.CreateUserCard(qaInfo, userID);

            //upisi u tabelu sa statistikama i userCourses
            bool firstSession = Data.Courses.updateStatistics(courseID, userID, score, qaInfo.Count, 0, cw.Correct, cw.Wrong, 0, 0,
                timeSpent);

            //upisi u user-a
            Data.Users.updateStatisctics(userID, score, qaInfo.Count, firstSession);

            UserSession.ReloadSidebar();

            return Json(new { success = true});
        }

      

        public ActionResult Review(int courseID, int? levelID, int? numberOfCards)
        {
            CheckIfEnrolled(courseID);

            SessionModel model = Models.SessionModel.LoadReviewSession(UserSession.GetUserID(), courseID, levelID, numberOfCards);
            if (model.Cards.Count < 1)
            {
                return RedirectToAction("Error", "Home");
                // neka baci neki bolji exception
            }
            UserSession.SetTime();
            return View("Review", model);
        }

        [HttpPost]
        public ActionResult Review(List<CardUserDTO> qaInfo, float score, int courseID)
        {
            int userID = UserSession.GetUserID();


            int timeSpent = UserSession.GetTimeSpent();

            //upisi u usercard
            Data.DTOs.CorrectWrong cw = Data.Cards.UpdateUserCards(qaInfo);

            //upisi u tabelu sa statistikama i userCourses
            bool streak = Data.Courses.updateStatistics(courseID, userID, score, 0, qaInfo.Count, 0, 0, cw.Correct, cw.Wrong, 
                timeSpent);

            //upisi u user-a
            Data.Users.updateStatisctics(userID, score, 0, streak);

            UserSession.ReloadSidebar();

            return Json(new { success = true });
        }

        public ActionResult Linky(int courseID, int? levelID, int? numberOfCards)
        {
            CheckIfEnrolled(courseID);

            SessionModel model = Models.SessionModel.LoadLinkySession(UserSession.GetUserID(), courseID, levelID, numberOfCards);
            UserSession.SetTime();
            return View("Linky", model);
        }

        [HttpPost]
        public ActionResult Linky(float score, int courseID)
        {
            int userID = UserSession.GetUserID();

            int timeSpent = UserSession.GetTimeSpent();

            //upisi u tabelu sa statistikama i userCourses
            bool streak = Data.Courses.updateStatistics(courseID, userID, score, 0, 0, 0, 0, 0, 0, timeSpent);

            //upisi u user-a
            Data.Users.updateStatisctics(userID, score, 0, streak);

            UserSession.ReloadSidebar();

            return Json(new { success = true });
        }

        public ActionResult Favorite(int courseID)
        {
            CheckIfEnrolled(courseID);

            Data.Courses.setFavorite(courseID, UserSession.GetUserID(), 1);

            UserSession.ReloadSidebar();
            return RedirectToAction("Profile", new {id = courseID});
        }

        public ActionResult UnFavorite(int courseID)
        {
            CheckIfEnrolled(courseID);

            Data.Courses.setFavorite(courseID, UserSession.GetUserID(), null);

            UserSession.ReloadSidebar();
            return RedirectToAction("Profile", new { id = courseID });
        }

        public void CheckIfEnrolled(int courseID)
        {
            if (!Data.Users.enrolled(UserSession.GetUserID(), courseID))
                throw new Exception("nije enrollovan");
        }

        public ActionResult CardsPartial(List<Data.DTOs.CardCourseProfileDTO> model)
        {
            return PartialView(model);
        }

        public ActionResult GetCardsOfLevel(int levelID)
        {
            LoginDTO usr = UserSession.GetUser();
            List<Data.DTOs.CardCourseProfileDTO> model = Data.Cards.getCards(levelID, usr?.UserID);

            JsonModel jsonModel = new JsonModel();
            jsonModel.HTMLString = RenderPartialViewToString("CardsPartial", model);
            return Json(jsonModel);
        }
    }
}