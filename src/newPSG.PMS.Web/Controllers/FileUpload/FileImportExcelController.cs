using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Abp.Authorization;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Abp.UI;
using Abp.Web.Models;
using newPSG.PMS.IO;


namespace newPSG.PMS.Web.Controllers
{
    [AbpAuthorize]
    public class FileImportExcelController : PMSControllerBase
    {
        private readonly IAppFolders _appFolders;
        private readonly IAbpSession _session;
        private readonly ICacheManager _cacheManager;
        public FileImportExcelController(
            IAppFolders appFolders,
            IAbpSession session,
            ICacheManager cacheManager
        )
        {
            _appFolders = appFolders;
            _session = session;
            _cacheManager = cacheManager;
        }

        public JsonResult UploadExcelFileBlackList()
        {
            string fileExcelBlackListName = $"FileExcelFileBlackList_{AbpSession.GetUserId()}";
            try
            {
                //Check input
                if (Request.Files.Count <= 0 || Request.Files[0] == null)
                {
                    throw new UserFriendlyException(L("Excel_File_Error"));
                }

                var file = Request.Files[0];
                //Delete old 
                AppFileHelper.DeleteFilesInFolderIfExists(_appFolders.TempFileDownloadFolder, fileExcelBlackListName);

                //Save new
                var fileInfo = new FileInfo(file.FileName);
                var tempFileName = fileExcelBlackListName + fileInfo.Extension;
                var tempFilePath = Path.Combine(_appFolders.TempFileDownloadFolder, tempFileName);
                file.SaveAs(tempFilePath);
                return Json(new AjaxResponse(new { fileName = tempFileName }));
            }
            catch (UserFriendlyException ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }
    }
}