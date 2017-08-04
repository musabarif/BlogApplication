using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BlogWebsite.Models;

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
                return RedirectToAction("Index","Posts");
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
            Membership.CreateUser(model.Username,model.Password,model.Email);
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
            RegisterModel r = new RegisterModel();
            r.Email = user.Email;
            r.Username = user.UserName;

            if (User.Identity.Name == name)
                return View(r);
            else
                return View("Display", r);
            
        }

        [HttpPost]
        public ActionResult ShowProfile(RegisterModel model)
        {
            var user = Membership.GetUser(User.Identity.Name);
            user.Email = model.Email;

            Membership.UpdateUser(user);
            return RedirectToAction("Index", "Posts");

        }

    }
}