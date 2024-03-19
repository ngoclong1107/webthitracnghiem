using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Đồ_án_cơ_sở.Controllers
{
    public class KtGVController : Controller
    {
        // GET: KtGV
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["GiaoVien"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { Controller = "DangNhap", Action = "DangNhap" }));
            }
            base.OnActionExecuting(filterContext);
        }
    }
}