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
    public class FileController : PMSControllerBase
    {
        private readonly IAppFolders _appFolders;
        private readonly IAbpSession _session;
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<DoanhNghiep, long> _doanhNghiepRepos;
        private readonly IRepository<LogUploadFile, long> _uploadFileRepos;
        private readonly IRepository<LoaiHoSo> _loaiHoSoRepos;

        public FileController(IAppFolders appFolders,
            IAbpSession session,
            ICacheManager cacheManager,
            IRepository<DoanhNghiep, long> doanhNghiepRepos,
            IRepository<LogUploadFile, long> uploadFileRepos,
            IRepository<LoaiHoSo> loaiHoSoRepos)
        {
            _appFolders = appFolders;
            _session = session;
            _cacheManager = cacheManager;
            _doanhNghiepRepos = doanhNghiepRepos;
            _uploadFileRepos = uploadFileRepos;
            _loaiHoSoRepos = loaiHoSoRepos;
        }

        #region Download file
        [AbpMvcAuthorize]
        [DisableAuditing]
        public ActionResult DownloadTempFile(FileDto file)
        {
            var filePath = Path.Combine(_appFolders.TempFileDownloadFolder, file.FileToken);
            if (!System.IO.File.Exists(filePath))
            {
                throw new UserFriendlyException(L("RequestedFileDoesNotExists"));
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            System.IO.File.Delete(filePath);
            return File(fileBytes, file.FileType, file.FileName);
        }
        
        public FileResult DownloadTepDinhKem(string duongDanTep, string tenTep)
        {
            if (!string.IsNullOrEmpty(duongDanTep) && !string.IsNullOrEmpty(tenTep))
            {
                string HCC_FILE_PDF = GetUrlFileDefaut();
                string duongDanTuyetDoi = Path.Combine(HCC_FILE_PDF, duongDanTep);
                if (!System.IO.File.Exists(duongDanTuyetDoi))
                {
                    throw new UserFriendlyException(L("RequestedFileDoesNotExists"));
                }
                byte[] fileBytes = System.IO.File.ReadAllBytes(duongDanTuyetDoi);
                string fileName = tenTep;
                return File(fileBytes, fileName);
            }
            else
            {
                throw new UserFriendlyException("Không tìm thấy đường dẫn và tên file");
            }
        }
        #endregion

        #region Upload file
        public JsonResult UploadTempFile(string folderName)
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
                var pathName = @"Temp\" + folderName;

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

                return Json(new { fileName = fileName, pathName = pathName + "\\" + fileName }, JsonRequestBehavior.AllowGet);
            }
            catch (UserFriendlyException ex)
            {
                Logger.Error($"{DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy")} UploadTaiLieuHoSo {ex.Message} {JsonConvert.SerializeObject(ex)}");
                return Json(new ErrorInfo(ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UploadChuKy(string maChuKy)
        {
            try
            {
                var userId = _session.UserId;
                //Check input
                if (Request.Files.Count <= 0 || Request.Files[0] == null)
                {
                    throw new UserFriendlyException(L("ProfilePicture_Change_Error"));
                }

                var file = Request.Files[0];

                if (file.ContentLength > 1048576) //1MB.
                {
                    throw new UserFriendlyException(L("ProfilePicture_Warn_SizeLimit"));
                }

                //Check file type & format
                var fileImage = System.Drawing.Image.FromStream(file.InputStream);

                var acceptedFormats = new List<ImageFormat>
                {
                    ImageFormat.Jpeg, ImageFormat.Png, ImageFormat.Gif
                };

                if (!acceptedFormats.Contains(fileImage.RawFormat))
                {
                    throw new ApplicationException("Uploaded file is not an accepted image file !");
                }

                //Delete old temp signatures
                AppFileHelper.DeleteFilesInFolderIfExists(_appFolders.TempFileDownloadFolder, userId + "_" + maChuKy);

                //Save new picture
                var fileInfo = new FileInfo(file.FileName);
                var tempFileName = userId + "_" + maChuKy + fileInfo.Extension;
                var tempFilePath = Path.Combine(_appFolders.TempFileDownloadFolder, tempFileName);
                file.SaveAs(tempFilePath);
                fileImage.Dispose();

                using (var bmpImage = new Bitmap(tempFilePath))
                {
                    return Json(new AjaxResponse(new { fileName = tempFileName }));
                }
            }
            catch (UserFriendlyException ex)
            {
                Logger.Error($"{DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy")} UploadChuKy {ex.Message} {JsonConvert.SerializeObject(ex)}");
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        public JsonResult UploadIconLoaiHoSo()
        {
            try
            {
                var userId = _session.UserId;
                //Check input
                if (Request.Files.Count <= 0 || Request.Files[0] == null)
                {
                    throw new UserFriendlyException(L("ProfilePicture_Change_Error"));
                }
                var file = Request.Files[0];
                if (file.ContentLength > 1048576) //1MB.
                {
                    throw new UserFriendlyException(L("ProfilePicture_Warn_SizeLimit"));
                }
                //Check file type & format
                var fileImage = System.Drawing.Image.FromStream(file.InputStream);
                var acceptedFormats = new List<ImageFormat>
                    {
                        ImageFormat.Jpeg, ImageFormat.Png, ImageFormat.Gif
                    };
                if (!acceptedFormats.Contains(fileImage.RawFormat))
                {
                    throw new ApplicationException("Uploaded file is not an accepted image file !");
                }
                var webImage = new System.Web.Helpers.WebImage(Request.Files[0].InputStream);
                byte[] imgByteArray = webImage.GetBytes();

                fileImage.Dispose();
                return Json(new AjaxResponse(new { data = imgByteArray }));
            }
            catch (UserFriendlyException ex)
            {
                Logger.Error($"{DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy")} UploadIconLoaiHoSo {ex.Message} {JsonConvert.SerializeObject(ex)}");
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        //Upload File 
        [DisableValidation]
        public JsonResult UploadTaiLieuHoSo(string maThuTuc, string maSoThue, string strThuMucHoSo = "HOSO_0", string folderName = "tepdinhkem")
        {
            try
            {
                if (string.IsNullOrEmpty(maThuTuc))
                {
                    maThuTuc = "TT-unknown";
                }
                var userId = _session.UserId;

                //Check input
                if (Request.Files.Count <= 0 || Request.Files[0] == null)
                {
                    throw new UserFriendlyException(L("Excel_File_Error"));
                }

                //lấy tỉnh để tạo thêm thư mục
                var strTinh = "unknown";
                var doanhnghiep = _doanhNghiepRepos.FirstOrDefault(x => x.MaSoThue == maSoThue);
                if (doanhnghiep != null)
                {
                    strTinh = !string.IsNullOrEmpty(doanhnghiep.Tinh) ? RemoveUnicode(doanhnghiep.Tinh).ToLower().Trim().Replace(" ", "-") : "unknown";
                }

                var file = Request.Files[0];
                var fileExtension = Path.GetExtension(file.FileName);
                var fileNameNotExtension = Path.GetFileNameWithoutExtension(file.FileName);
                var fileNameWithOutExtensionEncode = Base64Encode(fileNameNotExtension);

                var ngayTao = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var fileName = RemoveUnicode(fileNameNotExtension).Replace(" ", "-") + "_" + maSoThue + "_" + ngayTao + fileExtension;
                fileName = HttpUtility.UrlEncode(fileName);

                if (string.IsNullOrEmpty(strThuMucHoSo) || strThuMucHoSo == "HOSO_0")
                {
                    strThuMucHoSo = $"HOSO_0";
                }

                var pathName = $"{maThuTuc}\\{strTinh}\\{maSoThue}\\{strThuMucHoSo}\\{folderName}\\";

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

                //LogUploadFile
                LogUploadFile uploadObj = new LogUploadFile
                {
                    DuongDanTep = pathName + fileName,
                    DaTaiLen = true
                };
                long _uploadFileId = _uploadFileRepos.InsertAndGetId(uploadObj);

                return Json(new
                {
                    fileName = fileNameNotExtension + fileExtension,
                    filePath = pathName + fileName,
                    uploadFileId = _uploadFileId
                },JsonRequestBehavior.AllowGet);
            }
            catch (UserFriendlyException ex)
            {
                Logger.Error($"{DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy")} Upload [{strThuMucHoSo} - {folderName}] {ex.Message} {JsonConvert.SerializeObject(ex)}");
                return Json(new ErrorInfo(ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region View file
        public FileResult GoToViewTaiLieu(string url)
        {
            try
            {
                if (!string.IsNullOrEmpty(url))
                {
                    string fileExtension = ".pdf";
                    string ATTP_FILE_PDF = GetUrlFileDefaut();
                    var duongDanTuyetDoi = Path.Combine(ATTP_FILE_PDF, url);

                    if (!System.IO.File.Exists(duongDanTuyetDoi))
                    {
                        throw new UserFriendlyException("Không tìm thấy đường dẫn và tên file");
                    }

                    byte[] fileBytes = System.IO.File.ReadAllBytes(duongDanTuyetDoi);
                    string fileName = HttpUtility.UrlDecode(url);
                    fileExtension = "." + fileName.Substring(fileName.LastIndexOf(".") + 1);
                    if (fileExtension == ".mp3")
                    {
                        return File(fileBytes, "audio/mp3");
                    }
                    else if (fileExtension == ".mp4")
                    {
                        return File(fileBytes, "video/mp4");
                    }
                    return File(fileBytes, "application/pdf");
                }
                else
                {
                    throw new UserFriendlyException("Không tìm thấy đường dẫn và tên file");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"{DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy")} GoToViewTaiLieu {ex.Message} {JsonConvert.SerializeObject(ex)}");
                throw new UserFriendlyException("Có lỗi xảy ra");
            }
        }
        #endregion
    }
}