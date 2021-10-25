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
    [AdminAuthentication]
    public class OrdersController : Controller
    {
        private ChangeDBEntities db = new ChangeDBEntities();

        // GET: Orders 后台管理订单 实现分页
        public ActionResult Index(int? id,int? states)
        {
            List<Orders> list;
            if (states==null)
            {
               list= db.Orders.Include(o => o.Delivery).Include(o => o.Users)
                    .OrderByDescending(o => o.Orderdate).ToList();
            }
            else
            {
                list = db.Orders.Where(o=>o.States==states).Include(o => o.Delivery).Include(o => o.Users)
                     .OrderByDescending(o => o.Orderdate).ToList();
            }
            int pageIndex = id ?? 1;

            PagedList<Orders> mPage = list.AsQueryable().ToPagedList(pageIndex, 5);
            return View(mPage);
        }

        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Orders orders = db.Orders.Find(id);
            if (orders == null)
            {
                return HttpNotFound();
            }
            return View(orders);
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            ViewBag.DeliveryID = new SelectList(db.Delivery, "DeliveryID", "Consignee");
            ViewBag.UserId = new SelectList(db.Users, "UserId", "UserName");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrdersID,Orderdate,UserId,Total,DeliveryID,DeliveryDate,States")] Orders orders)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(orders);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DeliveryID = new SelectList(db.Delivery, "DeliveryID", "Consignee", orders.DeliveryID);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "UserName", orders.UserId);
            return View(orders);
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Orders orders = db.Orders.Find(id);
            if (orders == null)
            {
                return HttpNotFound();
            }
            ViewBag.DeliveryID = new SelectList(db.Delivery, "DeliveryID", "Consignee", orders.DeliveryID);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "UserName", orders.UserId);
            return View(orders);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrdersID,Orderdate,UserId,Total,DeliveryID,DeliveryDate,States")] Orders orders)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orders).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DeliveryID = new SelectList(db.Delivery, "DeliveryID", "Consignee", orders.DeliveryID);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "UserName", orders.UserId);
            return View(orders);
        }

        // GET: Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Orders orders = db.Orders.Find(id);
            if (orders == null)
            {
                return HttpNotFound();
            }
            return View(orders);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Orders orders = db.Orders.Find(id);
            db.Orders.Remove(orders);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        // GET: /Orders/Send/5 订单发货
        public ActionResult Send(int id)
        {
            Orders order = db.Orders.Find(id);
            order.States = 2;
            db.Entry(order).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: /Orders/Close/5 关闭订单
        public ActionResult Close(int id)
        {
            Orders order = db.Orders.Find(id);
            order.States = 9;
            db.Entry(order).State = EntityState.Modified;
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
