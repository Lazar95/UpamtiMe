using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UpamtiMe.Controllers
{
    public class CoursesController : Controller
    {
        // GET: Courses
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateNew(Models.CreateNewCourseModel model)
        {
            if (ModelState.IsValid)
            {
                Data.Courses.addCourse(model.Name, model.CategoryID, model.SubcategoryID, model.NumberOfCards, model.CreatorID);
            }
            else
            {
                // ??
            }

            return View(model);
        }

        public ActionResult EditCourse(int courseID)
        {
            try
            {
                Models.EditCourseModel model = Models.EditCourseModel.Load(courseID);
                return View(model);
            }
            catch(Exception e)
            {
                return RedirectToAction("Error", "Home");
            }
        }


    }
}