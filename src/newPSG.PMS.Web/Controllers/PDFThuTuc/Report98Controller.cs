using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using FlexCel.Core;
using FlexCel.XlsAdapter;
using iTextSharp.text;
using iTextSharp.text.pdf;
using newPSG.PMS.Authorization.Users;
using newPSG.PMS.Dto;
using newPSG.PMS.EntityDB;
using newPSG.PMS.MultiTenancy;
using newPSG.PMS.Web.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

#region Class Riêng Cho Từng Thủ tục

using XHoSo = newPSG.PMS.EntityDB.HoSo98;
using XHoSoDto = newPSG.PMS.Dto.HoSo98Dto;
using XBienBanThamDinhChiTietDto = newPSG.PMS.Dto.BienBanThamDinhChiTiet98Dto;
using XHoSoTepDinhKem = newPSG.PMS.EntityDB.HoSoTepDinhKem98;
using newPSG.PMS.Services;
using static newPSG.PMS.CommonENum;

#endregion Class Riêng Cho Từng Thủ tục

namespace newPSG.PMS.Web.Controllers.Report
{
    //[AbpAuthorize]
    public class Report98Controller : PMSControllerBase
    {
        // GET: Report
        private readonly IRepository<HoSoXuLy98, long> _hoSoXuLyRepos;
        private readonly IRepository<DoanhNghiep, long> _doanhNghiepRepos;
        private readonly IRepository<Tinh> _tinhRepos;
        private readonly IRepository<User, long> _userRepos;
        private readonly IRepository<ChuKy, long> _chuKyRepos;
        private readonly IRepository<LogFileKy, long> _fileKyRepos;
        private readonly ICapSoThuTucAppService _capSoThuTucAppService;

        private readonly IRepository<XHoSo, long> _hoSoRepos;

        public Report98Controller(IRepository<HoSoXuLy98, long> hoSoXuLyRepos,
                                  IRepository<DoanhNghiep, long> doanhNghiepRepos,
                                  IRepository<Tinh> tinhRepos,
                                  IRepository<User, long> userRepos,
                                  IRepository<ChuKy, long> chuKyRepos,
                                  IRepository<LogFileKy, long> fileKyRepos,
                                  IRepository<XHoSo, long> hoSoRepos,
                                  ICapSoThuTucAppService capSoThuTucAppService)
        {
            _hoSoXuLyRepos = hoSoXuLyRepos;
            _doanhNghiepRepos = doanhNghiepRepos;
            _tinhRepos = tinhRepos;
            _userRepos = userRepos;
            _chuKyRepos = chuKyRepos;
            _fileKyRepos = fileKyRepos;
            _hoSoRepos = hoSoRepos;
            _capSoThuTucAppService = capSoThuTucAppService;
        }

        public ActionResult Index()
        {
            return View();
        }

        #region ------------ File Hồ sơ -----------------

        public ExcelFile FileHoSo(long hoSoId)
        {
            ExcelFile xls = null;
            XlsFile Result = new XlsFile(true);
            try
            {
                var hoSo = _hoSoRepos.Get(hoSoId);
                var fr = new BaseFlexCelReport();
                string DuongDan = "~/PDFTemplate/ThuTuc_98/rptHoSo.xlsx";
                var path = Server.MapPath(DuongDan);
                fr.SetValue("TenCoSo", hoSo.TenCoSo);
                fr.SetValue("DiaChiCoSo", hoSo.DiaChiCoSo);
                fr.SetValue("SoFax", hoSo.Fax);
                fr.SetValue("SoDienThoai", hoSo.SoDienThoai);
                fr.SetValue("SoDangKy", hoSo.SoDangKy);

                var dn = _doanhNghiepRepos.FirstOrDefault(x => x.Id == hoSo.DoanhNghiepId);
                fr.SetValue("TenNguoiDaiDien", dn.TenNguoiDaiDien);

                var doanhnghiep = _doanhNghiepRepos.Get(hoSo.DoanhNghiepId.Value);
                fr.SetValue("TenDoanhNghiep", doanhnghiep.TenDoanhNghiep);
                var tinh = _tinhRepos.Get(doanhnghiep.TinhId.Value);
                string NgayKy = tinh.Ten + ", ngày " + DateTime.Today.Day + " tháng " + DateTime.Today.Month + " năm " + DateTime.Today.Year;
                fr.SetValue("NgayKy", NgayKy);
                Result.Open(path);
                fr.Run(Result);
                fr.Dispose();
                xls = Result;

                #region Cấu hình footer
                THeaderAndFooter headerAndFooter = new THeaderAndFooter();
                headerAndFooter.DefaultFooter = string.Format("&LMA HO SO: {0}", hoSo.MaHoSo);
                headerAndFooter.AlignMargins = true;
                xls.SetPageHeaderAndFooter(headerAndFooter);
                #endregion Cấu hình footer
                return xls;
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                xls = ExportExcelError(ex.Message);
                return xls;
            }
        }

        public ActionResult TemplateHoSo(long hoSoId = 0, bool exportExcel = false)
        {
            if (hoSoId > 0)
            {
                //File Template
                var xls = FileHoSo(hoSoId);
                return ViewReport(xls, "rptHoSo.xls", exportExcel);
            }

            return ExportExcelNotData();
        }

        public ActionResult GoToViewHoSo(long hoSoId = 0, bool exportExcel = false)
        {
            var hoSo = _hoSoRepos.Get(hoSoId);

            if (hoSo != null)
            {
                if (hoSo.IsCA == true)
                {
                    var stream = new MemoryStream();
                    var outStream = new MemoryStream();
                    byte[] byteInfo = stream.ToArray();
                    using (iTextSharp.text.Document document = new iTextSharp.text.Document())
                    using (PdfCopy copy = new PdfCopy(document, outStream))
                    {
                        document.Open();
                        string ATTP_FILE_PDF = GetUrlFileDefaut();
                        string filePath = Path.Combine(ATTP_FILE_PDF, hoSo.DuongDanTepCA);
                        var fileBytes = System.IO.File.ReadAllBytes(filePath);
                        copy.AddDocument(new PdfReader(fileBytes));
                    }

                    return File(outStream.ToArray(), "application/pdf");
                }
                else
                {
                    //File Template
                    var xls = FileHoSo(hoSoId);
                    byte[] fileTemplate = null;
                    using (FlexCel.Render.FlexCelPdfExport pdf = new FlexCel.Render.FlexCelPdfExport())
                    {
                        pdf.Workbook = xls;
                        using (MemoryStream ms = new MemoryStream())
                        {
                            pdf.BeginExport(ms);
                            pdf.ExportAllVisibleSheets(false, "PDF");
                            pdf.EndExport();
                            ms.Position = 0;
                            fileTemplate = ms.ToArray();
                        }
                    }

                    //Copy List File Đính Kèm
                    string ATTP_FILE_PDF = GetUrlFileDefaut();
                    var outStream = new MemoryStream();
                    using (iTextSharp.text.Document document = new iTextSharp.text.Document())
                    using (PdfCopy copy = new PdfCopy(document, outStream))
                    {
                        document.Open();
                        copy.AddDocument(new PdfReader((fileTemplate)));
                    }
                    return File(outStream.ToArray(), "application/pdf");
                }
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult InsertPDFHoSo(long hoSoId)
        {
            try
            {
                var hoSo = _hoSoRepos.Get(hoSoId);
                string HCC_FILE_PDF = GetUrlFileDefaut();
                var xls = FileHoSo(hoSoId);
                using (FlexCel.Render.FlexCelPdfExport pdf = new FlexCel.Render.FlexCelPdfExport())
                {
                    pdf.Workbook = xls;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        pdf.BeginExport(ms);
                        pdf.ExportAllVisibleSheets(false, "PDF");
                        pdf.EndExport();
                        ms.Position = 0;
                        var file = File(ms.ToArray(), "application/pdf");
                        var outStream = new MemoryStream();
                        using (iTextSharp.text.Document document = new iTextSharp.text.Document())
                        using (PdfCopy copy = new PdfCopy(document, outStream))
                        {
                            document.Open();
                            copy.AddDocument(new PdfReader((file.FileContents)));
                        }

                        String ext = "pdf";

                        var doanhnghiep = _doanhNghiepRepos.GetAll().FirstOrDefault(t => t.Id == hoSo.DoanhNghiepId);

                        //lấy tỉnh để tạo thêm thư mục
                        var strTinh = "unknown";
                        if (doanhnghiep != null)
                        {
                            strTinh = !string.IsNullOrEmpty(doanhnghiep.Tinh) ? RemoveUnicode(doanhnghiep.Tinh).ToLower().Trim().Replace(" ", "-") : "unknown";
                        }

                        var maSoThue = hoSo.MaSoThue;
                        var strThuMucHoSo = "HOSO_0";
                        if (!string.IsNullOrEmpty(hoSo.StrThuMucHoSo))
                        {
                            strThuMucHoSo = hoSo.StrThuMucHoSo;
                        }
                        var folder = $"{MA_THU_TUC.THU_TUC_98}\\{strTinh}\\{maSoThue}\\{hoSo.StrThuMucHoSo}\\HoSo\\";

                        if (!Directory.Exists(Path.Combine(HCC_FILE_PDF, folder)))
                        {
                            Directory.CreateDirectory(Path.Combine(HCC_FILE_PDF, folder));
                        }

                        var filename = Path.Combine(folder, @"HoSo_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + Guid.NewGuid() + "." + ext);

                        var name = (Path.Combine(HCC_FILE_PDF, filename));
                        System.IO.File.WriteAllBytes(name, outStream.ToArray());

                        #region lưu đường dẫn file rác
                        var fileKy = new LogFileKy
                        {
                           HoSoId = hoSo.Id,
                           ThuTucId = hoSo.ThuTucId,
                           DuongDanTep = filename,
                           LoaiTepDinhKem = (int)CommonENum.LOAI_FILE_KY.DON_DANG_KY,
                           DaSuDung = false
                        };
                        _fileKyRepos.Insert(fileKy);
                        #endregion

                        return Json(new
                        {
                            fileName = filename
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                return Json(new
                {
                    fileName = ""
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion ------------- File HoSo -----------------

        

       


        #region ------------ Văn thư xử lý PDF đã ký ---------

        public ActionResult GetDuongDanTepGiayCongVan(long hoSoId)
        {
            try
            {
                if (hoSoId > 0)
                {
                    var hoSo = _hoSoRepos.Get(hoSoId);

                    if (hoSo.HoSoXuLyId_Active.HasValue)
                    {
                        var hsxl = _hoSoXuLyRepos.Get(hoSo.HoSoXuLyId_Active.Value);

                        if (!string.IsNullOrEmpty(hsxl.DuongDanTepCA))
                        {
                            return Json(new
                            {
                                status = 1,
                                fileName = hsxl.DuongDanTepCA,
                            }, JsonRequestBehavior.AllowGet);
                        }
                        else if (!string.IsNullOrEmpty(hsxl.GiayTiepNhanCA))
                        {
                            return Json(new
                            {
                                status = 1,
                                fileName = hsxl.GiayTiepNhanCA,
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
            }

            return Json(new
            {
                status = 0,
                fileName = "",
                giayTiepNhanCA = ""
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GoToViewGiayCongVan(long hoSoId, bool exportExcel = false)
        {
            ExcelFile xls = null;
            try
            {
                if (hoSoId > 0)
                {
                    var hoSo = _hoSoRepos.Get(hoSoId);

                    if (hoSo.HoSoXuLyId_Active.HasValue)
                    {
                        var hsxl = _hoSoXuLyRepos.Get(hoSo.HoSoXuLyId_Active.Value);

                        if (!string.IsNullOrEmpty(hsxl.DuongDanTepCA))
                        {
                            var stream = new MemoryStream();
                            var outStream = new MemoryStream();
                            byte[] byteInfo = stream.ToArray();
                            using (Document document = new Document())
                            using (PdfCopy copy = new PdfCopy(document, outStream))
                            {
                                document.Open();
                                string HCC_FILE_PDF = GetUrlFileDefaut();
                                string filePath = Path.Combine(HCC_FILE_PDF, hsxl.DuongDanTepCA);
                                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                                copy.AddDocument(new PdfReader(fileBytes));
                            }

                            return File(outStream.ToArray(), "application/pdf");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                xls = ExportExcelError(ex.Message);
            }

            return ViewReport(xls, "rptGiayChungNhanOrCongVan.xls", exportExcel);
        }

        #endregion ------------ Van Thu Xu Ly PDF Da Ky ---------
    }
}