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
    public class DeliveriesController : Controller
    {
        private ChangeDBEntities db = new ChangeDBEntities();

        // GET: Deliveries
        public ActionResult Index()
        {
            var delivery = db.Delivery.Include(d => d.Users);
            return View(delivery.ToList());
        }

        // GET: Deliveries/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Delivery delivery = db.Delivery.Find(id);
            if (delivery == null)
            {
                return HttpNotFound();
            }
            return View(delivery);
        }

        // GET: Deliveries/Create 为当前登录用户添加收货地址
        public ActionResult Create()
        {
            //获取用户ID
            int uid= int.Parse(MyAuthentication.GetUserId()); //获取当前登录用户ID
            ViewBag.User = db.Users.Find(uid);
            ViewBag.Deliverys=db.Delivery.Where(d=>d.UserId==uid);
            return View();
        }

        // POST: Deliveries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DeliveryID,UserId,Consignee,Complete,Phone")] Delivery delivery)
        {
            if (ModelState.IsValid)
            {
                db.Delivery.Add(delivery);
                db.SaveChanges();
                //return RedirectToAction("Index");
                return RedirectToAction("Create", new { id = delivery.UserId });//返回地址管理页面
            }
            ViewBag.UserId = new SelectList(db.Users, "UserId", "UserName", delivery.UserId);
            return View(delivery);
        }

        // GET: Deliveries/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Delivery delivery = db.Delivery.Find(id);
            if (delivery == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.Users, "UserId", "UserName", delivery.UserId);
            return View(delivery);
        }

        // POST: Deliveries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DeliveryID,UserId,Consignee,Complete,Phone")] Delivery delivery)
        {
            if (ModelState.IsValid)
            {
                db.Entry(delivery).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.Users, "UserId", "UserName", delivery.UserId);
            return View(delivery);
        }

        // GET: Deliveries/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Delivery delivery = db.Delivery.Find(id);
            //直接删除
            db.Delivery.Remove(delivery);
            db.SaveChanges();
            return RedirectToAction("Create", new { id = delivery.UserId });//返回地址管理页面
            //if (delivery == null)
            //{
            //    return HttpNotFound();
            //}
            //return View(delivery);
        }

        // GET: Deliveries/SetDefault/5
        public ActionResult SetDefault(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Delivery delivery = db.Delivery.Find(id);
            //设为默认
            int uid = int.Parse(MyAuthentication.GetUserId());
            Users user = db.Users.Find(uid);
            user.DeliveryID = id;
            db.SaveChanges();
            return RedirectToAction("Create", new { id = delivery.UserId });//返回地址管理页面
            //if (delivery == null)
            //{
            //    return HttpNotFound();
            //}
            //return View(delivery);
        }

        // POST: Deliveries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Delivery delivery = db.Delivery.Find(id);
            db.Delivery.Remove(delivery);
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
