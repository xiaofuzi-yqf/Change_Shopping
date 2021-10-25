using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Change
{
    public class MyAuthentication
    {
        /// <summary>
        /// 设置用户登陆成功凭据（Cookie存储）
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <param name="UserID">用户编号</param>
        /// <param name="Rights">权限</param>
        public static void SetCookie(string UserName, string UserID, string Rights)
        {
            String UserData = UserID + "#" + Rights;
            //数据放入ticket
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, UserName, DateTime.Now, DateTime.Now.AddMinutes(60), false, UserData);
            //数据加密
            string enyTicket = FormsAuthentication.Encrypt(ticket);
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, enyTicket);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }
        /// <summary>
        /// 判断用户是否登陆
        /// </summary>
        /// <returns>True,Fales</returns>
        public static bool IsLogin()
        {
            return HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName] != null;
        }
        /// <summary>
        /// 注销登陆
        /// </summary>
        public static void LogOut()
        {
            FormsAuthentication.SignOut();
        }
        /// <summary>
        /// 获取凭据中的用户名
        /// </summary>
        /// <returns>用户名</returns>
        public static string GetUserName()
        {
            if (IsLogin())
            {
                HttpCookie authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);//解密 
                return authTicket.Name;
            }
            else
                return "";
        }
        /// <summary>
        /// 获取凭据中的用户ID
        /// </summary>
        /// <returns>用户ID</returns>
        public static string GetUserId()
        {
            if (IsLogin())
            {
                // string strUserData = ((FormsIdentity)(HttpContext.Current.User.Identity)).Ticket.UserData;
                HttpCookie authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);//解密 
                string[] UserData = authTicket.UserData.Split('#');
                if (UserData.Length > 0)
                    return UserData[0].ToString();
                else return "";
            }
            else return "";
        }
        /// <summary>
        /// 获取凭据中的用户权限
        /// </summary>
        /// <returns>用户权限</returns>
        public static string GetRights()
        {
            if (IsLogin())
            {
                // string strUserData = ((FormsIdentity)(HttpContext.Current.User.Identity)).Ticket.UserData;
                HttpCookie authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);//解密 
                string[] UserData = authTicket.UserData.Split('#');
                if (UserData.Length >1)
                    return UserData[1].ToString();
                else return "";
            }
            else return "";
        }
    }
}