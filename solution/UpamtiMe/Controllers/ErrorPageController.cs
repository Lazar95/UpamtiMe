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

            if (exception.Message == "nije on kreator")
            {
                ViewBag.Message = "Nisi kreator kursa, ne mozes da ga editujes";
            }

            if (exception.Message == "Sequence contains no elements")
            {
                ViewBag.Message = "Trazis nesto sto ne postoji";
            }

            if (exception.Message == "nije enrollovan")
            {
                ViewBag.Message = "Nisi prijavljen na kurs!";
            }

            if (Regex.IsMatch(exception.Message,
                @"The controller for path .+ was not found or does not implement IController.+")
                ||
                Regex.IsMatch(exception.Message, @"A public action method .+ was not found on controller.+"))
            {
                ViewBag.Message = "Ta stranica ne postoji";
            }
           
            //ako nije ni jedna od prethodnih poruka, posalji gresku meni na mail

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