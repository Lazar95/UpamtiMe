﻿using System;
using System.Collections.Generic;
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


    }
}