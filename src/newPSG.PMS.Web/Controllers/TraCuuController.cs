using System.Web.Mvc;

namespace newPSG.PMS.Web.Controllers
{
    public class TraCuuController : PMSControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ThuTuc(string id)
        {
            ViewBag.Url = Request.Url.AbsoluteUri.ToLower();

            string tenThuTuc = id.ToLower();
            ViewBag.TenThuTucDirective = tenThuTuc;
            if (tenThuTuc == "tucongbo")
            {
                ViewBag.TenThuTuc = "Hồ sơ tự công bố";
            }
            if (tenThuTuc == "dangkycongbo")
            {
                ViewBag.TenThuTuc = "Hồ sơ đăng ký công bố";
            }
            if (tenThuTuc == "dangkyquangcao")
            {
                ViewBag.TenThuTuc = "Hồ sơ đăng ký quảng cáo";
            }
            if (tenThuTuc == "cosodudieukien")
            {
                ViewBag.TenThuTuc = "Hồ sơ cơ sở đủ điều kiện";
            }

            return View($"~/Views/TraCuu/Index.cshtml");
        }
    }
}