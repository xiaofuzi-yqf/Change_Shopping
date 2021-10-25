using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//EF模型命名空间
using Change.Models;
//分页命名空间
using Webdiyer.WebControls.Mvc;

namespace Change.Controllers
{
    public class HomeController : Controller
    {
        private ChangeDBEntities db = new ChangeDBEntities();
        //首页
        public ActionResult Index()
        {
            //获取所有一级分类
            ViewBag.Categories = db.Category.Include("Product").Where(c => c.ParentId == null).OrderBy(c => c.CateId).ToList();

            //获取置顶资讯信息
            ViewBag.News = db.News.Where(n => n.States == 0).OrderBy(n => n.PushTime).ToList();

            //获取1F5个服装商品分类列表
            List<Category> cates = db.Category.Where(c => c.ParentId == 1).ToList();
            int[] cateids = new int[cates.Count];
            for (int i = 0; i < cateids.Length; i++)
            {
                cateids[i] = cates[i].CateId;
            }
            //获取1F5个服装商品列表
            var products1 = from p in db.Product where (cateids).Contains(p.CateId) select p;
            ViewBag.Products1= products1.Take(5).ToList();

            //获取2F5个电器商品分类列表
            cates = db.Category.Where(c => c.ParentId == 2).ToList();
            cateids = new int[cates.Count];
            for (int i = 0; i < cateids.Length; i++)
            {
                cateids[i] = cates[i].CateId;
            }
            //获取2F5个电器商品列表
            var products2 = from p in db.Product where (cateids).Contains(p.CateId) select p;
            ViewBag.Products2 = products2.Take(5).ToList();

            //获取3F5个美食分类列表
            cates = db.Category.Where(c => c.ParentId == 5).ToList();
            cateids = new int[cates.Count];
            for (int i = 0; i < cateids.Length; i++)
            {
                cateids[i] = cates[i].CateId;
            }
            //获取3F5个美食商品列表
            var products3 = from p in db.Product where (cateids).Contains(p.CateId) select p;
            ViewBag.Products3 = products2.Take(5).ToList();

            return View();
        }
        //搜索
        public ActionResult Search(string searchText)
        {
            ViewBag.search = searchText;
            //模糊查询
            var products = from product in db.Product
                         where product.Title.Contains(searchText)
                         select product;
            return View(products);
        }

        //资讯分类查询
        public ActionResult News(string type)
        {
            ViewBag.type = type;
            return View(db.News.Where(n => n.NTypes==type).OrderBy(n=>n.States).OrderByDescending(n=>n.PushTime).ToList());
        }
        //商品分类查询
        public ActionResult Category(int? id)
        {
            int cid = id ?? 1;
            ViewBag.category = db.Category.Find(id);
            return View(db.Product.Where(p=>p.CateId==cid).ToList());
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        //用户登录
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
                    Users user = db.Users.FirstOrDefault(t => t.UserName == UserName && t.Pwd == Pwd);
                    if (user != null)
                    {
                        //验证登录
                        MyAuthentication.SetCookie(user.UserName, user.UserId.ToString(), "user");
                        return RedirectToAction("Index");
                    }
                    ModelState.AddModelError("", "帐号或密码错误!");
                }
            }
            return View();
        }
        //用户注册
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(Users user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (ChangeDBEntities db = new ChangeDBEntities())
                    {
                        //注册
                        db.Users.Add(user);
                        db.SaveChanges();
                        ViewBag.msg = "注册成功,请输入账号和密码进行登录!!";
                        return RedirectToAction("Login");
                    }
                }
                catch (Exception exp)
                {
                    ModelState.AddModelError("", "注册失败！ "+exp.Message);
                }

            }
            return View();
        }
        //退出
        public ActionResult Logout()
        {
            MyAuthentication.LogOut();//退出登录
            return RedirectToAction("Index");
        }
    }
}