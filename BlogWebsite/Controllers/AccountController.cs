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

        //[NonAction]
        //private void CreateAuthenticationToken(LoginModel model)
        //{
        //    string userData = string.Join("|", model.Username, DateTime.Now);

        //    var ticket =new  FormsAuthenticationTicket(
        //         1,                                     // ticket version
        //      model.Username,                              // authenticated username
        //      DateTime.Now,                          // issueDate
        //      DateTime.Now.AddMinutes(30),           // expiryDate
        //      model.RememberMe,                          // true to persist across browser sessions
        //      userData,                              // can be used to store additional user data
        //      System.Web.Security.FormsAuthentication.FormsCookiePath);

        //    string encryptedTicket = System.Web.Security.FormsAuthentication.Encrypt(ticket);

        //    HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName,encryptedTicket);

        //    cookie.HttpOnly = true;
        //    Response.Cookies.Add(cookie);

        //}

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

        public ActionResult ShowProfile()
        {
            MembershipUser m=Membership.GetUser(Session["name"].ToString());
            ProfileModel user = new ProfileModel();
            user.User = m;
            return View(user);
        }

        [HttpPost]
        public ActionResult ShowProfile(MembershipUser user)
        {
            Membership.UpdateUser(user);
            return RedirectToAction("Index", "Posts");
            
        }

    }
}