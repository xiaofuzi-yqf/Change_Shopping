using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Change
{
    public class AdminAuthentication : AuthorizeAttribute
    {
        /// <summary>
        /// 视图响应前执行验证,查看当前用户是否有效
        /// </summary>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (MyAuthentication.IsLogin())
            {
                if (MyAuthentication.GetRights()!="admin")
                    HttpContext.Current.Response.Redirect("~/Admin/Login", true);
            }
            else
                HttpContext.Current.Response.Redirect("~/Admin/Login", true);
        }
    }

}