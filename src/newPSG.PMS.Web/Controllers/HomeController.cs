using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using newPSG.PMS.Dto;
using newPSG.PMS.EntityDB;
using newPSG.PMS.MultiTenancy;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace newPSG.PMS.Web.Controllers
{
    public class HomeController : PMSControllerBase
    {
        private readonly IAppFolders _appFolders;
        private readonly IRepository<DoanhNghiep, long> _doanhNghiepRepos;
        private readonly TenantManager _tenantManager;
        //private readonly IThongKeChungAppService _thongKeChungService;

        public HomeController(
            IAbpSession abpSession,
            IAppFolders appFolders,
            IRepository<DoanhNghiep, long> doanhNghiepRepos,
            TenantManager tenantManager)
            //IThongKeChungAppService thongKeChungService)
        {
            _appFolders = appFolders;
            _doanhNghiepRepos = doanhNghiepRepos;
            _tenantManager = tenantManager;
            //_thongKeChungService = thongKeChungService;
        }

        public ActionResult Index()
        {
            var url = Request.Url.AbsoluteUri.ToLower();
            
            if (url.Contains("bqlattp."))
            {
                return View("~/Views/Home/HoChiMinh/Index.cshtml");
            }
            if (url.Contains("danang."))
            {
                return View("~/Views/Home/DaNang/Index.cshtml");
            }
            if (url.Contains("bacninh."))
            {
                return View("~/Views/Home/BacNinh/Index.cshtml");
            }
            return View();
        }

        public ActionResult Download()
        {
            ViewBag.statusResultDownloadFile = "error";
            ViewBag.descErrorDownloadFile = "";

            String fileName = Request.QueryString.Get("fileName");
            if (fileName == null || fileName.Trim().Length == 0)
            {
                ViewBag.descErrorDownloadFile = "FileName is empty";
                return View();
            }

            if (fileName.ToLower().IndexOf(".pdf") == -1
                    || !fileName.ToLower().Substring(fileName.ToLower().LastIndexOf(".pdf")).Equals(".pdf"))
            {
                ViewBag.descErrorDownloadFile = "Cần chọn file PDF có phần mở rộng .pdf";
                return View();
            }

            string pattern = @"[^-_.A-Za-z0-9]";
            Match m = Regex.Match(fileName, pattern, RegexOptions.IgnoreCase);
            if (m.Success)
            {
                ViewBag.descErrorHashFile = "FileName chứa ký tự đặc biệt";
                return View();
            }

            //Get the servers upload directory real path name
            String folderPath = Server.MapPath("~/upload/");
            String fileFullPath = folderPath + "/" + fileName;
            //create the upload folder if not exists
            //logger.info("fileFullPath: " + fileFullPath);
            bool exists = System.IO.File.Exists(fileFullPath);
            if (!exists)
            {
                ViewBag.descErrorHashFile = "File không tồn tại";
                return View();
            }
            try
            {
                FileInfo fileInfo = new FileInfo(fileFullPath);

                if (fileInfo.Exists)
                {
                    Response.Clear();
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + fileInfo.Name);
                    Response.AddHeader("Content-Length", fileInfo.Length.ToString());
                    Response.ContentType = "application/octet-stream";
                    Response.Flush();
                    Response.TransmitFile(fileInfo.FullName);
                    Response.End();
                }
                else
                {
                    ViewBag.descErrorHashFile = "File không tồn tại";
                    return View();
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"{DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy")} DownloadFile {ex.Message} {JsonConvert.SerializeObject(ex)}");
                ViewBag.descErrorHashFile = "Download File thất bại";
                return View();
            }

            ViewBag.statusResultDownloadFile = "success";

            return View();
        }

        [HttpPost]
        [ValidateInput(true)]
        public PartialViewResult ViewPDF(PdfDto pdfDto)
        {
            if (string.IsNullOrEmpty(pdfDto.Controller) || string.IsNullOrEmpty(pdfDto.Action) || pdfDto.HoSoId <= 0)
            {
                return null;
            }
            return PartialView("~/App/quanlyhoso/_common/excel/ViewPDF_LoadHtml.cshtml", pdfDto);
        }
    }
}