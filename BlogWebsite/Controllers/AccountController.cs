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
            if (Membership.ValidateUser(model.Username, model.password))
            {
                CreateAuthenticationToken(model);
                return RedirectToAction("Index","Posts");
            }
            return RedirectToAction("Login");
        }

        [NonAction]
        private void CreateAuthenticationToken(LoginModel model)
        {
            string userData = string.Join("|", model.Username, DateTime.Now);

            var ticket =new  FormsAuthenticationTicket(
                 1,                                     // ticket version
              model.Username,                              // authenticated username
              DateTime.Now,                          // issueDate
              DateTime.Now.AddMinutes(30),           // expiryDate
              model.RememberMe,                          // true to persist across browser sessions
              userData,                              // can be used to store additional user data
              System.Web.Security.FormsAuthentication.FormsCookiePath);

            string encryptedTicket = System.Web.Security.FormsAuthentication.Encrypt(ticket);

            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName,encryptedTicket);

            cookie.HttpOnly = true;
            Response.Cookies.Add(cookie);

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

    }
}