using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//
using Change.Models;

namespace Change.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        [AdminAuthentication]
        public ActionResult Index()
        {
            return View();
        }

        //管理员登录
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string UserName, string Pwd)
        {
            if (ModelState.IsValid)
            {
                using (ChangeDBEntities db = new ChangeDBEntities())
                {
                    AdminUser admin = db.AdminUser.FirstOrDefault(t => t.UserName == UserName && t.Pwd == Pwd);
                    if (admin != null)
                    {
                        //验证登录
                        MyAuthentication.SetCookie(admin.UserName,admin.SuId.ToString(),"admin");
                        return RedirectToAction("Index");
                    }
                    ModelState.AddModelError("", "帐号或密码错误!");
                }
            }
            return View();
        }

        //退出
        public ActionResult Logout()
        {
            MyAuthentication.LogOut();//退出
            return RedirectToAction("Index");
        }
    }
}