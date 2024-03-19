using System.Web;
using System.Web.Mvc;

namespace Đồ_án_cơ_sở
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
