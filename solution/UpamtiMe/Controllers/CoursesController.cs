using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Data;
using Data.DTOs;
using UpamtiMe.Extensions;
using UpamtiMe.Models;

namespace UpamtiMe.Controllers
{
    public class CoursesController : Controller
    {
        // GET: Courses
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Profile(int id)
        {
            CourseProfileModel model;
            if ( UserSession.GetUser() != null && Users.enrolled(UserSession.GetUser().UserID, id))
            {
                model = CourseProfileModel.Load(id, UserSession.GetUser().UserID);
            }
            else
            {
                model = CourseProfileModel.Load(id);
            }
            
            return View(model);
        }

        public ActionResult Enroll(int id)
        {
            int usrID = UserSession.GetUser().UserID;
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
                Course c = Data.Courses.addCourse(model.Name, model.CategoryID, model.SubcategoryID, model.NumberOfCards, model.CreatorID);
                return RedirectToAction("EditCourse", new {id = c.courseID });
            }
            else
            {
                return Json(new { success = false, result = ModelState.Errors() });
            }
        }

        public ActionResult EditCourse(int id)
        {
            try
            {
                Models.EditCourseModel model = Models.EditCourseModel.Load(id);
                return View(model);
            }
            catch(Exception e)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public ActionResult EditCourse(EditCourseLevelsCards model)
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
                    foreach (LevelsDTO level in model.AddedLevels)
                    {
                        if (level.Cards == null)
                            continue;
                        numAdded += level.Cards.Count;
                    }
                }

                int oldnum = Courses.getCardNuber(model.CourseID);
                int newnum = oldnum + numAdded - numDeleted;
                

                Courses.updateCourseInfo(model.CourseID, model.Name, model.CategoryID, model.SubcategoryID, newnum, model.Description);

                return Json(new {success = true});
            }
            catch (Exception e)
            { 
                return Json(new { success = false});
            }
        }

        public ActionResult Learn(int userID, int courseID, int? levelID, int? numberOfCards)
        {
            SessionModel model = Models.SessionModel.LoadLearningSession(userID, courseID, levelID, numberOfCards);
            return View("SessionTest",model);
        }
    }
}