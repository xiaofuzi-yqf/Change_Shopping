using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Change.Models;
//分页命名空间
using Webdiyer.WebControls.Mvc;
//
using System.Transactions;

namespace Change.Controllers
{
    [UserAuthentication]
    public class CarController : Controller
    {
        private ChangeDBEntities db = new ChangeDBEntities();
        // GET: /Car/Index 购物车
        public ActionResult Index()
        {
            //获取购物车:临时订单
            Orders car = GetCar();
            car.Total = SumTotal();//计算总价
                                   // ViewBag.Total = SumTotal();//计算总价
            return View(car);
        }

        // GET: /Car/Order 结算订单
        public ActionResult Order()
        {
            //获取购物车:临时订单
            Orders car = GetCar();
            //至少选中一个
            if (car.OrdersDetail.Count == 0)
            {
                TempData["msg"] = "请至少选中一个商品!!!";
            }
            else
            {
                int uid = int.Parse(MyAuthentication.GetUserId());
                Users user = db.Users.Include("Delivery").First(u => u.UserId ==uid );
                car.UserId = user.UserId;//当前用户ID
                car.Users = user;
                car.DeliveryID = user.DeliveryID;//默认收货ID
                car.Orderdate = DateTime.Now.AddHours(2);//默认为当前时间+2小时
                car.Total = SumTotal();//计算总价
                ViewBag.Total = SumTotal();//计算总价
            }
            return View(car);
        }
        [HttpPost]
        public ActionResult Order(Orders o)
        {
            Orders car = GetCar();
            //使用事务生成订单
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    //新增订单
                    o.Orderdate = DateTime.Now;
                    o.States = 0;
                    o.UserId = car.UserId;
                    o.OrdersDetail = car.OrdersDetail;
                    o.Total = car.Total;
                    //依次清除订单详情中的产品对象，并减少对应商品的库存
                    List<OrdersDetail> list = o.OrdersDetail as List<OrdersDetail>;
                    for (int i = 0; i < list.Count; i++)
                    {
                        Product p = db.Product.Find(list[i].ProductId);
                        p.Stock -= list[i].Quantity;//库存减少       
                        db.Entry(p).State = EntityState.Modified;
                        db.SaveChanges();
                        list[i].Product = null;
                    }
                    db.Orders.Add(o);//新增订单记录,同时自动新增订单详情
                    db.SaveChanges();
                    scope.Complete(); //提交事务
                    TempData["msg"] = "生成订单成功!!";
                    //清空购物车
                    ClearCar();
                    return RedirectToAction("OrderPay", new { id = o.OrdersID });
                }
                catch (Exception exp)
                {
                    TempData["msg"] = "生成订单失败!" + exp.Message;
                }
            }

            return View(car);
        }

        // GET: /Car/OrderPay/5 支付订单
        public ActionResult OrderPay(int id)
        {
            return View(db.Orders.Find(id));
        }
        [HttpPost]
        public ActionResult OrderPay(Orders o)
        {
            Orders order = db.Orders.Find(o.OrdersID);
            order.States = 1;
            db.Entry(order).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("MyOrders","MyUser");
        }


        // GET: /Car/Add/X 新增一个商品到购物车
        public ActionResult Add(int id, int quantity)
        {
            //添加1
            AddCar(id, quantity);
            return RedirectToAction("Index");
        }

        // GET: /Car/Del/X 从购物车减少一个商品
        public ActionResult Del(int id)
        {
            //减少1
            DelCar(id);
            return RedirectToAction("Index");
        }

        // GET: /Car/Remove/X 移除商品
        public ActionResult Remove(int id)
        {
            //减少1
            RemoveCar(id);
            return RedirectToAction("Index");
        }

        // GET: /Car/Clear 清空购物车
        public ActionResult Clear()
        {
            ClearCar();
            return RedirectToAction("Index");
        }

        #region//购物车相关方法
        private Orders GetCar()
        {
            Orders car;
            if (Session["car"] == null)
            {
                car = new Orders();
                car.OrdersDetail = new List<OrdersDetail>();
                //保存购物车(临时订单)用户信息
                Session["car"] = car;
            }
            else
            {
                car = (Orders)Session["car"];
            }
            return car;
        }
        //添加n个商品到购物车
        private void AddCar(int pid, int quantity)
        {
            Orders car = GetCar();
            OrdersDetail d = car.OrdersDetail.FirstOrDefault(p => p.ProductId == pid);
            if (d != null)
            {
                d.Quantity += quantity;
            }
            else
            {
                Product p = db.Product.Find(pid);
                d = new OrdersDetail();
                d.ProductId = pid;
                d.Product = p;
                d.Quantity = 1;
                car.OrdersDetail.Add(d);
            }
            Session["car"] = car;
        }
        //移除某商品
        private void RemoveCar(int pid)
        {
            Orders car = GetCar();
            OrdersDetail d = car.OrdersDetail.FirstOrDefault(p => p.ProductId == pid);
            if (d != null)
            {
                car.OrdersDetail.Remove(d);
                Session["car"] = car;
            }
        }
        //减少1个商品
        private void DelCar(int pid)
        {
            Orders car = GetCar();
            OrdersDetail d = car.OrdersDetail.FirstOrDefault(p => p.ProductId == pid);
            if (d != null && d.Quantity > 1)
            {
                d.Quantity -= 1;
                Session["car"] = car;
            }
        }
        //清空购物车
        private void ClearCar()
        {
            Orders car = GetCar();
            car.OrdersDetail.Clear();
            Session["car"] = car;
        }
        //计算总金额
        private decimal SumTotal()
        {
            Orders car = GetCar();
            decimal sum = 0;
            foreach (OrdersDetail item in car.OrdersDetail)
            {
                sum += item.Quantity * item.Product.Price;
            }
            return sum;
        }
        #endregion
    }
}