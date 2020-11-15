using Abp.Domain.Repositories;
using newPSG.PMS.EntityDB;
using newPSG.PMS.Web.Utils;
using Newtonsoft.Json;
using Org.BouncyCastle.X509;
using Spire.Pdf;
using Spire.Pdf.General.Find;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using ViettelFileSigner;

namespace newPSG.PMS.Web.Controllers
{
    public class USBTokenController : PMSControllerBase
    {
        private readonly IRepository<ChuKy, long> _chuKyRepos;
        private readonly IRepository<LogFileKy, long> _fileKyRepos;
        public USBTokenController(IRepository<ChuKy, long> chuKyRepos,
                                  IRepository<LogFileKy, long> fileKyRepos)
        {
            _chuKyRepos = chuKyRepos;
            _fileKyRepos = fileKyRepos;
        }

        private SignPdfFile pdfSig;

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// LoaiChuKy: =0 là có chữ ký, =1 ký theo dạng khung kèm theo number page
        /// </summary>
        /// <param name="certUserBase64"></param>
        /// <param name="fileName"></param>
        /// <param name="LoaiChuKy"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(true)]
        public ActionResult Hash(string certUserBase64 = "", string fileName = "", int LoaiChuKy = 0, int numberPage = 1)
        {
            var statusResultHashFile = "error";
            var descErrorHashFile = "";
            var serialNumber = "";
            var hashBase64 = "";
            try
            {
                if (certUserBase64 == null || certUserBase64.Trim().Length == 0)
                {
                    descErrorHashFile = "Cert Chain is empty";
                    return Json(new
                    {
                        statusResultHashFile = statusResultHashFile,
                        descErrorHashFile = descErrorHashFile,
                        serialNumber = "",
                        hashBase64 = ""
                    }, JsonRequestBehavior.AllowGet);
                }
                if (fileName == null || fileName.Trim().Length == 0 || fileName.IndexOf(".pdf") == -1)
                {
                    descErrorHashFile = "Chưa chọn File cần ký";
                    return Json(new
                    {
                        statusResultHashFile = statusResultHashFile,
                        descErrorHashFile = descErrorHashFile,
                        serialNumber = "",
                        hashBase64 = ""
                    }, JsonRequestBehavior.AllowGet);
                }

                //Get the servers upload directory real path name

                string HCC_FILE_PDF = GetUrlFileDefaut();
                string fileFullPath = Path.Combine(HCC_FILE_PDF, fileName);

                bool exists = System.IO.File.Exists(fileFullPath);
                if (!exists)
                {
                    descErrorHashFile = "File không tồn tại";
                    return Json(new
                    {
                        statusResultHashFile = statusResultHashFile,
                        descErrorHashFile = descErrorHashFile,
                        serialNumber = "",
                        hashBase64 = ""
                    }, JsonRequestBehavior.AllowGet);
                }

                string[] stringSeparators = new string[] { "," };
                String[] chainBase64 = certUserBase64.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);

                if (chainBase64 == null || chainBase64.Length == 0)
                {
                    descErrorHashFile = "Lấy Chứng thư số không thành công";
                    return Json(new
                    {
                        statusResultHashFile = statusResultHashFile,
                        descErrorHashFile = descErrorHashFile,
                        serialNumber = "",
                        hashBase64 = ""
                    }, JsonRequestBehavior.AllowGet);
                }
                X509Certificate x509Cert = CertUtils.GetX509Cert(chainBase64[0]);
                //Check chứng thư số còn giá trị
                //#if DEBUG

                //#else
                //    var tokenValid = x509Cert.IsValidNow;
                //    if (tokenValid != true)
                // {
                //        descErrorHashFile = "Chứng thư số đã hết hiệu lực.";
                //                    return Json(new
                //                    {
                //                        statusResultHashFile = statusResultHashFile,
                //                        descErrorHashFile = descErrorHashFile,
                //                        serialNumber = "",
                //                        hashBase64 = ""
                //                    }, JsonRequestBehavior.AllowGet);
                // }
                //#endif

                if (x509Cert == null)
                {
                    descErrorHashFile = "Lấy Chứng thư số không thành công";
                    return Json(new
                    {
                        statusResultHashFile = statusResultHashFile,
                        descErrorHashFile = descErrorHashFile,
                        serialNumber = "",
                        hashBase64 = ""
                    }, JsonRequestBehavior.AllowGet);
                }
                X509Certificate certCA = null;
                if (chainBase64.Length > 1)
                {
                    certCA = CertUtils.GetX509Cert(chainBase64[1]);
                }
                X509Certificate[] certChain = null;
                if (certCA != null)
                {
                    certChain = new X509Certificate[] { x509Cert, certCA };
                }
                else
                {
                    string certViettelCABase64 = "MIIEKDCCAxCgAwIBAgIKYQ4N5gAAAAAAETANBgkqhkiG9w0BAQUFADB+MQswCQYDVQQGEwJWTjEzMDEGA1UEChMqTWluaXN0cnkgb2YgSW5mb3JtYXRpb24gYW5kIENvbW11bmljYXRpb25zMRswGQYDVQQLExJOYXRpb25hbCBDQSBDZW50ZXIxHTAbBgNVBAMTFE1JQyBOYXRpb25hbCBSb290IENBMB4XDTE1MTAwMjAyMzIyMFoXDTIwMTAwMjAyNDIyMFowOjELMAkGA1UEBhMCVk4xFjAUBgNVBAoTDVZpZXR0ZWwgR3JvdXAxEzARBgNVBAMTClZpZXR0ZWwtQ0EwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQDLdiGZcPhwSm67IiLUWELaaol8kHF+qHPmEdcG0VDKf0FtpSWiE/t6NPzqqmoF4gbIrue1/TzUs7ZeAj28o6Lb2BllA/zB6YFrXfppD4jKqHMO139970MeTbDrhHTbVugX4t2QHS+B/p8+8lszJpuduBrnZ/LWxbhnjeQRr21g89nh/W5q1VbIvZnq4ci5m0aDiJ8arhK2CKpvNDWWQ5E0L7NTVoot8niv6/Wjz19yvUCYOKHYsq97y7eBaSYmpgJosD1VtnXqLG7x4POdb6Q073eWXQB0Sj1qJPrXtOqWsnnmzbbKMrnjsoE4gg9B6qLyQS4kRMp0RrUV0z041aUFAgMBAAGjgeswgegwCwYDVR0PBAQDAgGGMBIGA1UdEwEB/wQIMAYBAf8CAQAwHQYDVR0OBBYEFAhg5h8bFNlIgAtep1xzJSwgDfnWMB8GA1UdIwQYMBaAFM1iceRhvf497LJAYNOBdd06rGvGMDwGA1UdHwQ1MDMwMaAvoC2GK2h0dHA6Ly9wdWJsaWMucm9vdGNhLmdvdi52bi9jcmwvbWljbnJjYS5jcmwwRwYIKwYBBQUHAQEEOzA5MDcGCCsGAQUFBzAChitodHRwOi8vcHVibGljLnJvb3RjYS5nb3Yudm4vY3J0L21pY25yY2EuY3J0MA0GCSqGSIb3DQEBBQUAA4IBAQCHtdHJXudu6HjO0571g9RmCP4b/vhK2vHNihDhWYQFuFqBymCota0kMW871sFFSlbd8xD0OWlFGUIkuMCz48WYXEOeXkju1fXYoTnzm5K4L3DV7jQa2H3wQ3VMjP4mgwPHjgciMmPkaBAR/hYyfY77I4NrB3V1KVNsznYbzbFtBO2VV77s3Jt9elzQw21bPDoXaUpfxIde+bLwPxzaEpe7KJhViBccJlAlI7pireTvgLQCBzepJJRerfp+GHj4Z6T58q+e3a9YhyZdtAHVisWYQ4mY113K1V7Z4D7gisjbxExF4UyrX5G4W0h0gXAR5UVOstv5czQyDraTmUTYtx5J";
                    X509Certificate certViettelCA = CertUtils.GetX509Cert(certViettelCABase64);
                    if (certViettelCA != null)
                    {
                        certChain = new X509Certificate[] { x509Cert, certViettelCA };
                    }
                }

                if (certChain == null || certChain.Length != 2)
                {
                    descErrorHashFile = "Lấy Chứng thư số không thành công. Không lấy được CTS CA.";
                    return View();
                }

                String base64Hash = null;

                //Trước khi insert thì kiểm tra chữ ký
                if (LoaiChuKy == 0)
                {
                    //Lấy ra các loại chữ ký
                    var lstChuKy = (from t in _chuKyRepos.GetAll() where t.IsActive == true && t.UserId == UserSession.Id select t).ToList();
                    if (lstChuKy.Count == 0)
                    {
                        descErrorHashFile = "Người dùng chưa import chữ ký, Vui lòng import chữ ký";
                        return Json(new
                        {
                            statusResultHashFile = statusResultHashFile,
                            descErrorHashFile = descErrorHashFile,
                            serialNumber = "",
                            hashBase64 = ""
                        }, JsonRequestBehavior.AllowGet);
                    }
                    if (lstChuKy.Count > 0)
                    {
                        int trangky = 1;
                        float vitrix = 0;
                        float vitriy = 0;
                        float widthImg = 0;
                        float heightImg = 0;

                        Spire.Pdf.PdfDocument doc = new Spire.Pdf.PdfDocument();
                        doc.LoadFromFile(fileFullPath);
                        PdfTextFind[] results = null;

                        int checkChuKy = 0;
                        for (int k = 0; k < doc.Pages.Count; k++)

                        {
                            PdfPageBase page = doc.Pages[k];
                            for (int i = 0; i < lstChuKy.Count; i++)
                            {
                                var machu = "ATTP_" + lstChuKy[i].MaChuKy;
                                results = page.FindText(machu).Finds;
                                if (results.Length > 0)
                                {
                                    foreach (PdfTextFind text in results)
                                    {
                                        var ImageData = lstChuKy[i].DataImage;
                                        var urlImage = (Path.Combine(HCC_FILE_PDF, lstChuKy[i].UrlImage));
                                        if (!string.IsNullOrEmpty(urlImage))
                                        {
                                            if (!System.IO.File.Exists(urlImage))
                                            {
                                                descErrorHashFile = "Chữ ký người dùng chưa import";
                                                return Json(new
                                                {
                                                    statusResultHashFile = statusResultHashFile,
                                                    descErrorHashFile = descErrorHashFile,
                                                    serialNumber = "",
                                                    hashBase64 = ""
                                                }, JsonRequestBehavior.AllowGet);
                                            }

                                            ImageData = System.IO.File.ReadAllBytes(urlImage);
                                        }
                                        //System.Drawing.Image image = null;
                                        //using (var ms = new MemoryStream(ImageData))
                                        //{
                                        //    image = Image.FromStream(ms);
                                        //}

                                        trangky = k + 1;
                                        vitrix = text.Position.X;
                                        vitriy = page.Size.Height - text.Position.Y - 20;
                                        widthImg = 80;
                                        heightImg = 40;
                                        if (lstChuKy[i].LoaiChuKy == (int)CommonENum.LOAI_CHU_KY.CHU_KY)
                                        {
                                            if (machu == "ATTP_CK_1_1")
                                            {
                                                widthImg = 171;
                                                heightImg = 100;
                                                vitrix = text.Position.X - 30;
                                                vitriy = page.Size.Height - text.Position.Y - 80;
                                            }
                                            else
                                            {
                                                widthImg = 80;
                                                heightImg = 40;
                                                vitriy = page.Size.Height - text.Position.Y - 20;
                                            }

                                            //điều chỉnh kích thước theo cấu hình của chữ ký
                                            if (lstChuKy[i].ChieuRong.HasValue && lstChuKy[i].ChieuRong > 0)
                                            {
                                                widthImg = lstChuKy[i].ChieuRong.Value;
                                            }
                                            if (lstChuKy[i].ChieuCao.HasValue && lstChuKy[i].ChieuCao > 0)
                                            {
                                                heightImg = lstChuKy[i].ChieuCao.Value;
                                            }
                                        }
                                        if (lstChuKy[i].LoaiChuKy == (int)CommonENum.LOAI_CHU_KY.DAU_CUA_CUC)
                                        {
                                            widthImg = 75;
                                            heightImg = 75;
                                            vitrix = text.Position.X - (widthImg / 2);
                                            vitriy = page.Size.Height - text.Position.Y - heightImg;
                                        }
                                        base64Hash = HashFilePDF.GetHashTypeImage_ExistedSignatureField(fileFullPath, certChain, k + 1, urlImage, vitrix, vitriy, widthImg, heightImg);
                                        checkChuKy = 1;
                                    }
                                }
                            }
                        }
                        if (checkChuKy == 0)
                        {
                            base64Hash = HashFilePDF.GetHashTypeRectangleText(1, fileFullPath, certChain);
                        }
                    }
                }
                else if (LoaiChuKy == 1)
                {
                    base64Hash = HashFilePDF.GetHashTypeRectangleText(numberPage, fileFullPath, certChain);
                }
                else
                {
                    base64Hash = HashFilePDF.GetHashTypeRectangleText(numberPage, fileFullPath, certChain);
                }
                if (base64Hash == null)
                {
                    descErrorHashFile = "Tạo Hash không thành công";
                    return View();
                }

                statusResultHashFile = "success";
                serialNumber = x509Cert.SerialNumber.ToString(16);
                hashBase64 = base64Hash;
                return Json(new
                {
                    statusResultHashFile = statusResultHashFile,
                    descErrorHashFile = descErrorHashFile,
                    serialNumber = serialNumber,
                    hashBase64 = hashBase64
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                descErrorHashFile = "Lỗi trong quá trình xử lý";
                return Json(new
                {
                    statusResultHashFile = statusResultHashFile,
                    descErrorHashFile = descErrorHashFile,
                    serialNumber = "",
                    hashBase64 = ""
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: USBToken
        [HttpPost]
        [ValidateInput(true)]
        public ActionResult InsertSignatrue(string signatureBase64, string fileName)
        {
            var statusResultInsertSignature = "error";
            var descErrorInsertSignature = "";
            var signedFileName = "";
            try
            {
                if (signatureBase64 == null || signatureBase64.Trim().Length == 0)
                {
                    descErrorInsertSignature = "Signature is empty";
                    return Json(new
                    {
                        statusResultInsertSignature = statusResultInsertSignature,
                        descErrorInsertSignature = descErrorInsertSignature,
                        signedFileName = signedFileName
                    }, JsonRequestBehavior.AllowGet);
                }
                if (fileName == null || fileName.Trim().Length == 0 || fileName.IndexOf(".pdf") == -1)
                {
                    descErrorInsertSignature = "File Name is empty";
                    return Json(new
                    {
                        statusResultInsertSignature = statusResultInsertSignature,
                        descErrorInsertSignature = descErrorInsertSignature,
                        signedFileName = signedFileName
                    }, JsonRequestBehavior.AllowGet);
                }

                byte[] signByteArray = Convert.FromBase64String(signatureBase64);
                if (signByteArray == null || signByteArray.Length == 0)
                {
                    descErrorInsertSignature = "Định dạng chữ ký không hợp lệ";
                    return Json(new
                    {
                        statusResultInsertSignature = statusResultInsertSignature,
                        descErrorInsertSignature = descErrorInsertSignature,
                        signedFileName = signedFileName
                    }, JsonRequestBehavior.AllowGet);
                }

                var session = System.Web.HttpContext.Current.Session;

                if (session["pdfSig"] != null)
                {
                    try
                    {
                        pdfSig = session["pdfSig"] as SignPdfFile;
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"{DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy")} InsertSignatrue {ex.Message} {JsonConvert.SerializeObject(ex)}");
                        descErrorInsertSignature = "Không tìm thấy phiên làm việc";
                        return Json(new
                        {
                            statusResultInsertSignature = statusResultInsertSignature,
                            descErrorInsertSignature = descErrorInsertSignature,
                            signedFileName = signedFileName
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                if (pdfSig == null)
                {
                    descErrorInsertSignature = "Không tìm thấy phiên làm việc";
                    return Json(new
                    {
                        statusResultInsertSignature = statusResultInsertSignature,
                        descErrorInsertSignature = descErrorInsertSignature,
                        signedFileName = signedFileName
                    }, JsonRequestBehavior.AllowGet);
                }

                string HCC_FILE_PDF = GetUrlFileDefaut();

                string[] stringSeparators = new string[] { ".pdf" };
                String[] chainBase64 = fileName.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                String name = fileName.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries)[0] + "_signed";
                String ext = "pdf";

                bool exists = System.IO.File.Exists(Path.Combine(HCC_FILE_PDF, name + "." + ext));
                if (exists)
                {
                    int index = 1;
                    String name_2 = name + "_" + index + "." + "_signed_1";
                    String path = Path.Combine(HCC_FILE_PDF, name_2 + "." + ext);
                    while (System.IO.File.Exists(path))
                    {
                        index++;
                        name_2 = name + "_" + index + "." + "_signed_2";
                        path = Path.Combine(HCC_FILE_PDF, name_2 + "." + ext);
                    }
                    name = name_2;
                }

                TimestampConfig timestampConfig = new TimestampConfig();
                timestampConfig.UseTimestamp = false;
                if (!pdfSig.insertSignature(signatureBase64, Path.Combine(HCC_FILE_PDF, name + "." + ext), timestampConfig))
                {
                    descErrorInsertSignature = "Insert signature into file fail.";
                    return Json(new
                    {
                        statusResultInsertSignature = statusResultInsertSignature,
                        descErrorInsertSignature = descErrorInsertSignature,
                        signedFileName = signedFileName
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    #region lưu đường dẫn file rác
                    var filecu = _fileKyRepos.FirstOrDefault(x => x.DuongDanTep == fileName);
                    if (filecu != null)
                    {
                        filecu.DaSuDung = false;
                        _fileKyRepos.Update(filecu);

                        #region lưu đường dẫn file đã ký
                        var fileKy = new LogFileKy
                        {
                            HoSoId = filecu.HoSoId,
                            ThuTucId = filecu.ThuTucId,
                            DuongDanTep = name + "." + ext,
                            LoaiTepDinhKem = filecu.LoaiTepDinhKem,
                            DaSuDung = true
                        };
                        _fileKyRepos.Insert(fileKy);
                        #endregion
                    }
                    #endregion

                    statusResultInsertSignature = "success";
                    signedFileName = name + "." + ext;
                }

                return Json(new
                {
                    statusResultInsertSignature = statusResultInsertSignature,
                    descErrorInsertSignature = descErrorInsertSignature,
                    signedFileName = signedFileName
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.Error($"{DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy")} InsertSignatrue {ex.Message} {JsonConvert.SerializeObject(ex)}");
                descErrorInsertSignature = "Lỗi trong quá trình xử lý";
                return Json(new
                {
                    statusResultInsertSignature = statusResultInsertSignature,
                    descErrorInsertSignature = descErrorInsertSignature,
                    signedFileName = signedFileName
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}