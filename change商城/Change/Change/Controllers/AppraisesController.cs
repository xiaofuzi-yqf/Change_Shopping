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
    [UserAuthentication]
    public class AppraisesController : Controller
    {
        private ChangeDBEntities db = new ChangeDBEntities();

        // GET: Appraises 我的评价 实现分页
        public ActionResult Index(int? id)
        {
            int uid = int.Parse(MyAuthentication.GetUserId());//获取登录用户ID
            //获取登录用户的评价
            List<Appraise> list = db.Appraise.Where(a=>a.UserId==uid).Include(a => a.Product).Include(a => a.Users)
               .OrderByDescending(a => a.RateTime).ToList();
            int pageIndex = id ?? 1;
            PagedList<Appraise> mPage = list.AsQueryable().ToPagedList(pageIndex, 5);
            return View(mPage);
        }

        // GET: Appraises/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appraise appraise = db.Appraise.Find(id);
            if (appraise == null)
            {
                return HttpNotFound();
            }
            return View(appraise);
        }

        // GET: Appraises/Create/5 对商品评价
        public ActionResult Create(int? id)
        {
            ViewBag.Product = db.Product.Find(id);
            ViewBag.UserId = MyAuthentication.GetUserId(); //获取当前登录用户ID
            return View();
        }

        // POST: Appraises/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AppraiseId,UserId,ProductId,Content,Grade,RateTime")] Appraise appraise)
        {
            if (ModelState.IsValid)
            {
                appraise.RateTime = DateTime.Now;//当前时间
                db.Appraise.Add(appraise);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProductId = new SelectList(db.Product, "ProductId", "Title", appraise.ProductId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "UserName", appraise.UserId);
            return View(appraise);
        }

        // GET: Appraises/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appraise appraise = db.Appraise.Find(id);
            if (appraise == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductId = new SelectList(db.Product, "ProductId", "Title", appraise.ProductId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "UserName", appraise.UserId);
            return View(appraise);
        }

        // POST: Appraises/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AppraiseId,UserId,ProductId,Content,Grade,RateTime")] Appraise appraise)
        {
            if (ModelState.IsValid)
            {
                db.Entry(appraise).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductId = new SelectList(db.Product, "ProductId", "Title", appraise.ProductId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "UserName", appraise.UserId);
            return View(appraise);
        }

        // GET: Appraises/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appraise appraise = db.Appraise.Find(id);
            if (appraise == null)
            {
                return HttpNotFound();
            }
            return View(appraise);
        }

        // POST: Appraises/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Appraise appraise = db.Appraise.Find(id);
            db.Appraise.Remove(appraise);
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
