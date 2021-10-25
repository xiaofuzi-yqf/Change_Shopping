using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Change.Models;

namespace Change.Controllers
{
    [UserAuthentication]
    public class FavoritesController : Controller
    {
        private ChangeDBEntities db = new ChangeDBEntities();

        // GET: Favorites
        public ActionResult Index()
        {
            int myid = int.Parse(MyAuthentication.GetUserId());
            var favorite = db.Favorite.Include(f => f.Product).Include(f => f.Users)
                    .Where(f=>f.UserId== myid);
            return View(favorite.ToList());
        }

        // GET: Favorites/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Favorite favorite = db.Favorite.Find(id);
            if (favorite == null)
            {
                return HttpNotFound();
            }
            return View(favorite);
        }

        // GET: Favorites/Create/5 为当前用户添加收藏商品
        public ActionResult Create(int id)
        {
            Favorite favorite = new Favorite();
            favorite.ProductId = id;
            favorite.UserId = int.Parse(MyAuthentication.GetUserId());//当前用户ID
            db.Favorite.Add(favorite);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: Favorites/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FavoriteID,ProductId,UserId")] Favorite favorite)
        {
            if (ModelState.IsValid)
            {
                db.Favorite.Add(favorite);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProductId = new SelectList(db.Product, "ProductId", "Title", favorite.ProductId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "UserName", favorite.UserId);
            return View(favorite);
        }

        // GET: Favorites/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Favorite favorite = db.Favorite.Find(id);
            if (favorite == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductId = new SelectList(db.Product, "ProductId", "Title", favorite.ProductId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "UserName", favorite.UserId);
            return View(favorite);
        }

        // POST: Favorites/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FavoriteID,ProductId,UserId")] Favorite favorite)
        {
            if (ModelState.IsValid)
            {
                db.Entry(favorite).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductId = new SelectList(db.Product, "ProductId", "Title", favorite.ProductId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "UserName", favorite.UserId);
            return View(favorite);
        }

        // GET: Favorites/Delete/5 直接删除
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Favorite favorite = db.Favorite.Find(id);
            if (favorite == null)
            {
                return HttpNotFound();
            }
            //直接删除
            db.Favorite.Remove(favorite);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: Favorites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Favorite favorite = db.Favorite.Find(id);
            db.Favorite.Remove(favorite);
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
