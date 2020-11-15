using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using newPSG.PMS.EntityDB;
using newPSG.PMS.MultiTenancy;
using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace newPSG.PMS
{
    public abstract class PMSDomainServiceBase : DomainService
    {
        /* Add your common members for all your domain services. */

        protected PMSDomainServiceBase()
        {
            LocalizationSourceName = PMSConsts.LocalizationSourceName;
        }

        protected string CreateGuid(long hoSoId, string strChiCuc)
        {
            return $"{hoSoId.ToString()}.{strChiCuc}";
        }

        protected string RemoveUnicode(string text)
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

            text = text.ToLower();
            for (int i = 0; i < arr1.Length; i++)
            {
                text = text.Replace(arr1[i], arr2[i]);
            }
            text = text.Replace(" ", "");
            return text;
        }

        protected string RemoveUnicodeTinh(int chiCucId)
        {
            using (var scope = IocManager.Instance.CreateScope())
            {
                var _tinhRepos = scope.Resolve<IRepository<Tinh, int>>();
                var _tenantRepos = scope.Resolve<IRepository<Tenant>>();
                string strTinh = "";
                var _tenant = _tenantRepos.Get(chiCucId);
                if (_tenant != null)
                {
                    var _tinh = _tinhRepos.Get((int)_tenant.TinhId.Value);
                    if (_tinh != null)
                    {
                        strTinh = RemoveUnicode(_tinh.Ten);
                    }
                }
                return strTinh;
            }
        }

        protected string FileToBase64String(string pathFile)
        {
            try
            {
                string ATTP_FILE_PDF = GetUrlFileDefaut();
                var folderPath = Path.Combine(ATTP_FILE_PDF, pathFile);

                byte[] bytes = File.ReadAllBytes(folderPath);
                string pdfBase64 = Convert.ToBase64String(bytes);

                return pdfBase64;
            }
            catch
            {
                return null;
            }
        }

        protected string GetUrlFileDefaut()
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
    }
}

