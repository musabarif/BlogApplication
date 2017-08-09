﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BlogWebsite.Models;
using System.IO;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;

namespace BlogWebsite.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Models.LoginModel model)
        {
            if (Membership.ValidateUser(model.Username, model.Password))
            {
                FormsAuthentication.SetAuthCookie(model.Username, true);

                Session["name"] = model.Username;
                return RedirectToAction("Index", "Posts");
            }
            return RedirectToAction("Login");
        }


        // GET: Account/Register
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            Membership.CreateUser(model.Username, model.Password, model.Email);
            return RedirectToAction("Login");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Posts");
        }

        public ActionResult ShowProfile(string name)
        {
            var user = Membership.GetUser(name);
            ProfileView profile = new ProfileView();
            RegisterModel r = new RegisterModel();
            Author author = new Author();
            r.Email = user.Email;
            r.Username = user.UserName;
            profile.Register = r;
            using (var d = new ApplicationDbContext())
            {
                var image = d.Image.FirstOrDefault(x => x.Username == name);
                if (image == null)
                { ViewBag.profile = "default";}
                else
                { ViewBag.profile = "profile";}
                var auth = d.Authors.FirstOrDefault(x => x.Username == name);
                if (auth != null)
                {
                    if (auth.Fullname != null)
                        author.Fullname = auth.Fullname;
                    if (auth.Expertise != null)
                        author.Expertise = auth.Expertise;
                    if (auth.About != null)
                        author.About = auth.About;
                    profile.Author = author;
                }
                else
                {
                    author.About = "Apparently, this user prefers to keep an air of mystery about them.";
                    profile.Author = author;
                }
            }
            if (User.Identity.Name == name)
                return View(profile);
            else
                return View("Display", profile);
        }


        public ActionResult GetImage(string Username)
        {
            using (var d = new ApplicationDbContext())
            {
                var image = d.Image.Where(x => x.Username == Username).OrderByDescending(x => x.ID).FirstOrDefault();
                if (image != null)
                {
                    byte[] arr = image.Data;
                    MemoryStream ms = new MemoryStream(arr);
                    return new FileContentResult(ms.ToArray(), "image/*");
                }
                else
                {
                    return new EmptyResult();
                }
            }
        }

        [HttpPost]
        public ActionResult ShowProfile(ProfileView model, HttpPostedFileBase file)
        {
            var user = Membership.GetUser(User.Identity.Name);
            user.Email = model.Register.Email;
            Membership.UpdateUser(user);

            //saving image
            if (file != null)
            {
                SaveImageToDatabase(file);
            }

            Author author = new Author();
            author.Username = User.Identity.Name;
            if (model.Author.Fullname != null)
                author.Fullname = model.Author.Fullname;
            if (model.Author.Expertise != null)
                author.Expertise = model.Author.Expertise;
            if (model.Author.About != null)
                author.About = model.Author.About;
            using (var d = new ApplicationDbContext())
            {
                var auth = d.Authors.FirstOrDefault(x => x.Username == user.UserName);
                if (auth == null)
                {
                    d.Authors.Add(author);
                    d.SaveChanges();
                }
                else
                {
                    auth.Fullname = author.Fullname;
                    auth.Expertise = author.Expertise;
                    auth.About = author.About;
                    d.SaveChanges();
                }
            }
            return RedirectToAction("Index", "Posts");
        }

        [NonAction]
        private void SaveImageToDatabase(HttpPostedFileBase file)
        {
            Image myImg = Image.FromStream(file.InputStream, true, true);
            ImageModel img = new ImageModel();
            using (var d = new ApplicationDbContext())
            {
                img.Data = ConvertToBytes(myImg);
                img.Username = User.Identity.Name;
                img.MimeType = myImg.GetType().ToString();
                d.Image.Add(img);
                d.SaveChanges();
            }
        }

        [NonAction]
        private static byte[] ConvertToBytes(Image myImg)
        {
            MemoryStream ms = new MemoryStream();
            myImg.Save(ms, ImageFormat.Jpeg);
            return ms.ToArray();
        }

        public ActionResult Follow(string Follower,string Leader)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            using (var d = new ApplicationDbContext())
            {
                var follower = d.Authors.FirstOrDefault(x => x.Username==Follower);
                var leader = d.Authors.FirstOrDefault(x =>x.Username==Leader);
                Followers folwrs = new Followers();
                folwrs.Follower = follower.ID;
                folwrs.Leader = leader.ID;
                d.Followers.Add(folwrs);
                d.SaveChanges();
            }
                return RedirectToAction("ShowProfile", "Account",new { name=Leader});
        }
    }
}