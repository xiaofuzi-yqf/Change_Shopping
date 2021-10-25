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
    public class OrdersDetailsController : Controller
    {
        private ChangeDBEntities db = new ChangeDBEntities();

        // GET: OrdersDetails
        public ActionResult Index()
        {
            var ordersDetail = db.OrdersDetail.Include(o => o.Orders).Include(o => o.Product);
            return View(ordersDetail.ToList());
        }

        // GET: OrdersDetails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrdersDetail ordersDetail = db.OrdersDetail.Find(id);
            if (ordersDetail == null)
            {
                return HttpNotFound();
            }
            return View(ordersDetail);
        }

        // GET: OrdersDetails/Create
        public ActionResult Create()
        {
            ViewBag.OrdersID = new SelectList(db.Orders, "OrdersID", "OrdersID");
            ViewBag.ProductId = new SelectList(db.Product, "ProductId", "Title");
            return View();
        }

        // POST: OrdersDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DetailID,OrdersID,ProductId,Quantity,States")] OrdersDetail ordersDetail)
        {
            if (ModelState.IsValid)
            {
                db.OrdersDetail.Add(ordersDetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.OrdersID = new SelectList(db.Orders, "OrdersID", "OrdersID", ordersDetail.OrdersID);
            ViewBag.ProductId = new SelectList(db.Product, "ProductId", "Title", ordersDetail.ProductId);
            return View(ordersDetail);
        }

        // GET: OrdersDetails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrdersDetail ordersDetail = db.OrdersDetail.Find(id);
            if (ordersDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.OrdersID = new SelectList(db.Orders, "OrdersID", "OrdersID", ordersDetail.OrdersID);
            ViewBag.ProductId = new SelectList(db.Product, "ProductId", "Title", ordersDetail.ProductId);
            return View(ordersDetail);
        }

        // POST: OrdersDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DetailID,OrdersID,ProductId,Quantity,States")] OrdersDetail ordersDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ordersDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OrdersID = new SelectList(db.Orders, "OrdersID", "OrdersID", ordersDetail.OrdersID);
            ViewBag.ProductId = new SelectList(db.Product, "ProductId", "Title", ordersDetail.ProductId);
            return View(ordersDetail);
        }

        // GET: OrdersDetails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrdersDetail ordersDetail = db.OrdersDetail.Find(id);
            if (ordersDetail == null)
            {
                return HttpNotFound();
            }
            return View(ordersDetail);
        }

        // POST: OrdersDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OrdersDetail ordersDetail = db.OrdersDetail.Find(id);
            db.OrdersDetail.Remove(ordersDetail);
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
