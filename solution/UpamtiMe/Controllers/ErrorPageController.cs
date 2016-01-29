using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

            if (Regex.IsMatch(exception.Message,
                @"The controller for path .+ was not found or does not implement IController.+")
                ||
                Regex.IsMatch(exception.Message, @"A public action method .+ was not found on controller.+"))
            {
                ViewBag.Message = "Ta stranica ne postoji";
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