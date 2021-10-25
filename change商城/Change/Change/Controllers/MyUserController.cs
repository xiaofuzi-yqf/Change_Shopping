using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Net;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Change.Models;
//分页命名空间
using Webdiyer.WebControls.Mvc;

namespace Change.Controllers
{
    [UserAuthentication]
    public class MyUserController : Controller
    {
        private ChangeDBEntities db = new ChangeDBEntities();
        // GET: MyUser
        public ActionResult Index()
        {
            if (MyAuthentication.IsLogin())
            {
                int myid = int.Parse(MyAuthentication.GetUserId());
                ViewBag.nopay = db.Orders.Include(o => o.Delivery).Include(o => o.Users)
                    .Where(o => o.UserId == myid && o.States == 0).ToList();//未付款
                ViewBag.nosend = db.Orders.Include(o => o.Delivery).Include(o => o.Users)
                    .Where(o => o.UserId == myid && o.States == 1).ToList();//未发货
                ViewBag.noconfirm = db.Orders.Include(o => o.Delivery).Include(o => o.Users)
                    .Where(o => o.UserId == myid && o.States == 2).ToList();//未收货
            }
            return View();
        }

        // GET: MyUser/MyInfo/5 前台我的资料
        public ActionResult MyInfo()
        {
            int id = int.Parse(MyAuthentication.GetUserId());
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MyInfo([Bind(Include = "UserId,UserName,Pwd,Email,Nick,Picture,DeliveryID")] Users users)
        {
            if (ModelState.IsValid)
            {
                db.Entry(users).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(users);
        }

        // GET: MyUser/ChangePwd/5 前台修改密码
        public ActionResult ChangePwd()
        {
            int id = int.Parse(MyAuthentication.GetUserId());
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // POST: Users/ChangePwd/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePwd(int UserId, string OldPwd, string NewPwd, string NewPwd2)
        {
            Users user = db.Users.Find(UserId);
            if (ModelState.IsValid)
            {
                if (OldPwd != user.Pwd)
                {
                    ModelState.AddModelError("", "原密码错误！");
                }
                else if (NewPwd != NewPwd2)
                {
                    ModelState.AddModelError("", "密码不一致！");
                }
                else
                {
                    user.Pwd = NewPwd;
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                    ModelState.AddModelError("", "密码修改成功！");
                }
            }
            return View(user);
        }

        // GET: /MyUser/Confirm 确认收货
        public ActionResult Confirm(int id)
        {
            Orders order = db.Orders.Find(id);
            order.States = 3;
            db.Entry(order).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("MyOrders");
        }

        // GET: /MyUser/MyOrders 我的订单 实现分页
        public ActionResult MyOrders(int? id, int? states)
        {
            List<Orders> list;
            int uid = int.Parse(MyAuthentication.GetUserId());

            if (states == null)
            {
                list = db.Orders.Include(o => o.Delivery).Include(o => o.Users)
                    .Where(o => o.UserId == uid).ToList();
            }
            else
            {
                list = db.Orders.Where(o => o.States == states).Include(o => o.Delivery).Include(o => o.Users).Where(o => o.UserId == uid).ToList();
            }
            int pageIndex = id ?? 1;
            PagedList<Orders> mPage = list.AsQueryable().ToPagedList(pageIndex, 5);
            return View(mPage);
        }

    }
}