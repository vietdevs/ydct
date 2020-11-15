using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.IdentityFramework;
using Abp.MultiTenancy;
using Abp.Runtime.Session;
using Microsoft.AspNet.Identity;
using newPSG.PMS.Authorization.Users;
using newPSG.PMS.MultiTenancy;
using Abp.Dependency;
using System.Threading;
using System.Security.Claims;
using Newtonsoft.Json;
using System.Linq;
using System.Configuration;
using System.IO;
using System.Net;
using System.Collections.Specialized;
using System.Text;
using newPSG.PMS.Common.Dto;
using newPSG.PMS.EntityDB;
using Abp.Domain.Repositories;
using System.Web;
using newPSG.PMS.Configuration.Tenants;
using System.Collections.Generic;
using newPSG.PMS.Dto;

namespace newPSG.PMS
{
    /// <summary>
    /// All application services in this application is derived from this class.
    /// We can add common application service methods here.
    /// </summary>
    public abstract class PMSAppServiceBase : ApplicationService
    {
        public TenantManager TenantManager { get; set; }

        public UserManager UserManager { get; set; }

        protected PMSAppServiceBase()
        {
            LocalizationSourceName = PMSConsts.LocalizationSourceName;
        }

        protected virtual async Task<User> GetCurrentUserAsync()
        {
            var user = await UserManager.FindByIdAsync(AbpSession.GetUserId());
            if (user == null)
            {
                throw new ApplicationException("There is no current user!");
            }

            return user;
        }

        protected virtual User GetCurrentUser()
        {
            var user = UserManager.FindById(AbpSession.GetUserId());
            if (user == null)
            {
                throw new ApplicationException("There is no current user!");
            }

            return user;
        }

        protected virtual Task<Tenant> GetCurrentTenantAsync()
        {
            using (CurrentUnitOfWork.SetTenantId(null))
            {
                return TenantManager.GetByIdAsync(AbpSession.GetTenantId());
            }
        }

        protected virtual Tenant GetCurrentTenant()
        {
            using (CurrentUnitOfWork.SetTenantId(null))
            {
                return TenantManager.GetById(AbpSession.GetTenantId());
            }
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }

        public string GetUrlFileDefaut()
        {
            string HCC_FILE_PDF = ConfigurationManager.AppSettings["HCC_FILE_PDF"];
            if (string.IsNullOrEmpty(HCC_FILE_PDF))
            {
                HCC_FILE_PDF = @"C:\HCC_FILE_PDF";
            }

            if (!Directory.Exists(HCC_FILE_PDF))
            {
                Directory.CreateDirectory(HCC_FILE_PDF);
            }
            return HCC_FILE_PDF;

        }

        protected static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        protected static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public bool VerifyEmail(string emailVerify)
        {
            using (WebClient webclient = new WebClient())
            {
                string url = "http://verify-email.org/";
                NameValueCollection formData = new NameValueCollection();
                formData["check"] = emailVerify;
                byte[] responseBytes = webclient.UploadValues(url, "POST", formData);
                string response = Encoding.ASCII.GetString(responseBytes);
                if (response.Contains("Result: Ok"))
                {
                    return true;
                }
                return false;
            }
        }

        public bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static string RemoveUnicode(string text)
        {
            string[] arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
            "đ",
            "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
            "í","ì","ỉ","ĩ","ị",
            "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
            "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
            "ý","ỳ","ỷ","ỹ","ỵ",};
            string[] arr2 = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
            "d",
            "e","e","e","e","e","e","e","e","e","e","e",
            "i","i","i","i","i",
            "o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",
            "u","u","u","u","u","u","u","u","u","u","u",
            "y","y","y","y","y",};
            for (int i = 0; i < arr1.Length; i++)
            {
                text = text.Replace(arr1[i], arr2[i]);
                text = text.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
            }
            return text;
        }

        public string CopyTo(string sourcePath, string targetPath)
        {
            try
            {
                string HCC_FILE_PDF = GetUrlFileDefaut();
                var index = sourcePath.LastIndexOf("\\");
                string fileName = sourcePath.Substring(index + 1);

                string sourceFile = System.IO.Path.Combine(HCC_FILE_PDF, sourcePath);
                string destFile = System.IO.Path.Combine(HCC_FILE_PDF, targetPath, fileName);
                var folderPath = Path.Combine(HCC_FILE_PDF, targetPath);

                if (!System.IO.Directory.Exists(folderPath))
                {
                    System.IO.Directory.CreateDirectory(folderPath);
                }

                System.IO.File.Copy(sourceFile, destFile, true);
                return targetPath + fileName;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public void EmptyFolder(string path)
        {
            try
            {
                string HCC_FILE_PDF = GetUrlFileDefaut();
                var folderPath = Path.Combine(HCC_FILE_PDF, path);

                DirectoryInfo directory = new DirectoryInfo(folderPath);
                foreach (System.IO.FileInfo file in directory.GetFiles()) file.Delete();
                foreach (System.IO.DirectoryInfo subDirectory in directory.GetDirectories()) subDirectory.Delete(true);

                directory.Delete();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
        }

        public static string SinhMaHoSo(long hoSoId, DateTime ngayTaoHoSo, int thuTucId = 0)
        {
            try
            {
                CommonENum.THU_TUC_ID thuTuc = CommonENum.THU_TUC_ID.THU_TUC_99;
                if (thuTucId > 0)
                {
                    thuTuc = (CommonENum.THU_TUC_ID)thuTucId;
                }
                //var res = string.Format("{0}.{1}.{2}", ngayTaoHoSo.ToString("yy.MM.dd"), hoSoId, CommonENum.GetEnumDescription(thuTuc));
                var res = string.Format("{0}/{1}", hoSoId.ToString("D4"), CommonENum.GetEnumDescription(thuTuc));
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string LayDuongDanTuyetDoiTaiLieu(string pathTaiLieu)
        {
            try
            {
                var fullPath = pathTaiLieu;
                string host = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                if (!string.IsNullOrEmpty(host) && !fullPath.Contains(host))
                {
                    fullPath = string.Format("{0}/File/GoToViewTaiLieu?url={1}", host, pathTaiLieu);
                }
                return fullPath;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string GetDuongDanTuongDoiApi(string duongDanTuongDoi)
        {
            string path = "/File/GoToViewTaiLieu?url=" + duongDanTuongDoi;
            return path;
        }

        protected string GetDuongDanTuyetDoiApi(string duongDanTuyetDoi)
        {
            try
            {
                string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                string path = domainName + "/File/GoToViewTaiLieu?url=" + duongDanTuyetDoi;

                return path;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}