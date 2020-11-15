using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Caching;
using newPSG.PMS.EntityDB;
using System.Web.Mvc;

namespace newPSG.PMS.Web.Controllers
{
    public class LienHeController : PMSControllerBase
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<LienHe> _lienHeRepos;
        private readonly ICacheManager _cacheManager;

        public LienHeController(IUnitOfWorkManager unitOfWorkManager,
            IRepository<LienHe> lienHeRepos,
            ICacheManager cacheManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _lienHeRepos = lienHeRepos;
            _cacheManager = cacheManager;
        }

        public ActionResult Index()
        {
            var url = Request.Url.AbsoluteUri.ToLower();
            return View();
        }

        public PartialViewResult _LienHe()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult _LienHe([Bind(Exclude = "")]LienHe model)
        {
            var respone = new { mess = 0 }; //mặc định là gửi thất bại

            if (ModelState.IsValid)
            {
                //if (CaptchaText == HttpContext.Session["captchastring"].ToString())
                //{
                _lienHeRepos.Insert(model);
                //gửi email
                string template = "<h2 stype='text-align: center; font-size: 16px; color: #3c763d; padding: 10px 0px; background: #dff0d8'>Khách hàng liên hệ</h2>";
                template += "<table style= 'width: 100%; margin-top: 15px;'>";
                template += "<tr><td style='width: 150px; border-color: #bce8f1;  background-color: #d9edf7; color: #31708f;  padding: 15px;'>Họ và tên: </td><td> " + model.HoTen + "</td></tr>";
                template += "<tr><td style='width: 150px; border-color: #bce8f1;  background-color: #d9edf7; color: #31708f;  padding: 15px;'>Email: </td><td> " + model.Email + "</td></tr>";
                template += "<tr><td style='width: 150px; border-color: #bce8f1;  background-color: #d9edf7; color: #31708f;  padding: 15px;'>Điện thoại: </td><td> " + model.DienThoai + "</td></tr>";
                template += "<tr><td style='width: 150px; border-color: #bce8f1;  background-color: #d9edf7; color: #31708f;  padding: 15px;'>Tiêu đề: </td><td> " + model.TieuDe + "</td></tr>";
                template += "<tr><td style='width: 150px; border-color: #bce8f1;  background-color: #d9edf7; color: #31708f;  padding: 15px;'>Lời nhắn: </td><td> " + model.LoiNhan + "</td></tr>";
                template += "</table>";
                template += "<b>Note: Đây là email tự động được gửi, quý khách vui lòng không phản hồi trực tiếp email này.</b>";
                Helpers.CommonHelper.SendMail(Helpers.SystemConfigHelper.GetSettingValue("NameEmail"), "Liên hệ từ khách hàng", template, Helpers.SystemConfigHelper.GetSettingValue("Email"));
                respone = new { mess = 1 }; // gửi đi thành công
                //}
                //else
                //    respone = new { mess = 2 }; // không đúng mã capthca
            }
            return Json(respone, JsonRequestBehavior.AllowGet);
        }
    }
}