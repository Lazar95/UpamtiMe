using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UpamtiMe.Controllers
{
    public class ErrorPageController : Controller
    {
        public ActionResult Error(int statusCode, Exception exception)
        {
            if (exception.Message == "nije ulogovan")
            {
                return RedirectToAction("Index", "Home");
            }
            if (exception.Message == "Sequence contains no elements")
            {
                ViewBag.Message = "Trazis nesto sto ne postoji";
            }

            if (exception.Message ==
                "The controller for path '/sdsada' was not found or does not implement IController."
                ||
                exception.Message ==
                "A public action method 'dfasd' was not found on controller 'UpamtiMe.Controllers.UsersController'.")
            {
                
            }
            

            Response.StatusCode = statusCode;
            ViewBag.StatusCode = statusCode + " Error";
            return View(exception);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}