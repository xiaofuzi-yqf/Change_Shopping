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
    public class ProductsController : Controller
    {
        private ChangeDBEntities db = new ChangeDBEntities();

        // GET: Products 实现分页
        [AdminAuthentication]
        public ActionResult Index(int? id = 1)
        {
            List<Product> list = db.Product.Include("Photo").Include(p => p.Category).OrderByDescending(p => p.PostTime).ToList();
            int pageIndex = id ?? 1;
            PagedList<Product> mPage = list.AsQueryable().ToPagedList(pageIndex, 5);
            return View(mPage);
        }

        // GET: Products/Details/5 商品详情
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Product.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            //统计总销量和总评价
            List<OrdersDetail> details = db.OrdersDetail.Where(d => d.ProductId == product.ProductId).ToList();
            int totalSale = 0;
            foreach (OrdersDetail item in details)
            {
                totalSale += item.Quantity;
            }
            ViewBag.TotalSale = totalSale;
            ViewBag.TotalRate = db.Appraise.Where(a => a.ProductId == product.ProductId).Count();
            return View(product);
        }

        // GET: Products/Details/5 商品评价详情 grade(0-好评1-中评2-差评) ，sort(1-按时间)
        public ActionResult DetailsRate(int? id, int? grade, int? sort)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Product.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            //统计总销量
            List<OrdersDetail> details = db.OrdersDetail.Where(d => d.ProductId == product.ProductId).ToList();
            int totalSale = 0;
            foreach (OrdersDetail item in details)
            {
                totalSale += item.Quantity;
            }
            ViewBag.TotalSale = totalSale;
            //统计总评价
            ViewBag.TotalRate = db.Appraise.Where(a => a.ProductId == product.ProductId).Count();
            //按条件获取评价信息
            List<Appraise> rates = grade == null ? db.Appraise.ToList() : db.Appraise.Where(a => a.Grade == grade).ToList();
            ViewBag.RateList = sort == 1 ? rates.OrderByDescending(a => a.UserId).ToList() : rates;
            return View(product);
        }

        // GET: Products/Create
        [AdminAuthentication]
        public ActionResult Create()
        {
            ViewBag.CateId = new SelectList(db.Category, "CateId", "CateName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AdminAuthentication]
        public ActionResult Create([Bind(Include = "ProductId,Title,CateId,MarketPrice,Price,Content,PostTime,Stock")] Product product)
        {
            if (ModelState.IsValid)
            {
                product.PostTime = DateTime.Now;//上架时间为当前时间
                db.Product.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CateId = new SelectList(db.Category, "CateId", "CateName", product.CateId);
            return View(product);
        }

        // GET: Products/Edit/5
        [AdminAuthentication]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Product.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CateId = new SelectList(db.Category, "CateId", "CateName", product.CateId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AdminAuthentication]
        public ActionResult Edit([Bind(Include = "ProductId,Title,CateId,MarketPrice,Price,Content,PostTime,Stock")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CateId = new SelectList(db.Category, "CateId", "CateName", product.CateId);
            return View(product);
        }

        // GET: Products/Delete/5
        [AdminAuthentication]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Product.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AdminAuthentication]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Product.Find(id);
            db.Product.Remove(product);
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
