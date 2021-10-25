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
    [AdminAuthentication]
    public class PhotosController : Controller
    {
        private ChangeDBEntities db = new ChangeDBEntities();

        // GET: Photos
        public ActionResult Index()
        {
            var photo = db.Photo.Include(p => p.Product);
            return View(photo.ToList());
        }

        // GET: Photos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Photo photo = db.Photo.Find(id);
            if (photo == null)
            {
                return HttpNotFound();
            }
            return View(photo);
        }

        // GET: Photos/Create 为某商品添加图片信息
        public ActionResult Create(int id)
        {
            //获取商品ID
            ViewBag.ProductId = id;
            ViewBag.Product = db.Product.Include("Photo").First(p => p.ProductId == id);
            return View();
        }

        // POST: Photos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PhotoId,ProductId,PhotoUrl")] Photo photo, HttpPostedFileBase PhotoUrl)
        {
            if (ModelState.IsValid)
            {
                //上传图片
                if (PhotoUrl != null)
                {
                    photo.PhotoUrl = "/images/Goods/" + PhotoUrl.FileName;
                    PhotoUrl.SaveAs(Server.MapPath(photo.PhotoUrl));
                }
                db.Photo.Add(photo);
                db.SaveChanges();
                return RedirectToAction("Create", new { id = photo.ProductId });//返回图片管理页面
            }

            ViewBag.ProductId = photo.ProductId;
            ViewBag.Product = db.Product.First(p => p.ProductId == photo.ProductId);
            return View(photo);
        }

        // GET: Photos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Photo photo = db.Photo.Find(id);
            if (photo == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductId = new SelectList(db.Product, "ProductId", "Title", photo.ProductId);
            return View(photo);
        }

        // POST: Photos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PhotoId,ProductId,PhotoUrl")] Photo photo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(photo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductId = new SelectList(db.Product, "ProductId", "Title", photo.ProductId);
            return View(photo);
        }

        // GET: Photos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Photo photo = db.Photo.Find(id);
            if (photo == null)
            {
                return HttpNotFound();
            }
            return View(photo);
        }

        // POST: Photos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Photo photo = db.Photo.Find(id);
            db.Photo.Remove(photo);
            db.SaveChanges();
            return RedirectToAction("Create", new { id = photo.ProductId });//返回图片管理页面
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
