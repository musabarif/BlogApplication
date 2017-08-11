using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using BlogWebsite.Models;
using System.Text.RegularExpressions;

namespace BlogWebsite.Controllers
{
    public class PostsController : Controller
    {
        const string PostAddModelKey = "_post_model";
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Posts
        public ActionResult Index()
        {
            List<Post> post = db.Posts.Include(x => x.Tags).ToList();
            foreach (var item in post)
            {
                item.Text = HttpUtility.HtmlDecode(item.Text);
                Regex.Replace(item.Text, @"\s+", " ");
                item.Text.Trim();
            }
            return View(post);
        }

        // GET: Posts/Details/5

        public ActionResult Details(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Where(x => x.ID == id).Include(x => x.Tags).FirstOrDefault();
            post.Text = HttpUtility.HtmlDecode(post.Text);

            post.Views = post.Views + 1;
            db.Entry(post).State = EntityState.Modified;
            db.SaveChanges();
            if (post == null)
            {
                return HttpNotFound();
            }

            Session["ReplyForm"] = "false";

            List<Comment> com = db.Comments.Where(x => x.PostID == id).ToList();

            ViewModel vm = new ViewModel { post = post, comment = com };
            return View("Details", vm);
        }

        // GET: Posts/Create

        public ActionResult Create()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            Post _model = null;
            if (TempData.ContainsKey(PostAddModelKey))
            {
                _model = TempData[PostAddModelKey] as Post;
            }
            else
            {
                _model = new Post();
            }
            //_model.VideoTypes = EnumOperations.ToList<Data.Video.Types>().Select(kV => new SelectListItem() { Value = kV.Value, Text = kV.Key }).ToList();
            return View(_model);

        }


        [HttpPost]
        public ActionResult AddTag(Post model)
        {
            if (model.Tags == null)
            {
                model.Tags = new List<Tag>();
            }

            model.Tags.Add(new Tag() { Name = "", ID = 0 });
            TempData[PostAddModelKey] = model;
            return RedirectToAction("Create");
        }

        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Post post)
        {
            if (ModelState.IsValid)
            {
                post.Text = HttpUtility.HtmlEncode(post.Text);

                db.Posts.Add(post);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            TempData[PostAddModelKey] = post;
            return RedirectToAction("Create");
        }

        // GET: Posts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Include(x => x.Tags).FirstOrDefault();
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Post post)
        {
            if (ModelState.IsValid)
            {
                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(post);
        }

        // GET: Posts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Post post = db.Posts.Include(x => x.Tags).FirstOrDefault();
            db.Posts.Remove(post);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //[HttpPost]
        public ActionResult Search(string sr)
        {
            using (var d = new ApplicationDbContext())
            {
                var _list = d.Posts.Where(p => p.Tags.Any(y => y.Name.Contains(sr))).Include(p => p.Tags).ToList();

                if (_list == null)
                    return View("Index", db.Posts.Include(x => x.Tags).ToList());
                else
                    return View("Index", _list);
            }
        }

        //public ActionResult Comment(string comment, int Id, string CommentTime)
        //{
        //    if (!Request.IsAuthenticated)
        //    {
        //        return RedirectToAction("Login", "Account");
        //    }
        //    using (var d = new ApplicationDbContext())
        //    {
        //        int def = 0;
        //        d.Database.ExecuteSqlCommand("Insert Into Comments Values('" + Session["name"] + "','" + comment + "','" + Id + "','" + def + "','" + CommentTime + "')");
        //        Post p = db.Posts.Where(x => x.ID == Id).Include(x => x.Tags).FirstOrDefault();
        //        List<Comment> com = db.Comments.Where(x => x.PostID == Id).ToList();
        //        ViewModel vm = new ViewModel { post = p, comment = com };
        //        return View("Details", vm);
        //    }
        //}

        public ActionResult Comment(string comment, int Id, string CommentTime, int ParentID = 0)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            using (var d = new ApplicationDbContext())
            {
                d.Database.ExecuteSqlCommand("Insert Into Comments Values('" + Session["name"] + "','" + comment + "','" + Id + "','" + ParentID + "','" + CommentTime + "')");
                Post p = db.Posts.Where(x => x.ID == Id).Include(x => x.Tags).FirstOrDefault();
                List<Comment> com = db.Comments.Where(x => x.PostID == Id).ToList();
                ViewModel vm = new ViewModel { post = p, comment = com };
                return View("Details", vm);
            }
        }

        public PartialViewResult Like(int id, string vote)
        {

            using (var d = new ApplicationDbContext())
            {
                Post p = d.Posts.Where(x => x.ID == id).Include(x => x.Tags).FirstOrDefault();
                Like like = new Like();
                var ldetails = d.Like.FirstOrDefault(x => x.Username == User.Identity.Name);
                if (ldetails == null)
                {
                    like.Post_ID = id;
                    like.Username = User.Identity.Name;
                    if (vote == "Upvote")
                    {
                        p.Votes += 1;
                        like.Vote = 1;
                    }
                    else
                    {
                        p.Votes -= 1;
                        like.Vote = -1;
                    }
                    d.Like.Add(like);
                    d.SaveChanges();
                }
                else
                {
                    if (vote == "Upvote" && ldetails.Vote == -1)
                    {
                        p.Votes += 1;
                        ldetails.Vote += 1;
                        ldetails.Flag = -1;
                    }
                    if (vote == "Downvote" && ldetails.Vote == 1)
                    {
                        p.Votes -= 1;
                        ldetails.Vote -= 1;
                        ldetails.Flag = 1;
                    }
                    if (vote == "Upvote" && ldetails.Vote == 0 && ldetails.Flag == 1)
                    {
                        p.Votes += 1;
                        ldetails.Vote += 1;
                        ldetails.Flag = 0;
                    }
                    if (vote == "Downvote" && ldetails.Vote == 0 && ldetails.Flag == -1)
                    {
                        p.Votes -= 1;
                        ldetails.Vote -= 1;
                        ldetails.Flag = 0;
                    }
                    d.SaveChanges();
                }
                d.SaveChanges();
                ViewBag.vote = p.Votes;
                return PartialView();
            }
        }

        public PartialViewResult ReplyForm(int ID, int PostID)
        {
            Comment comment = new Comment();
            comment.ID = ID;
            comment.PostID = PostID;
            if (Session["ReplyForm"].ToString() == "false")
                Session["ReplyForm"] = "true";
            else
                Session["ReplyForm"] = "false";

            return PartialView(comment);
        }

    }
}
