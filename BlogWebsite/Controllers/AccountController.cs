using System;
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
using System.Data.Entity;
using System.Data.Entity.Core;

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

        public ActionResult ShowProfile(string name, string follow = " ")
        {
            var user = Membership.GetUser(name);
            ProfileView profile = new ProfileView();
            RegisterModel r = new RegisterModel();
            Author author = new Author();
            r.Email = user.Email;
            r.Username = user.UserName;
            profile.Register = r;
            ViewBag.follow = follow;
            using (var d = new ApplicationDbContext())
            {
                var image = d.Image.FirstOrDefault(x => x.Username == name);
                if (image == null)
                { ViewBag.profile = "default"; }
                else
                { ViewBag.profile = "profile"; }
                var auth = d.Authors.FirstOrDefault(x => x.Username == name);
                var post = d.Posts.Where(x => x.Author == name).Count();

                author.Posts = post;
                if (auth != null)
                {
                    auth.Views = auth.Views + 1;
                    d.Entry(auth).State = EntityState.Modified;
                    d.SaveChanges();
                    author.Views = auth.Views;
                    author.Followers = auth.Followers;
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

        public ActionResult Follow(string Follower, string Leader)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            var display = " ";
            using (var d = new ApplicationDbContext())
            {

                var follower = d.Authors.FirstOrDefault(x => x.Username == Follower);
                var leader = d.Authors.FirstOrDefault(x => x.Username == Leader);
                var already = d.Followers.Where(x => x.Leader == leader.ID && (x.Follower == follower.ID)).Any();
                if (!already)
                {
                    leader.Followers = leader.Followers + 1;
                    d.Entry(leader).State = EntityState.Modified;
                    d.SaveChanges();
                    Followers folwrs = new Followers();
                    folwrs.Follower = follower.ID;
                    folwrs.Leader = leader.ID;
                    d.Followers.Add(folwrs);
                    d.SaveChanges();
                    display = string.Format("You are Now Following {0}", Leader);
                }
                else
                {
                    display = string.Format("You are already Following {0}", Leader);
                }
            }
            return RedirectToAction("ShowProfile", "Account", new { name = Leader, follow = display });
        }


        public ActionResult Notification()
        {
            using (var d = new ApplicationDbContext())
            {
                var posts = d.Posts.Where(x => x.Author == User.Identity.Name);
                List<Like> likes = new List<Like>();
                List<Notifications> notify = new List<Notifications>();
                List<Comment> comment = new List<Comment>();
                foreach (var item in posts)
                {
                    likes = d.Like.Where(x => x.Post_ID == item.ID).ToList();
                    comment = d.Comments.Where(x => x.PostID == item.ID).ToList();
                }

                LikeNotification(likes);
                CommentNotification(comment);
                notify = d.Notifications.Where(x => x.UserName == User.Identity.Name).ToList();
                return View(notify);
            }

        }

        private void CommentNotification(List<Comment> comment)
        {
            using (var d = new ApplicationDbContext())
            {
                foreach (var item in comment)
                {
                    Notifications n = new Notifications();
                    if (!AlreadyExists(item))
                    {
                        n.Post_ID = item.ID;
                        n.Action = "Commented";
                        n.Name = item.Name;
                        n.UserName = User.Identity.Name;
                        d.Notifications.Add(n);
                        d.SaveChanges();
                    }
                }
            }
        }

        private bool AlreadyExists(Comment item)
        {
            using (var d = new ApplicationDbContext())
            {
                var exist = d.Notifications.Any(x => x.Post_ID == item.PostID
                            && x.UserName == User.Identity.Name && x.Name == item.Name
                            && x.Action == "Commented");
                return exist;
            }
        }


        private void LikeNotification(List<Like> likes)
        {
            using (var d = new ApplicationDbContext())
            {
                foreach (var item in likes)
                {
                    Notifications n = new Notifications();
                    if (!AlreadyExists(item))
                    {
                        n.Post_ID = item.Post_ID;
                        n.Action = "Voted";
                        n.Name = item.Username;
                        n.UserName = User.Identity.Name;
                        d.Notifications.Add(n);
                        d.SaveChanges();
                    }
                }
            }
        }
        private bool AlreadyExists(Like item)
        {
            using (var d = new ApplicationDbContext())
            {
                var exist = d.Notifications.Any(x => x.Post_ID == item.Post_ID
                            && x.UserName == User.Identity.Name && x.Name == item.Username
                            && x.Action == "Voted");
                return exist;
            }
        }

        public ActionResult DisableNotification(int id)
        {
            using (var d = new ApplicationDbContext())
            {
                List<Notifications> notify = new List<Notifications>();
                var not = d.Notifications.FirstOrDefault(x => x.ID == id);
                not.Flag = 1;
                d.SaveChanges();
                notify = d.Notifications.Where(x => x.UserName == User.Identity.Name).ToList();
                var dec = Convert.ToInt32(Session["Notification"]) - 1;
                Session["Notification"] = dec;
                return View("Notification", notify);
            }
        }

    }
}