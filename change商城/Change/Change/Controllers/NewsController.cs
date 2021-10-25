using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Change.Models;
//分页命名空间
using Webdiyer.WebControls.Mvc;

namespace Change.Controllers
{
   
    public class NewsController : Controller
    {
        private ChangeDBEntities db = new ChangeDBEntities();

        // GET: News 实现分页
        [AdminAuthentication]
        public ActionResult Index(int? id)
        {
            List<News> list = db.News.OrderBy(p => p.States).OrderByDescending(p => p.PushTime).ToList();
            int pageIndex = id ?? 1;
            PagedList<News> mPage = list.AsQueryable().ToPagedList(pageIndex, 5);
            return View(mPage);
        }

        // GET: News/Details/5
        [AllowAnonymous]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        // GET: News/Create
        [AdminAuthentication]
        public ActionResult Create()
        {
            return View();
        }

        // POST: News/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AdminAuthentication]
        public ActionResult Create([Bind(Include = "NewsID,NTypes,Content,PhotoUrl,PushTime,States,Title")] News news, HttpPostedFileBase PhotoUrl)
        {
            if (ModelState.IsValid)
            {
                //上传图片
                if (PhotoUrl != null)
                {
                    news.PhotoUrl = "/images/" + PhotoUrl.FileName;
                    PhotoUrl.SaveAs(Server.MapPath(news.PhotoUrl));
                }
                news.PushTime = DateTime.Now;
                news.States = 1; //正常
                db.News.Add(news);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(news);
        }

        // GET: News/Edit/5
        [AdminAuthentication]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        // POST: News/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AdminAuthentication]
        public ActionResult Edit([Bind(Include = "NewsID,NTypes,Content,PhotoUrl,PushTime,States,Title")] News news)
        {
            if (ModelState.IsValid)
            {
                db.Entry(news).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(news);
        }
        [AdminAuthentication]
        public ActionResult SetTop(int id, int states)
        {
            News news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            news.States = states;
            db.Entry(news).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: News/Delete/5
        [AdminAuthentication]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        // POST: News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AdminAuthentication]
        public ActionResult DeleteConfirmed(int id)
        {
            News news = db.News.Find(id);
            db.News.Remove(news);
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
    }
}
