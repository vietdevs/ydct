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

using XHoSo = newPSG.PMS.EntityDB.HoSo99;
using XHoSoDto = newPSG.PMS.Dto.HoSo99Dto;
using XBienBanThamDinh = newPSG.PMS.EntityDB.BienBanThamDinh99;
using XBienBanThamDinhDto = newPSG.PMS.Dto.BienBanThamDinh99Dto;
using XBienBanThamDinhChiTietDto = newPSG.PMS.Dto.BienBanThamDinhChiTiet99Dto;
using XHoSoTepDinhKem = newPSG.PMS.EntityDB.HoSoTepDinhKem99;
using newPSG.PMS.Services;
using static newPSG.PMS.CommonENum;

#endregion Class Riêng Cho Từng Thủ tục

namespace newPSG.PMS.Web.Controllers.Report
{
    //[AbpAuthorize]
    public class Report99Controller : PMSControllerBase
    {
        // GET: Report
        private readonly IRepository<HoSoXuLy99, long> _hoSoXuLyRepos;
        private readonly IRepository<DoanhNghiep, long> _doanhNghiepRepos;
        private readonly IRepository<XBienBanThamDinh, long> _BienBanThamDinhRepos;
        private readonly IRepository<Tinh> _tinhRepos;
        private readonly IRepository<User, long> _userRepos;
        private readonly IRepository<ChuKy, long> _chuKyRepos;
        private readonly IRepository<LogFileKy, long> _fileKyRepos;
        private readonly ICapSoThuTucAppService _capSoThuTucAppService;

        private readonly IRepository<XHoSo, long> _hoSoRepos;

        public Report99Controller(IRepository<HoSoXuLy99, long> hoSoXuLyRepos,
                                  IRepository<DoanhNghiep, long> doanhNghiepRepos,
                                  IRepository<XBienBanThamDinh, long> BienBanThamDinhRepos,
                                  IRepository<Tinh> tinhRepos,
                                  IRepository<User, long> userRepos,
                                  IRepository<ChuKy, long> chuKyRepos,
                                  IRepository<LogFileKy, long> fileKyRepos,
                                  IRepository<XHoSo, long> hoSoRepos,
                                  ICapSoThuTucAppService capSoThuTucAppService)
        {
            _hoSoXuLyRepos = hoSoXuLyRepos;
            _doanhNghiepRepos = doanhNghiepRepos;
            _BienBanThamDinhRepos = BienBanThamDinhRepos;
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
                string DuongDan = "~/PDFTemplate/ThuTuc_99/rptHoSo.xlsx";
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
                        var folder = $"{MA_THU_TUC.THU_TUC_99}\\{strTinh}\\{maSoThue}\\{hoSo.StrThuMucHoSo}\\HoSo\\";

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

        #region ------------ File Biên bản thẩm định, thẩm xét ------------

        public ExcelFile FileBienBanThamDinh(long hoSoId, string noiDungThamXetJson = "", string yKienChung = "")
        {
            ExcelFile xls = null;

            var hoSoXuLy = (from hs in _hoSoRepos.GetAll()
                            join hsxl in _hoSoXuLyRepos.GetAll() on hs.HoSoXuLyId_Active equals hsxl.Id
                            join r_dn in _doanhNghiepRepos.GetAll() on hs.DoanhNghiepId equals r_dn.Id into tb_dn //Left Join
                            from dn in tb_dn.DefaultIfEmpty()
                            where hs.Id == hoSoId
                            select new XHoSoDto()
                            {
                                ThuTucId = hs.ThuTucId,
                                Id = hs.Id,
                                TenCoSo = hs.TenCoSo,
                                HoSoXuLyId_Active = hs.HoSoXuLyId_Active,
                                SoDangKy = hs.SoDangKy,
                                NgayXacNhanThanhToan = hs.NgayXacNhanThanhToan,
                                TenDoanhNghiep = hs.TenDoanhNghiep,
                                DiaChi = hs.DiaChi,
                                MaHoSo = hs.MaHoSo,
                                NgayGui = hsxl.NgayGui,
                                NgayTiepNhan = hsxl.NgayTiepNhan,
                                IsChiCuc = hs.IsChiCuc,
                                ChiCucId = hs.ChiCucId,
                                NoiDungCV = hsxl.NoiDungCV,
                                ChuyenVienThuLyId = hsxl.ChuyenVienThuLyId,
                                ChuyenVienPhoiHopId = hsxl.ChuyenVienPhoiHopId,
                                LanhDaoCucId = hsxl.LanhDaoCucId,
                                HoSoIsDat = hsxl.HoSoIsDat,
                                QuiTrinh = hsxl.QuiTrinh,
                                BienBanThamDinhId_ChuyenVienThuLy = hsxl.BienBanThamDinhId_ChuyenVienThuLy,
                                BienBanThamDinhId_ChuyenVienPhoiHop = hsxl.BienBanThamDinhId_ChuyenVienPhoiHop,
                                YKienChung = hsxl.YKienChung,
                                ChuyenVienThuLyDaDuyet = hsxl.ChuyenVienThuLyDaDuyet,
                                ChuyenVienPhoiHopDaDuyet = hsxl.ChuyenVienPhoiHopDaDuyet,
                                PhoPhongDaDuyet = hsxl.PhoPhongDaDuyet,
                                TruongPhongDaDuyet = hsxl.TruongPhongDaDuyet,
                                TruongPhongId = hsxl.TruongPhongId,
                                PhoPhongId = hsxl.PhoPhongId,
                                DonViXuLy = hsxl.DonViXuLy
                            }).FirstOrDefault();

            var fr = new BaseFlexCelReport();
            string DuongDan = "";
            DuongDan = "~/PDFTemplate/ThuTuc_99/rptBienBanThamDinh.xlsx";

            #region GET Biên bản thẩm định

            var BienBanThamDinhDto = new XBienBanThamDinhDto();

            BienBanThamDinhDto.ArrNoiDungThamDinh = new List<XBienBanThamDinhChiTietDto>();

            //Biên bản thẩm định - Chuyên viên thụ lý
            if (!string.IsNullOrEmpty(noiDungThamXetJson))
            {
                try
                {
                    BienBanThamDinhDto.ArrNoiDungThamDinh = JsonConvert.DeserializeObject<List<XBienBanThamDinhChiTietDto>>(noiDungThamXetJson);
                }
                catch (Exception ex)
                {
                    Logger.Fatal(ex.Message);
                }
            }
            else
            {
                if (hoSoXuLy.BienBanThamDinhId_ChuyenVienThuLy.HasValue && hoSoXuLy.BienBanThamDinhId_ChuyenVienThuLy.Value > 0)
                {
                    var BienBanThamDinh = _BienBanThamDinhRepos.Get(hoSoXuLy.BienBanThamDinhId_ChuyenVienThuLy.Value);
                    if (BienBanThamDinh != null)
                    {
                        try
                        {
                            BienBanThamDinhDto.ArrNoiDungThamDinh = JsonConvert.DeserializeObject<List<XBienBanThamDinhChiTietDto>>(BienBanThamDinh.NoiDungThamXetJson);
                        }
                        catch (Exception ex)
                        {
                            Logger.Fatal(ex.Message);
                        }
                    }
                }
            }

            #endregion GET Biên bản thẩm định

            var path = Server.MapPath(DuongDan);

            var thuTuc = CommonENum.THU_TUC_ID.THU_TUC_99;
            if (hoSoXuLy.ThuTucId.HasValue)
            {
                thuTuc = (CommonENum.THU_TUC_ID)hoSoXuLy.ThuTucId.Value;
            }
            var maThuTuc = CommonENum.GetEnumDescription(thuTuc);

            #region Bind Data xlsx

            fr.SetValue("TenDoanhNghiep", hoSoXuLy.TenDoanhNghiep);
            fr.SetValue("SoDangKy", hoSoXuLy.SoDangKy);
            fr.SetValue("TenCoSo", hoSoXuLy.TenCoSo);
            fr.SetValue("DiaChiCoSo", hoSoXuLy.DiaChiCoSo);
            fr.SetValue("MaHoSo", hoSoXuLy.MaHoSo);
            fr.SetValue("NgayGui", hoSoXuLy.NgayTiepNhan == null ? "" : hoSoXuLy.NgayTiepNhan.Value.ToString("dd/MM/yyyy"));
            fr.SetValue("NgayTiepNhan", hoSoXuLy.NgayGui == null ? "" : hoSoXuLy.NgayGui.Value.ToString("dd/MM/yyyy"));

            if (BienBanThamDinhDto.ArrNoiDungThamDinh != null)
            {
                //if (BienBanThamDinhDto.ArrNoiDungThamDinh[0].BienBanThamDinhPhoiHop == null)
                //{
                //    BienBanThamDinhDto.ArrNoiDungThamDinh[0].BienBanThamDinhPhoiHop = new BienBanThamDinhPhoiHop99Dto();
                //}
                //fr.AddTable("ChiTiet", BienBanThamDinhDto.ArrNoiDungThamDinh);
            }
            else
            {
                fr.AddTable("ChiTiet", new List<XBienBanThamDinhChiTietDto>());
            }

            fr.SetValue("NgayKy", "Hà Nội, ngày " + DateTime.Now.ToString("dd") + " tháng " + DateTime.Now.ToString("MM") + " năm " + DateTime.Now.ToString("yyyy"));

            bool? IsCopyThamXet = false;
            string YKienCV1 = ""; string YKienCV2 = "";
            if (hoSoXuLy.BienBanThamDinhId_ChuyenVienThuLy.HasValue)
            {
                var bienBan = _BienBanThamDinhRepos.Get(hoSoXuLy.BienBanThamDinhId_ChuyenVienThuLy.Value);
                YKienCV1 = bienBan.YKienBoSung;
            }
            if (hoSoXuLy.BienBanThamDinhId_ChuyenVienPhoiHop.HasValue)
            {
                var bienBan = _BienBanThamDinhRepos.Get(hoSoXuLy.BienBanThamDinhId_ChuyenVienPhoiHop.Value);
                YKienCV2 = bienBan.YKienBoSung;
                IsCopyThamXet = bienBan.IsCopyThamXet;
            }
            fr.SetValue("IsCopyThamXet", IsCopyThamXet);
            fr.SetValue("YKienCV1", FormatYkien(YKienCV1));
            fr.SetValue("YKienCV2", FormatYkien(YKienCV2));
            #region Thông tin ký nháy

            var tenChuyenVienThuLy = "";
            var chuKyNhay_ChuyenVienThuLy = "";
            if (hoSoXuLy.ChuyenVienThuLyId.HasValue)
            {
                using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                    var objUser = _userRepos.FirstOrDefault(hoSoXuLy.ChuyenVienThuLyId.Value);
                    if (objUser != null)
                    {
                        tenChuyenVienThuLy = "CV. " + objUser.Surname + " " + objUser.Name;
                        chuKyNhay_ChuyenVienThuLy = objUser.UrlImageChuKyNhay;
                    }
                    var chuKy = _chuKyRepos.FirstOrDefault(x => x.UserId == hoSoXuLy.ChuyenVienThuLyId.Value && x.IsActive == true);
                    if (chuKy != null)
                    {
                        chuKyNhay_ChuyenVienThuLy = chuKy.UrlImage;
                    }
                }
            }
            fr.SetValue("tenChuyenVienThuLy", tenChuyenVienThuLy.ToUpper());

            var tenChuyenVienPhoiHop = "";
            var chuKyNhay_ChuyenVienPhoiHop = "";
            if (hoSoXuLy.ChuyenVienPhoiHopId.HasValue)
            {
                using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                    var objUser = _userRepos.FirstOrDefault(hoSoXuLy.ChuyenVienPhoiHopId.Value);
                    if (objUser != null)
                    {
                        tenChuyenVienPhoiHop = "CV. " + objUser.Surname + " " + objUser.Name;
                    }
                    var chuKy = _chuKyRepos.FirstOrDefault(x => x.UserId == hoSoXuLy.ChuyenVienPhoiHopId.Value && x.IsActive == true);
                    if (chuKy != null)
                    {
                        chuKyNhay_ChuyenVienPhoiHop = chuKy.UrlImage;
                    }
                }
            }
            fr.SetValue("tenChuyenVienPhoiHop", tenChuyenVienPhoiHop.ToUpper());

            fr.SetValue("tenLanhDaoPhong", "");
            if (hoSoXuLy.PhoPhongId.HasValue && hoSoXuLy.PhoPhongDaDuyet == true)
            {
                using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                    var objUser = _userRepos.FirstOrDefault(hoSoXuLy.PhoPhongId.Value);
                    if (objUser != null)
                    {
                        //chuKyNhay_LanhDaoPhong = objUser.UrlImageChuKyNhay;
                        var tenLanhDaoPhong = objUser.Surname + " " + objUser.Name;
                        fr.SetValue("tenLanhDaoPhong", tenLanhDaoPhong.ToUpper());
                    }
                }
            }
            else if (hoSoXuLy.TruongPhongId.HasValue && hoSoXuLy.TruongPhongDaDuyet == true)
            {
                using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                    var objUser = _userRepos.FirstOrDefault(hoSoXuLy.TruongPhongId.Value);
                    if (objUser != null)
                    {
                        //chuKyNhay_LanhDaoPhong = objUser.UrlImageChuKyNhay;
                        var tenLanhDaoPhong = objUser.Surname + " " + objUser.Name;
                        fr.SetValue("tenLanhDaoPhong", tenLanhDaoPhong.ToUpper());
                    }
                }
            }

            #endregion Thông tin ký nháy

            XlsFile Result = new XlsFile(true);
            Result.Open(path);
            fr.Run(Result);
            fr.Dispose();
            xls = Result;

            #endregion Bind Data xlsx

            #region Ký nháy biên bản thẩm định

            string HCC_FILE_PDF = GetUrlFileDefaut();
            if (!string.IsNullOrWhiteSpace(chuKyNhay_ChuyenVienThuLy) && hoSoXuLy.ChuyenVienThuLyDaDuyet == true)
            {
                var imgChuKy = InitImage(HCC_FILE_PDF, chuKyNhay_ChuyenVienThuLy, 299, 199);
                Result.AddImage(16, 3, imgChuKy);
            }
            if (!string.IsNullOrWhiteSpace(chuKyNhay_ChuyenVienPhoiHop) && hoSoXuLy.ChuyenVienPhoiHopDaDuyet == true)
            {
                var imgChuKy = InitImage(HCC_FILE_PDF, chuKyNhay_ChuyenVienPhoiHop, 299, 199);
                Result.AddImage(16, 7, imgChuKy);
            }

            #endregion Ký nháy biên bản thẩm định

            return xls;
        }

        public ActionResult TemplateBienBanThamDinh(long hoSoId = 0, bool exportExcel = false)
        {
            if (hoSoId > 0)
            {
                //File Template
                var xls = FileBienBanThamDinh(hoSoId, "");
                return ViewReport(xls, "rptBienBanThamDinh.xls", exportExcel);
            }

            return ExportExcelNotData();
        }


        public ActionResult GoToViewBienBanThamDinh(long hoSoId = 0, string jsonThamDinhCoSo = "", string yKienChung = "", bool exportExcel = false)
        {
            ExcelFile xls = null;
            try
            {
                if (string.IsNullOrEmpty(jsonThamDinhCoSo))
                {
                    var hoSo = _hoSoRepos.Get(hoSoId);

                    if (hoSo.HoSoXuLyId_Active.HasValue)
                    {
                        var hsxl = _hoSoXuLyRepos.Get(hoSo.HoSoXuLyId_Active.Value);

                        if (hsxl.HoSoIsDat != true && !string.IsNullOrEmpty(hsxl.DuongDanTepCA))
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

                xls = FileBienBanThamDinh(hoSoId, jsonThamDinhCoSo, yKienChung);
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                xls = ExportExcelError(ex.Message);
            }
            return ViewReport(xls, "rptBienBanThamDinh.xls", exportExcel);
        }

        #endregion ------------ File Bien Ban Tham Dinh ------------

        #region ------------ File Công văn ------------

        public ExcelFile FileCongVan(long hoso_id, bool? hoSoIsDat = null, string noidungcv = "", bool? isKySo = null)
        {
            ExcelFile xls = null;
            try
            {
                var hoSoXuLy = (from hs in _hoSoRepos.GetAll()
                                join hsxl in _hoSoXuLyRepos.GetAll() on hs.HoSoXuLyId_Active equals hsxl.Id
                                join r_dn in _doanhNghiepRepos.GetAll() on hs.DoanhNghiepId equals r_dn.Id into tb_dn //Left Join
                                from dn in tb_dn.DefaultIfEmpty()
                                where hs.Id == hoso_id
                                select new XHoSoDto()
                                {
                                    ThuTucId = hs.ThuTucId,
                                    Id = hs.Id,
                                    HoSoXuLyId_Active = hs.HoSoXuLyId_Active,
                                    SoDangKy = hs.SoDangKy,
                                    NgayXacNhanThanhToan = hs.NgayXacNhanThanhToan,
                                    TenDoanhNghiep = hs.TenDoanhNghiep,
                                    DiaChiCoSo = hs.DiaChiCoSo,
                                    TenCoSo = hs.TenCoSo,
                                    MaHoSo = hs.MaHoSo,
                                    IsChiCuc = hs.IsChiCuc,
                                    ChiCucId = hs.ChiCucId,
                                    NoiDungCV = hsxl.NoiDungCV,
                                    LanhDaoCucId = hsxl.LanhDaoCucId,
                                    HoSoIsDat = hsxl.HoSoIsDat,
                                    DonViXuLy = hsxl.DonViXuLy,
                                    TrangThaiCV = hsxl.TrangThaiCV,
                                    SoDienThoai = hs.SoDienThoai,
                                    Fax = hs.Fax,
                                    TruongPhongId = hsxl.TruongPhongId,
                                }).FirstOrDefault();

                if (!string.IsNullOrEmpty(noidungcv))
                {
                    hoSoXuLy.NoiDungCV = noidungcv;
                }
                if (!string.IsNullOrEmpty(hoSoXuLy.NoiDungCV))
                {
                    hoSoXuLy.NoiDungCV = hoSoXuLy.NoiDungCV.Replace("\n", "<br>");
                }
                var noiDungCongVanObj = xuLyVanBan(hoSoXuLy.NoiDungCV, 95, 36);

                var fr = new BaseFlexCelReport();

                bool? _hoSoIsDat = null;
                if (hoSoIsDat.HasValue)
                {
                    _hoSoIsDat = hoSoIsDat;
                }
                else if (hoSoXuLy.HoSoIsDat.HasValue)
                {
                    _hoSoIsDat = hoSoXuLy.HoSoIsDat.Value;
                }

                string DuongDan = _hoSoIsDat == true ? "~/PDFTemplate/ThuTuc_99/rptGiayChungNhan.xlsx" : "~/PDFTemplate/ThuTuc_99/rptCongVan.xlsx";

                var path = Server.MapPath(DuongDan);

                fr.SetValue("NgayKy", "Hà Nội, ngày " + DateTime.Now.ToString("dd") + " tháng " + DateTime.Now.ToString("MM") + " năm " + DateTime.Now.ToString("yyyy"));

                string soChungNhan = "";
                if (_hoSoIsDat == true)
                {
                    soChungNhan = _capSoThuTucAppService.SinhSoChungNhan(hoso_id, hoSoXuLy.ThuTucId.Value, isKySo == true ? false : true);
                    fr.SetValue("SoCongVan", soChungNhan);
                }
                else
                {
                    int soCongVan = _capSoThuTucAppService.SinhSoCongVan(hoSoXuLy.Id, hoSoXuLy.ThuTucId.Value, isKySo == true ? false : true);
                    string strSoCongVan = string.Format("{0}/{1}/{2}", soCongVan, DateTime.Now.Year, "ATTP-TTe");
                    fr.SetValue("SoCongVan", strSoCongVan);
                }

                fr.SetValue("SoDangKy", hoSoXuLy.SoDangKy);

                fr.SetValue("TenCoSo", hoSoXuLy.TenCoSo);
                fr.SetValue("DiaChiCoSo", hoSoXuLy.DiaChiCoSo);
                fr.SetValue("SoDienThoai", hoSoXuLy.SoDienThoai);
                fr.SetValue("SoFax", hoSoXuLy.Fax);
                fr.SetValue("NoiDungCongVan1", noiDungCongVanObj.NoiDung1);
                fr.SetValue("NoiDungCongVan2", noiDungCongVanObj.NoiDung2);
                fr.SetValue("NoiDungCongVan3", noiDungCongVanObj.NoiDung3);
                fr.SetValue("NoiDungCongVan4", noiDungCongVanObj.NoiDung4);
                fr.SetValue("NoiDungCongVan5", noiDungCongVanObj.NoiDung5);

                fr.SetValue("NguoiKy", "");
                fr.SetValue("ChanKy", "Cục Trưởng");

                using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                    if (hoSoXuLy.LanhDaoCucId.HasValue && hoSoXuLy.LanhDaoCucId.Value > 0)
                    {
                        var userObj = _userRepos.FirstOrDefault(hoSoXuLy.LanhDaoCucId.Value);
                        hoSoXuLy.TenLanhDaoCuc = userObj.Surname + " " + userObj.Name;
                        fr.SetValue("NguoiKy", hoSoXuLy.TenLanhDaoCuc);
                    }
                }
                //Lấy ra các loại chữ ký
                var lstChuKy = (from t in _chuKyRepos.GetAll() where t.LoaiChuKy == 1 && t.IsActive == true && t.UserId == hoSoXuLy.LanhDaoCucId select t).ToList();
                if (lstChuKy.Count > 0)
                {
                    lstChuKy = lstChuKy.Where(t => t.LoaiChuKy == 1 && t.ChanChuKy != "").ToList();
                    if (lstChuKy.Count > 0)
                    {
                        fr.SetValue("ChanKy", lstChuKy[0].ChanChuKy);
                    }
                }

                XlsFile Result = new XlsFile(true);
                Result.Open(path);
                fr.Run(Result);
                fr.Dispose();
                xls = Result;

                #region Cấu hình footer

                //THeaderAndFooter headerAndFooter = new THeaderAndFooter();
                //headerAndFooter.DefaultFooter = string.Format("&LMA HO SO: {0}", hoSoXuLy.MaHoSo);
                //headerAndFooter.AlignMargins = true;
                //xls.SetPageHeaderAndFooter(headerAndFooter);

                #endregion Cấu hình footer

                #region Tạo QR Code
                if (_hoSoIsDat == true)
                {
                    var content = "Tên cơ sở: " + hoSoXuLy.TenCoSo;
                    content = content + "\n" + "Địa chỉ: " + hoSoXuLy.DiaChiCoSo;
                    content = content + "\n" + "Đơn vị cấp: " + "Cục An toàn thực phẩm";
                    content = content + "\n" + "Số chứng nhận: " + soChungNhan;
                    content = content + "\n" + "Ngày cấp:" + DateTime.Now.ToString("dd/MM/yyyy");
                    Result.AddImage(32, 3, QRCode.Encode(content, 120, 120));
                }
                #endregion

                xls.KeepRowsTogether(24, 33, 1, false);
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                xls = ExportExcelError(ex.Message);
            }
            return xls;
        }

        public ActionResult TemplateCongVan(long hoSoId = 0, bool exportExcel = false)
        {
            if (hoSoId > 0)
            {
                //File Template
                var xls = FileCongVan(hoSoId);
                return ViewReport(xls, "rptCongVanBoSung.xls", exportExcel);
            }

            return ExportExcelNotData();
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult GoToViewCongVan(ReportBaseDto input)
        {
            ExcelFile xls = null;
            try
            {
                if (string.IsNullOrEmpty(input.NoiDungCV))
                {
                    var hoSo = _hoSoRepos.Get(input.HoSoId);

                    if (hoSo.HoSoXuLyId_Active.HasValue)
                    {
                        var hsxl = _hoSoXuLyRepos.Get(hoSo.HoSoXuLyId_Active.Value);

                        if (hsxl.HoSoIsDat != true && !string.IsNullOrEmpty(hsxl.DuongDanTepCA))
                        {
                            var stream = new MemoryStream();
                            var outStream = new MemoryStream();
                            byte[] byteInfo = stream.ToArray();
                            using (Document document = new Document())
                            using (PdfCopy copy = new PdfCopy(document, outStream))
                            {
                                document.Open();
                                string ATTP_FILE_PDF = GetUrlFileDefaut();
                                string filePath = Path.Combine(ATTP_FILE_PDF, hsxl.DuongDanTepCA);
                                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                                copy.AddDocument(new PdfReader(fileBytes));
                            }

                            return File(outStream.ToArray(), "application/pdf");
                        }
                    }
                }

                xls = FileCongVan(input.HoSoId, input.HoSoIsDat, input.NoiDungCV);
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                xls = ExportExcelError(ex.Message);
            }
            return ViewReport(xls, "rptCongVanBoSung.xls", input.ExportExcel);
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult InsertPDFCongVan(ReportBaseDto hoSo)
        {
            try
            {
                string HCC_FILE_PDF = GetUrlFileDefaut();

                var xls = FileCongVan(hoSo.HoSoId, null, hoSo.NoiDungCV, true);

                using (FlexCel.Render.FlexCelPdfExport pdf = new FlexCel.Render.FlexCelPdfExport())
                {
                    pdf.Workbook = xls;

                    FileContentResult file = null;
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                    {
                        pdf.BeginExport(ms);
                        pdf.ExportAllVisibleSheets(false, "BaoCao");
                        pdf.EndExport();
                        ms.Position = 0;
                        file = File(ms.ToArray(), "application/pdf");
                    }

                    var _hoSo = _hoSoRepos.Get(hoSo.HoSoId);

                    String ext = "pdf";

                    var doanhnghiep = _doanhNghiepRepos.GetAll().FirstOrDefault(t => t.Id == _hoSo.DoanhNghiepId);

                    //lấy tỉnh để tạo thêm thư mục
                    var strTinh = "unknown";
                    if (doanhnghiep != null)
                    {
                        strTinh = !string.IsNullOrEmpty(doanhnghiep.Tinh) ? RemoveUnicode(doanhnghiep.Tinh).ToLower().Trim().Replace(" ", "-") : "unknown";
                    }

                    var maSoThue = _hoSo.MaSoThue;
                    var strThuMucHoSo = "HOSO_0";
                    if (!string.IsNullOrEmpty(_hoSo.StrThuMucHoSo))
                    {
                        strThuMucHoSo = _hoSo.StrThuMucHoSo;
                    }
                    var folder = $"{MA_THU_TUC.THU_TUC_99}\\{strTinh}\\{maSoThue}\\{_hoSo.StrThuMucHoSo}\\GiayCongVan\\";

                    if (!Directory.Exists(Path.Combine(HCC_FILE_PDF, folder)))
                    {
                        Directory.CreateDirectory(Path.Combine(HCC_FILE_PDF, folder));
                    }

                    var filename = Path.Combine(folder, @"GiayCongVan_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + Guid.NewGuid() + "." + ext);

                    var name = (Path.Combine(HCC_FILE_PDF, filename));

                    System.IO.File.WriteAllBytes(name, file.FileContents);

                    #region lưu đường dẫn file rác
                    var fileKy = new LogFileKy
                    {
                        HoSoId = _hoSo.Id,
                        ThuTucId = _hoSo.ThuTucId,
                        DuongDanTep = filename,
                        LoaiTepDinhKem = (int)CommonENum.LOAI_FILE_KY.CONG_VAN,
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
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                return Json(new
                {
                    fileName = ""
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion ------------ File Công văn ------------

        #region ------------ File Giấy chứng nhận ------------

        public ExcelFile FileGiayChungNhan(long hoso_id, bool? isKySo = null)
        {
            ExcelFile xls = null;
            try
            {
                var hoSoXuLy = (from hs in _hoSoRepos.GetAll()
                                join hsxl in _hoSoXuLyRepos.GetAll() on hs.HoSoXuLyId_Active equals hsxl.Id
                                join r_dn in _doanhNghiepRepos.GetAll() on hs.DoanhNghiepId equals r_dn.Id into tb_dn //Left Join
                                from dn in tb_dn.DefaultIfEmpty()
                                where hs.Id == hoso_id
                                select new XHoSoDto()
                                {
                                    ThuTucId = hs.ThuTucId,
                                    Id = hs.Id,
                                    HoSoXuLyId_Active = hs.HoSoXuLyId_Active,
                                    SoDangKy = hs.SoDangKy,
                                    NgayXacNhanThanhToan = hs.NgayXacNhanThanhToan,
                                    TenDoanhNghiep = hs.TenDoanhNghiep,
                                    DiaChiCoSo = hs.DiaChiCoSo,
                                    TenCoSo = hs.TenCoSo,
                                    MaHoSo = hs.MaHoSo,
                                    IsChiCuc = hs.IsChiCuc,
                                    ChiCucId = hs.ChiCucId,
                                    NoiDungCV = hsxl.NoiDungCV,
                                    LanhDaoCucId = hsxl.LanhDaoCucId,
                                    HoSoIsDat = hsxl.HoSoIsDat,
                                    DonViXuLy = hsxl.DonViXuLy,
                                    TrangThaiCV = hsxl.TrangThaiCV,
                                    SoDienThoai = hs.SoDienThoai,
                                    Fax = hs.Fax,
                                    TruongPhongId = hsxl.TruongPhongId,
                                }).FirstOrDefault();

                var fr = new BaseFlexCelReport();

                string DuongDan = "~/PDFTemplate/ThuTuc_99/rptGiayChungNhan.xlsx";

                var path = Server.MapPath(DuongDan);

                fr.SetValue("NgayKy", "Hà Nội, ngày " + DateTime.Now.ToString("dd") + " tháng " + DateTime.Now.ToString("MM") + " năm " + DateTime.Now.ToString("yyyy"));

                string soChungNhan = _capSoThuTucAppService.SinhSoChungNhan(hoso_id, hoSoXuLy.ThuTucId.Value, isKySo == true ? false : true);
                fr.SetValue("SoCongVan", soChungNhan);

                fr.SetValue("SoDangKy", hoSoXuLy.SoDangKy);

                fr.SetValue("TenCoSo", hoSoXuLy.TenCoSo);
                fr.SetValue("DiaChiCoSo", hoSoXuLy.DiaChiCoSo);
                fr.SetValue("SoDienThoai", hoSoXuLy.SoDienThoai);
                fr.SetValue("SoFax", hoSoXuLy.Fax);

                fr.SetValue("NguoiKy", "");
                fr.SetValue("ChanKy", "Cục Trưởng");

                using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                    if (hoSoXuLy.LanhDaoCucId.HasValue && hoSoXuLy.LanhDaoCucId.Value > 0)
                    {
                        var userObj = _userRepos.FirstOrDefault(hoSoXuLy.LanhDaoCucId.Value);
                        hoSoXuLy.TenLanhDaoCuc = userObj.Surname + " " + userObj.Name;
                        fr.SetValue("NguoiKy", hoSoXuLy.TenLanhDaoCuc);
                    }
                }
                //Lấy ra các loại chữ ký
                var lstChuKy = (from t in _chuKyRepos.GetAll() where t.LoaiChuKy == 1 && t.IsActive == true && t.UserId == hoSoXuLy.LanhDaoCucId select t).ToList();
                if (lstChuKy.Count > 0)
                {
                    lstChuKy = lstChuKy.Where(t => t.LoaiChuKy == 1 && t.ChanChuKy != "").ToList();
                    if (lstChuKy.Count > 0)
                    {
                        fr.SetValue("ChanKy", lstChuKy[0].ChanChuKy);
                    }
                }

                XlsFile Result = new XlsFile(true);
                Result.Open(path);
                fr.Run(Result);
                fr.Dispose();
                xls = Result;

                #region Cấu hình footer

                //THeaderAndFooter headerAndFooter = new THeaderAndFooter();
                //headerAndFooter.DefaultFooter = string.Format("&LMA HO SO: {0}", hoSoXuLy.MaHoSo);
                //headerAndFooter.AlignMargins = true;
                //xls.SetPageHeaderAndFooter(headerAndFooter);

                #endregion Cấu hình footer

                #region Tạo QR Code
                var content = "Tên cơ sở: " + hoSoXuLy.TenCoSo;
                content = content + "\n" + "Địa chỉ: " + hoSoXuLy.DiaChiCoSo;
                content = content + "\n" + "Đơn vị cấp: " + "Cục An toàn thực phẩm";
                content = content + "\n" + "Số chứng nhận: " + soChungNhan;
                content = content + "\n" + "Ngày cấp:" + DateTime.Now.ToString("dd/MM/yyyy");
                Result.AddImage(32, 3, QRCode.Encode(content, 120, 120));
                #endregion

                xls.KeepRowsTogether(24, 33, 1, false);
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                xls = ExportExcelError(ex.Message);
            }
            return xls;
        }

        public ActionResult TemplateGiayChungNhan(long hoSoId = 0, bool exportExcel = false)
        {
            if (hoSoId > 0)
            {
                //File Template
                var xls = FileGiayChungNhan(hoSoId);
                return ViewReport(xls, "rptCongVanBoSung.xls", exportExcel);
            }

            return ExportExcelNotData();
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult GoToViewGiayChungNhan(ReportBaseDto input)
        {
            ExcelFile xls = null;
            try
            {
                if (string.IsNullOrEmpty(input.NoiDungCV))
                {
                    var hoSo = _hoSoRepos.Get(input.HoSoId);

                    if (hoSo.HoSoXuLyId_Active.HasValue)
                    {
                        var hsxl = _hoSoXuLyRepos.Get(hoSo.HoSoXuLyId_Active.Value);

                        if (hsxl.HoSoIsDat != true && !string.IsNullOrEmpty(hsxl.DuongDanTepCA))
                        {
                            var stream = new MemoryStream();
                            var outStream = new MemoryStream();
                            byte[] byteInfo = stream.ToArray();
                            using (Document document = new Document())
                            using (PdfCopy copy = new PdfCopy(document, outStream))
                            {
                                document.Open();
                                string ATTP_FILE_PDF = GetUrlFileDefaut();
                                string filePath = Path.Combine(ATTP_FILE_PDF, hsxl.DuongDanTepCA);
                                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                                copy.AddDocument(new PdfReader(fileBytes));
                            }

                            return File(outStream.ToArray(), "application/pdf");
                        }
                    }
                }

                xls = FileGiayChungNhan(input.HoSoId);
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                xls = ExportExcelError(ex.Message);
            }
            return ViewReport(xls, "rptCongVanBoSung.xls", input.ExportExcel);
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult InsertPDFGiayChungNhan(ReportBaseDto hoSo)
        {
            try
            {
                string HCC_FILE_PDF = GetUrlFileDefaut();

                var xls = FileCongVan(hoSo.HoSoId, null, hoSo.NoiDungCV, true);

                using (FlexCel.Render.FlexCelPdfExport pdf = new FlexCel.Render.FlexCelPdfExport())
                {
                    pdf.Workbook = xls;

                    FileContentResult file = null;
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                    {
                        pdf.BeginExport(ms);
                        pdf.ExportAllVisibleSheets(false, "BaoCao");
                        pdf.EndExport();
                        ms.Position = 0;
                        file = File(ms.ToArray(), "application/pdf");
                    }

                    var _hoSo = _hoSoRepos.Get(hoSo.HoSoId);

                    String ext = "pdf";

                    var doanhnghiep = _doanhNghiepRepos.GetAll().FirstOrDefault(t => t.Id == _hoSo.DoanhNghiepId);

                    //lấy tỉnh để tạo thêm thư mục
                    var strTinh = "unknown";
                    if (doanhnghiep != null)
                    {
                        strTinh = !string.IsNullOrEmpty(doanhnghiep.Tinh) ? RemoveUnicode(doanhnghiep.Tinh).ToLower().Trim().Replace(" ", "-") : "unknown";
                    }

                    var maSoThue = _hoSo.MaSoThue;
                    var strThuMucHoSo = "HOSO_0";
                    if (!string.IsNullOrEmpty(_hoSo.StrThuMucHoSo))
                    {
                        strThuMucHoSo = _hoSo.StrThuMucHoSo;
                    }
                    var folder = $"{MA_THU_TUC.THU_TUC_99}\\{strTinh}\\{maSoThue}\\{_hoSo.StrThuMucHoSo}\\GiayCongVan\\";

                    if (!Directory.Exists(Path.Combine(HCC_FILE_PDF, folder)))
                    {
                        Directory.CreateDirectory(Path.Combine(HCC_FILE_PDF, folder));
                    }

                    var filename = Path.Combine(folder, @"GiayCongVan_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + Guid.NewGuid() + "." + ext);

                    var name = (Path.Combine(HCC_FILE_PDF, filename));
                    System.IO.File.WriteAllBytes(name, file.FileContents);

                    #region lưu đường dẫn file rác
                    var fileKy = new LogFileKy
                    {
                        HoSoId = _hoSo.Id,
                        ThuTucId = _hoSo.ThuTucId,
                        DuongDanTep = filename,
                        LoaiTepDinhKem = (int)CommonENum.LOAI_FILE_KY.GIAY_CHUNG_NHAN,
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
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                return Json(new
                {
                    fileName = ""
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion ------------ File Giấy chứng nhận------------

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