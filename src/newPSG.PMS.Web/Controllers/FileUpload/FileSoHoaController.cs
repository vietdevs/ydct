using Abp.Auditing;
using Abp.Domain.Repositories;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Abp.Runtime.Validation;
using Abp.UI;
using Abp.Web.Models;
using Abp.Web.Mvc.Authorization;
using newPSG.PMS.Dto;
using newPSG.PMS.EntityDB;
using newPSG.PMS.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace newPSG.PMS.Web.Controllers
{
    public class FileSoHoaController : PMSControllerBase
    {
        private readonly IAppFolders _appFolders;
        private readonly IAbpSession _session;
        private readonly ICacheManager _cacheManager;

        public FileSoHoaController(IAppFolders appFolders,
            IAbpSession session,
            ICacheManager cacheManager
        )
        {
            _appFolders = appFolders;
            _session = session;
            _cacheManager = cacheManager;
        }

        #region Upload tai lieu so hoa

        //Upload File 
        [DisableValidation]
        public JsonResult UploadTaiLieuSoHoa(string folderName = "TaiLieuSoHoa")
        {
            try
            {
                var userId = _session.UserId;

                //Check input
                if (Request.Files.Count <= 0 || Request.Files[0] == null)
                {
                    throw new UserFriendlyException(L("Excel_File_Error"));
                }

                var file = Request.Files[0];
                var fileExtension = Path.GetExtension(file.FileName);
                var fileNameNotExtension = Path.GetFileNameWithoutExtension(file.FileName);
                var fileNameWithOutExtensionEncode = Base64Encode(fileNameNotExtension);

                var ngayTao = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var fileName = RemoveUnicode(fileNameNotExtension).Replace(" ", "-") + "_" + ngayTao + fileExtension;
                fileName = HttpUtility.UrlEncode(fileName);


                var thuMucThang = DateTime.Now.ToString("yyyyMM");
                var pathName = $"\\HCC_TAILIEUSO\\{folderName}\\{thuMucThang}\\";

                string HCC_FILE_PDF = GetUrlFileDefaut();
                var folderPath = Path.Combine(HCC_FILE_PDF, pathName);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                //Delete old temp profile pictures
                AppFileHelper.DeleteFilesInFolderIfExists(folderPath, fileName);

                //Save new picture
                file.SaveAs(Path.Combine(folderPath, fileName));

                return Json(new { fileName = pathName + fileName }, JsonRequestBehavior.AllowGet);
            }
            catch (UserFriendlyException ex)
            {
                Logger.Error($"{DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy")} Upload [HCC_TAILIEUSO - {folderName}] {ex.Message} {JsonConvert.SerializeObject(ex)}");
                return Json(new ErrorInfo(ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}