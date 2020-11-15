using Abp.Extensions;
using Abp.IdentityFramework;
using Abp.Web.Mvc.Controllers;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using HtmlAgilityPack;
using Microsoft.AspNet.Identity;
using newPSG.PMS.Authorization.Users;
using newPSG.PMS.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Mvc;

namespace newPSG.PMS.Web.Controllers
{
    /// <summary>
    /// Derive all Controllers from this class.
    /// Add your methods to this class common for all controllers.
    /// </summary>
    public abstract class PMSControllerBase : AbpController
    {
        protected PMSControllerBase()
        {
            LocalizationSourceName = PMSConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }

        protected XlsFile ExportExcelError(string value = "Xảy ra lỗi trong quá trình hiển thị báo cáo! Liên hệ trung tâm CSKH để được hỗ trợ!")
        {
            try
            {
                XlsFile result;
                result = new XlsFile(true);
                result.Open(Server.MapPath("~/PDFTemplate/rptError.xls"));
                var fr = new FlexCelReport();
                fr.SetValue("error", value);
                fr.Run(result);
                fr.Dispose();
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error($"{DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy")} ExportExcelError {ex.Message} {JsonConvert.SerializeObject(ex)}");
                return null;
            }
        }
      
        protected ActionResult ViewReport(FlexCel.Core.ExcelFile xls, string fileName, bool exportExcel = false)
        {
            try
            {
                if (exportExcel)
                {
                    if (xls == null)
                        return ExportExcelNotData();

                    var clsResult = new Models.Excel.clsExcelResult();
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                    {
                        xls.Save(ms);
                        ms.Position = 0;
                        clsResult.ms = ms;
                        clsResult.FileName = fileName;
                        clsResult.type = "xls";
                        return clsResult;
                    }
                }
                else
                {
                    if (xls == null)
                        xls = ExportExcelError("Không có giá trị");

                    using (FlexCel.Render.FlexCelPdfExport pdf = new FlexCel.Render.FlexCelPdfExport())
                    {
                        pdf.Workbook = xls;
                        using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                        {
                            pdf.BeginExport(ms);
                            pdf.ExportAllVisibleSheets(false, "PDF");
                            pdf.EndExport();
                            ms.Position = 0;
                            return File(ms.ToArray(), "application/pdf");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"{DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy")} ViewReport {ex.Message} {JsonConvert.SerializeObject(ex)}");
                return null;
            }
        }

        protected ActionResult ExportExcelNotData()
        {
            return PartialView("~/Views/Report/NotDataReport.cshtml");
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

        public User UserSession
        {
            get
            {
                var ret = new User();
                var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
                if (claimsPrincipal == null)
                {
                    return null;
                }

                var userClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == AppConsts.KeyClaimSession);
                if (userClaim == null || string.IsNullOrEmpty(userClaim.Value))
                {
                    return null;
                }
                ret = JsonConvert.DeserializeObject<User>(userClaim.Value);

                return ret;
            }
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
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

        public static string SinhMaHoSo(long hoSoId, DateTime ngayTaoHoSo, int thuTucId = 0)
        {
            return PMSAppServiceBase.SinhMaHoSo(hoSoId, ngayTaoHoSo, thuTucId);
        }

        protected TienIchCatChuoi xuLyVanBan(string noiDungHTML, int maxKyTuTrenDong = 0, int maxSoDongTrenDoanVan = 0)
        {
            TienIchCatChuoi result = new TienIchCatChuoi();

            var htmlDoc = new HtmlDocument();
            if (!string.IsNullOrEmpty(noiDungHTML))
            {
                htmlDoc.LoadHtml(noiDungHTML);
            }
            var pNotes = htmlDoc.DocumentNode.SelectNodes("//p | //div");
            var countDong = 0;
            if (pNotes != null)
            {
                foreach (var p in pNotes)
                {
                    countDong++;
                    countDong = TinhToanDongCongThem(p, countDong, maxKyTuTrenDong);
                    if (countDong < maxSoDongTrenDoanVan)
                    {
                        if (!string.IsNullOrWhiteSpace(p.InnerText))
                        {
                            if (string.IsNullOrWhiteSpace(result.NoiDung1))
                            {
                                result.NoiDung1 = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + result.NoiDung1 + p.InnerHtml.Replace("<br>", "");
                            }
                            else
                            {
                                result.NoiDung1 = result.NoiDung1 + "<br/>" + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + p.InnerHtml.Replace("<br>", "");
                            }
                        }
                    }
                    else if ((maxSoDongTrenDoanVan * 2 >= countDong) && (countDong * 2 > maxSoDongTrenDoanVan))
                    {
                        if (!string.IsNullOrWhiteSpace(p.InnerText))
                        {
                            if (string.IsNullOrWhiteSpace(result.NoiDung2))
                            {
                                result.NoiDung2 = result.NoiDung2 + p.InnerHtml.Replace("<br>", "");
                            }
                            else
                            {
                                result.NoiDung2 = result.NoiDung2 + "<br/>" + p.InnerHtml.Replace("<br>", "");
                            }
                        }
                    }
                    else if ((maxSoDongTrenDoanVan * 3 >= countDong) && (countDong * 3 > maxSoDongTrenDoanVan))
                    {
                        if (!string.IsNullOrWhiteSpace(p.InnerText))
                        {
                            if (string.IsNullOrWhiteSpace(result.NoiDung3))
                            {
                                result.NoiDung3 = result.NoiDung3 + p.InnerHtml.Replace("<br>", "");
                            }
                            else
                            {
                                result.NoiDung3 = result.NoiDung3 + "<br/>" + p.InnerHtml.Replace("<br>", "");
                            }
                        }
                    }
                    else if ((maxSoDongTrenDoanVan * 3 >= countDong) && (countDong * 4 < maxSoDongTrenDoanVan))
                    {
                        if (!string.IsNullOrWhiteSpace(p.InnerText))
                        {
                            if (string.IsNullOrWhiteSpace(result.NoiDung4))
                            {
                                result.NoiDung4 = result.NoiDung4 + p.InnerHtml.Replace("<br>", "");
                            }
                            else
                            {
                                result.NoiDung4 = result.NoiDung4 + "<br/>" + p.InnerHtml.Replace("<br>", "");
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(p.InnerText))
                        {
                            if (string.IsNullOrWhiteSpace(result.NoiDung5))
                            {
                                result.NoiDung5 = result.NoiDung5 + p.InnerHtml.Replace("<br>", "");
                            }
                            else
                            {
                                result.NoiDung5 = result.NoiDung5 + "<br/>" + p.InnerHtml.Replace("<br>", "");
                            }
                        }
                    }
                }
                result.MaxSoDongTrenDoanVan = maxSoDongTrenDoanVan;
                result.MaxSoTuTrenDong = maxKyTuTrenDong;
                result.NoiDungVanBan = noiDungHTML;
            }
            return result;
        }

        protected string RemoveElementBr(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return str;
            var index = str.IndexOf("<br/>");
            if (index != -1) str = str.Substring(5);

            index = str.LastIndexOf("<br>");
            if (index != -1) str = str.Substring(0, index);

            return str;
        }

        private int TinhToanDongCongThem(HtmlNode pHtml, int currDong, int maxKyTuTrenDong)
        {
            var strpLength = pHtml.InnerText.Length;
            if (strpLength > maxKyTuTrenDong)
            {
                var div = strpLength / maxKyTuTrenDong;
                if (div * maxKyTuTrenDong < strpLength)
                {
                    currDong += div + 1;
                }
                else
                {
                    currDong += div;
                }
            }
            return currDong;
        }

        protected System.Drawing.Image InitImage(string HCC_FILE_PDF, string path, int dWidth = 0, int dHeight = 0)
        {
            var urlImage = Path.Combine(HCC_FILE_PDF, path);
            byte[] ImageData = System.IO.File.ReadAllBytes(urlImage);

            System.Drawing.Image img = new System.Drawing.Bitmap(urlImage);

            if (dWidth > 0 && dHeight > 0)
            {
                return img.GetThumbnailImage(dWidth, dHeight, null, IntPtr.Zero);
            }
            var res= img.GetThumbnailImage(255, 115, null, IntPtr.Zero);
            img.Dispose();
            return res;
            //System.Drawing.Size thumbnailSize = GetThumbnailSize(img);
            //System.Drawing.Image thumbnail = img.GetThumbnailImage(thumbnailSize.Width, thumbnailSize.Height, null, IntPtr.Zero);
            //return thumbnail;
        }

        protected static byte[] ImageToByteArray(System.Drawing.Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        private System.Drawing.Size GetThumbnailSize(System.Drawing.Image original)
        {
            // Maximum size of any dimension.
            const int maxPixels = 116;

            // Width and height.
            int originalWidth = original.Width;
            int originalHeight = original.Height;

            // Compute best factor to scale entire image based on larger dimension.
            double factor;
            if (originalWidth > originalHeight)
            {
                factor = (double)maxPixels / originalWidth;
            }
            else
            {
                factor = (double)maxPixels / originalHeight;
            }

            // Return thumbnail size.
            return new System.Drawing.Size((int)(originalWidth * factor), (int)(originalHeight * factor));
        }

        public class TienIchCatChuoi
        {
            public TienIchCatChuoi()
            {
                NoiDung1 = "";
                NoiDung2 = "";
                NoiDung3 = "";
                NoiDung4 = "";
                NoiDung5 = "";
                NoiDungVanBan = "";
            }

            public string NoiDung1 { get; set; }
            public string NoiDung2 { get; set; }
            public string NoiDung3 { get; set; }
            public string NoiDung4 { get; set; }
            public string NoiDung5 { get; set; }
            public int MaxSoTuTrenDong { get; set; }
            public int MaxSoDongTrenDoanVan { get; set; }
            public string NoiDungVanBan { get; set; }
        }

        protected string FormatNumber(long? n, string dv = "")
        {
            CultureInfo elGR = CultureInfo.CreateSpecificCulture("el-GR");
            if (n == null) return "";
            //return $"{n.Value.ToString("#.#")} {dv}".Trim();
            return String.Format(elGR, "{0:0,0}", n.Value) + " " + dv;
        }

        protected string FormatDecimal(decimal? n, string dv = "")
        {
            CultureInfo elGR = CultureInfo.CreateSpecificCulture("el-GR");
            if (n == null) return "";
            //return String.Format("{0:0,0.00}", n.Value);
            //return $"{n.Value.ToString("#.#,00", System.Globalization.CultureInfo.InvariantCulture)} {dv}".Trim();
            return String.Format(elGR, "{0:0,0}", n.Value) + " " + dv;
        }

        protected string FormatDateTime(string n, string dv = "")
        {
            CultureInfo elGR = CultureInfo.CreateSpecificCulture("el-GR");
            if (n == null) return "";
            DateTime dt = DateTime.ParseExact(n, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            return dt.ToShortDateString();
        }

        protected string FormatYkien(string str, string title = "Ghi chú")
        {
            return string.IsNullOrWhiteSpace(str) ? ""
                    : string.Format("<b><u>{0}:</u> </b>{1}", title, str);
        }

        protected string FormatYkienChuyenVien(string str)
        {
            return string.IsNullOrWhiteSpace(str) ? ""
                    : string.Format("{0}", str);
        }

        public static string FirstCharToUpper(string input)
        {
            switch (input)
            {
                case null: throw new ArgumentNullException(nameof(input));
                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default: return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }
        protected List<CongVan> XuLyVanBan_New(string noiDungHTML)
        {
            var ret = new List<CongVan>();
            var htmlDoc = new HtmlDocument();
            if (!string.IsNullOrEmpty(noiDungHTML))
            {
                htmlDoc.LoadHtml(noiDungHTML);
            }
            else
            {
                return ret;
            }
            var pNotes = htmlDoc.DocumentNode.SelectNodes("//text()[normalize-space()]");
            var i = 1;
            foreach (var p in pNotes)
            {
                if (!string.IsNullOrWhiteSpace(p.InnerText))
                {

                    var isParentContent = p.ParentNode.Name == "b" || p.ParentNode.Name == "i" || p.ParentNode.Name == "u";
                    var isGrandContent = p.ParentNode.ParentNode != null ? p.ParentNode.ParentNode.Name == "b" || p.ParentNode.ParentNode.Name == "i" || p.ParentNode.ParentNode.Name == "u" : false;
                    var isGrandParentContent = (p.ParentNode.ParentNode != null && p.ParentNode.ParentNode.ParentNode != null) ? p.ParentNode.ParentNode.ParentNode.Name == "b" || p.ParentNode.ParentNode.ParentNode.Name == "i" || p.ParentNode.ParentNode.ParentNode.Name == "u" : false;

                    var addContent = p.OuterHtml;
                    if (isParentContent)
                    {
                        addContent = p.ParentNode.OuterHtml.Replace("<br>", "");
                    }
                    if (isGrandContent)
                    {
                        addContent = p.ParentNode.ParentNode.OuterHtml.Replace("<br>", "");
                    }
                    if (isGrandParentContent)
                    {
                        addContent = p.ParentNode.ParentNode.ParentNode.OuterHtml.Replace("<br>", "");
                    }

                    var cv = new CongVan();
                    cv.Id = i;
                    cv.TenCongVan = addContent;
                    i++;
                    ret.Add(cv);
                }
            }
            return ret;
        }
    }
}