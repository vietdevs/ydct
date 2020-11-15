using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace newPSG.PMS.Web.Routing
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();

            //ASP.NET Web API Route Config
            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );
            //Lienhe
            routes.MapRoute(
            name: "LienHe",
            url: "lien-he",
            defaults: new { controller = "LienHe", action = "Index" },
            namespaces: new[] { "newPSG.PMS.Web.Controllers" }
            );
            //ThongBao
            routes.MapRoute(
            name: "ThongBao",
            url: "thong-bao",
            defaults: new { controller = "ThongBao", action = "Index" },
            namespaces: new[] { "newPSG.PMS.Web.Controllers" }
            );
            //ThongBao
            routes.MapRoute(
            name: "ThongKe",
            url: "thong-ke",
            defaults: new { controller = "ThongKe", action = "Index" },
            namespaces: new[] { "newPSG.PMS.Web.Controllers" }
            );
            //thutuchanhchinh
            routes.MapRoute(
            name: "ThuTucHanhChinh",
            url: "thu-tuc-hanh-chinh",
            defaults: new { controller = "ThuTucHanhChinh", action = "Index" },
            namespaces: new[] { "newPSG.PMS.Web.Controllers" }
            );
            routes.MapRoute(
          name: "ThuTucHanhChinh_ChiTiet",
          url: "thu-tuc/{alias}",
          defaults: new { controller = "ThuTucHanhChinh", action = "Detail", alias = UrlParameter.Optional },
          namespaces: new[] { "newPSG.PMS.Web.Controllers" }
          );
            routes.MapRoute(
           name: "ThongBao_ChiTiet",
           url: "thong-bao/{alias}",
           defaults: new { controller = "ThongBao", action = "Detail", alias = UrlParameter.Optional },
           namespaces: new[] { "newPSG.PMS.Web.Controllers" }
           );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "newPSG.PMS.Web.Controllers" }
            );
        }
    }
}