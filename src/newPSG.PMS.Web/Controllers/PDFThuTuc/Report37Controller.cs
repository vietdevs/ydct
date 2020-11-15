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

using XHoSo = newPSG.PMS.EntityDB.HoSo37;
using XHoSoDto = newPSG.PMS.Dto.HoSo37Dto;
using XBienBanThamDinhChiTietDto = newPSG.PMS.Dto.BienBanThamDinhChiTiet37Dto;
using XHoSoTepDinhKem = newPSG.PMS.EntityDB.HoSoTepDinhKem37;
using newPSG.PMS.Services;
using static newPSG.PMS.CommonENum;
using System.Text;
using System.Drawing;

#endregion Class Riêng Cho Từng Thủ tục

namespace newPSG.PMS.Web.Controllers.Report
{
    //[AbpAuthorize]
    public class Report37Controller : PMSControllerBase
    {
        // GET: Report
        private readonly IRepository<HoSoXuLy37, long> _hoSoXuLyRepos;
        private readonly IRepository<DoanhNghiep, long> _doanhNghiepRepos;
        private readonly IRepository<Tinh> _tinhRepos;
        private readonly IRepository<User, long> _userRepos;
        private readonly IRepository<ChuKy, long> _chuKyRepos;
        private readonly IRepository<LogFileKy, long> _fileKyRepos;
        private readonly ICapSoThuTucAppService _capSoThuTucAppService;
        private readonly IRepository<TT37_PhamViHoatDong> _phamViHoatDongRepos;
        private readonly IRepository<TT37_HoSoPhamViHD, long> _hosoPhamViHDRepos;
        private readonly IRepository<XHoSo, long> _hoSoRepos;
        private readonly IRepository<HoSoTepDinhKem37, long> _hoSoTepDinhKemRepos;
        private readonly IAbpSession _abpSession;
        private readonly IRepository<PhongBan> _phongBanRepos;
        public Report37Controller(IRepository<HoSoXuLy37, long> hoSoXuLyRepos,
                                  IRepository<DoanhNghiep, long> doanhNghiepRepos,
                                  IRepository<Tinh> tinhRepos,
                                  IRepository<User, long> userRepos,
                                  IRepository<ChuKy, long> chuKyRepos,
                                  IRepository<LogFileKy, long> fileKyRepos,
                                  IRepository<TT37_PhamViHoatDong> phamViHoatDongRepos,
                                  IRepository<TT37_HoSoPhamViHD, long> hosoPhamViHDRepos,
                                  IRepository<XHoSo, long> hoSoRepos,
                                  ICapSoThuTucAppService capSoThuTucAppService,
                                  IRepository<HoSoTepDinhKem37, long> hoSoTepDinhKemRepos,
                                  IAbpSession abpSession,
                                  IRepository<PhongBan> phongBanRepos)
        {
            _hoSoXuLyRepos = hoSoXuLyRepos;
            _doanhNghiepRepos = doanhNghiepRepos;
            _tinhRepos = tinhRepos;
            _userRepos = userRepos;
            _chuKyRepos = chuKyRepos;
            _fileKyRepos = fileKyRepos;
            _phamViHoatDongRepos = phamViHoatDongRepos;
            _hosoPhamViHDRepos = hosoPhamViHDRepos;
            _hoSoRepos = hoSoRepos;
            _capSoThuTucAppService = capSoThuTucAppService;
            _hoSoTepDinhKemRepos = hoSoTepDinhKemRepos;
            _abpSession = abpSession;
            _phongBanRepos = phongBanRepos;
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
                string DuongDan = "~/PDFTemplate/ThuTuc_37/rptHoSo.xlsx";
                var path = Server.MapPath(DuongDan);
                fr.SetValue("HoVaTen", hoSo.TenNguoiDaiDien);
                fr.SetValue("NgaySinh", hoSo.NgaySinh.Value.ToShortDateString());
                fr.SetValue("DiaChi", hoSo.DiaChi);
                fr.SetValue("Id", hoSo.MaSoThue);
                fr.SetValue("NgayCap", hoSo.NgayCap.Value.ToShortDateString());
                fr.SetValue("NoiCap", hoSo.NoiCap);
                fr.SetValue("DienThoai", hoSo.SoDienThoai);
                fr.SetValue("Email", hoSo.Email);
                string vanBangChuyenMonStr = GetEnumDescription((CommonENum.VAN_BANG_CHUYEN_MON)hoSo.VanBangChuyenMon);
                fr.SetValue("VanBangChuyenMon", vanBangChuyenMonStr);

                var listPhamViHoatDongDeNghi = (from hoso in _hoSoRepos.GetAll()
                                                join hspv in _hosoPhamViHDRepos.GetAll()
                on hoso.Id equals hspv.HoSoId
                                                join pv in _phamViHoatDongRepos.GetAll()
                  on hspv.PhamViHoatDongId equals pv.Id
                                                where hoso.Id == hoSo.Id
                                                select new
                                                {
                                                    pv.Ten,
                                                    pv.Chung.Name
                                                }).ToList();
                StringBuilder sb = new StringBuilder();
                foreach (var item in listPhamViHoatDongDeNghi)
                {
                    sb.Append(item.Ten + ", ");
                }
                fr.SetValue("PhamViHoatDong", sb.ToString());

                // phạm vi hoạt động
                var _list = new List<ItemObj<int>>();
                foreach (object iEnumItem in Enum.GetValues(typeof(TT37_LOAI_TAI_LIEU_DINH_KEM)))
                {
                    int iEnum = Convert.ToInt32(iEnumItem);
                    _list.Add(new ItemObj<int>
                    {
                        Id = (int)iEnumItem,
                        Name = GetEnumDescription((Enum)Enum.ToObject(typeof(TT37_LOAI_TAI_LIEU_DINH_KEM), iEnum))
                    });
                }

                var lisTepDinhKem = _hoSoTepDinhKemRepos.GetAll().Where(x => x.HoSoId == hoSo.Id).ToList();

                List<TaiLieuDinhKemReportTT37Dto> taiLieuReport = new List<TaiLieuDinhKemReportTT37Dto>();
                var index = 1;
                foreach (var item in lisTepDinhKem)
                {
                    taiLieuReport.Add(new TaiLieuDinhKemReportTT37Dto
                    {
                        Name = item.MoTaTep,
                        STT = index
                    });
                    index++;
                }

                fr.AddTable("ListTaiLieuForReport", taiLieuReport);


                var doanhnghiep = _doanhNghiepRepos.Get(hoSo.DoanhNghiepId.Value);
                var tinh = _tinhRepos.Get(doanhnghiep.TinhId.Value);
                string NgayKy = tinh.Ten + ", ngày " + hoSo.NgayDeNghi.Value.Day + " tháng " + hoSo.NgayDeNghi.Value.Month + " năm " + hoSo.NgayDeNghi.Value.Year;
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
                        var folder = $"{MA_THU_TUC.THU_TUC_37}\\{strTinh}\\{maSoThue}\\{hoSo.StrThuMucHoSo}\\HoSo\\";

                        if (!Directory.Exists(Path.Combine(HCC_FILE_PDF, folder)))
                        {
                            Directory.CreateDirectory(Path.Combine(HCC_FILE_PDF, folder));
                        }

                        var filename = Path.Combine(folder, @"HoSo_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + Guid.NewGuid() + "." + ext);

                        var name = (Path.Combine(HCC_FILE_PDF, filename));
                        System.IO.File.WriteAllBytes(name, outStream.ToArray());

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

        public ExcelFile FilePhieuTiepNhan(PhieuTiepNhanInputDto input)
        {
            ExcelFile xls = null;
            XlsFile Result = new XlsFile(true);
            try
            {
                var hoso = _hoSoRepos.Get(input.HoSoId);
                var hsxl = _hoSoXuLyRepos.Get(hoso.HoSoXuLyId_Active.Value);
                var fr = new BaseFlexCelReport();
                string DuongDan = "~/PDFTemplate/ThuTuc_37/PhieuTiepNhan.xlsx";
                var path = Server.MapPath(DuongDan);

                if (input.ListTaiLieuDaNhan.Count > 0)
                {
                    fr.AddTable("ListTaiLieuDaNhan", input.ListTaiLieuDaNhan);
                }
                fr.SetValue("HoVaTen", hoso.TenNguoiDaiDien);
                fr.SetValue("DiaChi", hoso.DiaChi);
                fr.SetValue("DienThoai", hoso.SoDienThoai);
                fr.SetValue("NgayHen", input.NgayHenCap.Value.ToShortDateString());
                var soTiepNhan = Utility.GetCodeRandom();
                fr.SetValue("SoGiayTiepNhan", soTiepNhan);

                hsxl.SoGiayTiepNhan = soTiepNhan;
                _hoSoXuLyRepos.Update(hsxl);
                var listHoSoXyLyBoSung = _hoSoXuLyRepos.GetAll().Where(x => x.HoSoId == hoso.Id && x.TrangThaiXuLy == (int)CommonENum.KET_QUA_XU_LY.BO_SUNG).ToList();
                List<TiepNhanBoSungReport37Dto> ListTiepNhanBoSung = new List<TiepNhanBoSungReport37Dto>();
                if (listHoSoXyLyBoSung.Count > 0)
                {
                    if (listHoSoXyLyBoSung.Count == 1) // lần đầu nộp bổ xung
                    {
                        ListTiepNhanBoSung.Add(new TiepNhanBoSungReport37Dto()
                        {
                            SoLanBoSung = listHoSoXyLyBoSung.Count,
                            NgayTraGiayTiepNhanStr = "ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year
                        });
                    }
                    else
                    {
                        for (int i = 0; i < listHoSoXyLyBoSung.Count; i++)
                        {
                            var NguoiNhan = _userRepos.Get(listHoSoXyLyBoSung[i].VanThuId.Value);
                            var tiepNhanBoSungReport37Dto = new TiepNhanBoSungReport37Dto()
                            {
                                SoLanBoSung = i + 1,
                                NgayTraGiayTiepNhanStr = listHoSoXyLyBoSung[i].NgayTraGiayTiepNhan.Value.ToShortDateString()
                            };
                            ListTiepNhanBoSung.Add(tiepNhanBoSungReport37Dto);
                        }
                        ListTiepNhanBoSung.Add(new TiepNhanBoSungReport37Dto()
                        {
                            SoLanBoSung = listHoSoXyLyBoSung.Count + 1,
                            NgayTraGiayTiepNhanStr = DateTime.Now.ToShortDateString()
                        });
                    }

                }
                fr.AddTable("ListTiepNhanBoSung", ListTiepNhanBoSung);

                var currentUser = _userRepos.Get(_abpSession.UserId.Value);
                var day = String.Empty;
                var month = String.Empty;
                if (DateTime.Today.Day <= 9)
                {
                    day = string.Format("0{0}", DateTime.Today.Day);
                }
                else
                {
                    day = DateTime.Today.Day.ToString();
                }
                if (DateTime.Today.Month <= 9)
                {
                    month = string.Format("0{0}", DateTime.Today.Month);
                }
                else
                {
                    month = DateTime.Today.Month.ToString();
                }
                string NgayKy = "Hà nội, ngày " + DateTime.Today.Day + " tháng " + DateTime.Today.Month + " năm " + DateTime.Today.Year;
                if (currentUser.TinhId.HasValue)
                {
                    var tinh = _tinhRepos.Get(currentUser.TinhId.Value);
                    NgayKy = tinh.Ten + ", ngày " + DateTime.Today.Day + " tháng " + DateTime.Today.Month + " năm " + DateTime.Today.Year;
                }
                fr.SetValue("NgayKy", NgayKy);

                var nguoiKyObj = _userRepos.Get(_abpSession.UserId.Value);
                fr.SetValue("NguoiKy", "");
                if (nguoiKyObj != null)
                {
                    fr.SetValue("NguoiKy", nguoiKyObj.Surname + " " + nguoiKyObj.Name);
                }

                Result.Open(path);
                fr.Run(Result);
                fr.Dispose();
                xls = Result;

                return xls;
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                xls = ExportExcelError(ex.Message);
                return xls;
            }
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult InsertPDFPhieuTiepNhan(PhieuTiepNhanInputDto input)
        {
            try
            {
                var hoSo = _hoSoRepos.Get(input.HoSoId);
                string HCC_FILE_PDF = GetUrlFileDefaut();
                var xls = FilePhieuTiepNhan(input);
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
                        var folder = $"{MA_THU_TUC.THU_TUC_37}\\{strTinh}\\{maSoThue}\\{hoSo.StrThuMucHoSo}\\phieutiepnhan\\";

                        if (!Directory.Exists(Path.Combine(HCC_FILE_PDF, folder)))
                        {
                            Directory.CreateDirectory(Path.Combine(HCC_FILE_PDF, folder));
                        }

                        var filename = Path.Combine(folder, @"HoSo_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + Guid.NewGuid() + "." + ext);

                        var name = (Path.Combine(HCC_FILE_PDF, filename));
                        System.IO.File.WriteAllBytes(name, outStream.ToArray());

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

        #endregion ------------ Van Thu Xu Ly PDF Da Ky ---------

        #region ----------------- Công văn hồ sơ đạt------------------

        public ExcelFile FileCongVanHoSoDat(ReportTT37Dto input)
        {
            ExcelFile xls = null;
            try
            {
                var dto = (from hoso in _hoSoRepos.GetAll()
                           join hsxl in _hoSoXuLyRepos.GetAll() on hoso.HoSoXuLyId_Active equals hsxl.Id
                           join dn in _doanhNghiepRepos.GetAll() on hoso.DoanhNghiepId equals dn.Id
                           where hoso.Id == input.HoSoId
                           select new CongVanReport37Dto()
                           {
                               ThuTucId = hoso.ThuTucId,
                               HoSoXuLyId = hsxl.Id,
                               Nam = DateTime.Now.Year,
                               CoQuanTiepNhan = "CỤC QUẢN LÝ Y, DƯỢC BỘ Y TẾ",
                               ChuyenVienId = hsxl.ChuyenVienThuLyId,
                               TruongPhongId = hsxl.TruongPhongId,
                               NoiDungCongVan = hsxl.NoiDungCV,
                               ChiCucId = hoso.ChiCucId,
                               TrangThaiXuLy = hsxl.TrangThaiXuLy,
                               LanhDaoCucId = hsxl.LanhDaoCucId,
                               HoSoIsDat = hsxl.HoSoIsDat,
                               SoDangKy = hoso.SoDangKy,
                               MaHoSo = hoso.MaHoSo,
                               PhongBanId = hoso.PhongBanId,
                               TruongPhongNgayKy = hsxl.TruongPhongNgayKy,

                               TruongPhongDaDuyet = hsxl.TruongPhongDaDuyet,

                               TenDoanhNghiep = hoso.TenNguoiDaiDien,
                               SoDienThoai = hoso.SoDienThoai,
                               DiaChiCoSo = hoso.DiaChi,
                               Email = hoso.Email,

                           }).FirstOrDefault();


                if (!string.IsNullOrEmpty(input.NoiDungCV))
                {
                    dto.NoiDungCongVan = input.NoiDungCV;
                }
                if (!string.IsNullOrEmpty(dto.NoiDungCongVan))
                {
                    dto.NoiDungCongVan = dto.NoiDungCongVan.Replace("<br>", "");
                }
                if (!string.IsNullOrEmpty(input.NoiDungCV))
                {
                    dto.NoiDungCongVan = input.NoiDungCV;
                }
                if (!string.IsNullOrEmpty(dto.NoiDungCongVan))
                {
                    dto.NoiDungCongVan = dto.NoiDungCongVan.Replace("<br>", "");
                }
                if (!string.IsNullOrEmpty(input.SoCongVan))
                {
                    dto.SoCongVan = input.SoCongVan;
                }
                if (input.NgayCongVan.HasValue)
                {
                    dto.NgayCongVan = input.NgayCongVan;
                }
                if (!string.IsNullOrEmpty(input.TenNguoiDaiDien))
                {
                    dto.TenDoanhNghiep = input.TenNguoiDaiDien;
                }
                if (!string.IsNullOrEmpty(input.DiaChiCoSo))
                {
                    dto.DiaChiCoSo = input.DiaChiCoSo;
                }
                if (!string.IsNullOrEmpty(input.SoDienThoai))
                {
                    dto.SoDienThoai = input.SoDienThoai;
                }
                if (!string.IsNullOrEmpty(input.Email))
                {
                    dto.Email = input.Email;
                }
                List<CongVan> listNoiDung = new List<CongVan>();
                listNoiDung = XuLyVanBan_New(dto.NoiDungCongVan);

                var fr = new BaseFlexCelReport();

                fr.SetValue("SoCongVan", dto.SoCongVan);
                fr.SetValue("HoVaTen", dto.TenDoanhNghiep);
                fr.AddTable("ListNoiDung", listNoiDung);
                fr.SetValue("NoiDungYeuCau", dto.NoiDungYeuCauGiaiQuyet);
                fr.SetValue("DiaChi", dto.DiaChiCoSo);
                fr.SetValue("DienThoai", dto.SoDienThoai);
                fr.SetValue("Email", dto.Email);

                using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
                {

                    var modelTruongPhong = _userRepos.FirstOrDefault(x => x.Id == dto.TruongPhongId);
                    if (modelTruongPhong != null)
                    {
                        dto.TenTruongPhong = modelTruongPhong.Surname + " " + modelTruongPhong.Name;
                    }

                    var modelChuyenVien = _userRepos.FirstOrDefault(x => x.Id == dto.ChuyenVienId);
                    if (modelChuyenVien != null)
                    {
                        dto.TenChuyenVien = modelChuyenVien.Surname + " " + modelChuyenVien.Name;
                    }
                    var modelLanhDao = _userRepos.FirstOrDefault(x => x.Id == dto.LanhDaoCucId);
                    if (modelLanhDao != null)
                    {
                        dto.TenLanhDaoCuc = modelLanhDao.Surname + " " + modelLanhDao.Name;
                    }
                }


                string DuongDan = "~/PDFTemplate/ThuTuc_37/rptCongVanDat.xlsx";
                var path = Server.MapPath(DuongDan);
                if (dto.NgayCongVan.HasValue)
                {
                    fr.SetValue("NgayCongVan", "Hà Nội, ngày " + dto.NgayCongVan.Value.ToString("dd") + " tháng " + dto.NgayCongVan.Value.ToString("MM") + " năm " + dto.NgayCongVan.Value.ToString("yyyy"));
                }
                else
                {
                    fr.SetValue("NgayCongVan", "Hà Nội, ngày " + DateTime.Now.ToString("dd") + " tháng " + DateTime.Now.ToString("MM") + " năm " + DateTime.Now.ToString("yyyy"));
                }

                if (!string.IsNullOrEmpty(dto.TenLanhDaoCuc))
                {
                    fr.SetValue("NguoiKy", dto.TenLanhDaoCuc);
                }
                else
                {
                    fr.SetValue("NguoiKy", "");
                }

                fr.SetValue("ChanKy", "");
                //Lấy ra các loại chữ ký
                var chuKy = _chuKyRepos.FirstOrDefault(x => x.LoaiChuKy == (int)CommonENum.LOAI_CHU_KY.CHU_KY && x.IsActive == true && x.UserId == dto.LanhDaoCucId); //(from t in _chuKyRepos.GetAll() where t.LoaiChuKy == (int)CommonENum.LOAI_CHU_KY.CHU_KY && t.IsActive == true && t.UserId == dto.TruongPhongId select t).ToList();
                if (chuKy != null)
                {
                    fr.SetValue("ChanKy", chuKy.ChanChuKy);
                }

                XlsFile Result = new XlsFile(true);
                Result.Open(path);
                fr.Run(Result);
                fr.Dispose();
                xls = Result;

                #region Cấu hình footer
                THeaderAndFooter headerAndFooter = new THeaderAndFooter();
                headerAndFooter.DefaultFooter = string.Format("&LMA HO SO: {0}", dto.MaHoSo);
                headerAndFooter.AlignMargins = true;
                xls.SetPageHeaderAndFooter(headerAndFooter);
                #endregion Cấu hình footer

                xls.AutoPageBreaks();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
            }
            return xls;
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult GoToViewCongVanHoSoDat(ReportTT37Dto input)
        {
            ExcelFile xls = null;
            try
            {
                if (input.HoSoId > 0)
                {
                    xls = FileCongVanHoSoDat(input);
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                xls = ExportExcelError();
            }
            return ViewReport(xls, "rptCongVanDat.xls", false);
        }

        #endregion

        #region ----------------- Công văn từ chối ------------------

        public ExcelFile FileCongVanHoSoTuChoi(ReportTT37Dto input)
        {
            ExcelFile xls = null;
            try
            {
                var dto = (from hoso in _hoSoRepos.GetAll()
                           join hsxl in _hoSoXuLyRepos.GetAll() on hoso.HoSoXuLyId_Active equals hsxl.Id
                           join dn in _doanhNghiepRepos.GetAll() on hoso.DoanhNghiepId equals dn.Id
                           where hoso.Id == input.HoSoId
                           select new CongVanReport37Dto()
                           {
                               ThuTucId = hoso.ThuTucId,
                               HoSoXuLyId = hsxl.Id,
                               Nam = DateTime.Now.Year,
                               CoQuanTiepNhan = "CỤC QUẢN LÝ Y, DƯỢC BỘ Y TẾ",
                               ChuyenVienId = hsxl.ChuyenVienThuLyId,
                               TruongPhongId = hsxl.TruongPhongId,
                               NoiDungCongVan = hsxl.NoiDungCV,
                               ChiCucId = hoso.ChiCucId,
                               TrangThaiXuLy = hsxl.TrangThaiXuLy,
                               LanhDaoCucId = hsxl.LanhDaoCucId,
                               HoSoIsDat = hsxl.HoSoIsDat,
                               SoDangKy = hoso.SoDangKy,
                               MaHoSo = hoso.MaHoSo,
                               PhongBanId = hoso.PhongBanId,
                               TruongPhongNgayKy = hsxl.TruongPhongNgayKy,

                               TruongPhongDaDuyet = hsxl.TruongPhongDaDuyet,

                               TenDoanhNghiep = hoso.TenNguoiDaiDien,
                               SoDienThoai = hoso.SoDienThoai,
                               DiaChiCoSo = hoso.DiaChi,
                               Email = hoso.Email,

                           }).FirstOrDefault();


                if (!string.IsNullOrEmpty(input.NoiDungCV))
                {
                    dto.NoiDungCongVan = input.NoiDungCV;
                }
                if (!string.IsNullOrEmpty(dto.NoiDungCongVan))
                {
                    dto.NoiDungCongVan = dto.NoiDungCongVan.Replace("<br>", "");
                }
                if (!string.IsNullOrEmpty(input.SoCongVan))
                {
                    dto.SoCongVan = input.SoCongVan;
                }
                if (input.NgayCongVan.HasValue)
                {
                    dto.NgayCongVan = input.NgayCongVan;
                }
                if (!string.IsNullOrEmpty(input.TenNguoiDaiDien))
                {
                    dto.TenDoanhNghiep = input.TenNguoiDaiDien;
                }
                if (!string.IsNullOrEmpty(input.DiaChiCoSo))
                {
                    dto.DiaChiCoSo = input.DiaChiCoSo;
                }
                if (!string.IsNullOrEmpty(input.SoDienThoai))
                {
                    dto.SoDienThoai = input.SoDienThoai;
                }
                if (!string.IsNullOrEmpty(input.Email))
                {
                    dto.Email = input.Email;
                }
                List<CongVan> listNoiDung = new List<CongVan>();
                listNoiDung = XuLyVanBan_New(dto.NoiDungCongVan);

                var fr = new BaseFlexCelReport();

                fr.SetValue("SoCongVan", dto.SoCongVan);
                fr.SetValue("HoVaTen", dto.TenDoanhNghiep);
                fr.AddTable("ListNoiDung", listNoiDung);
                fr.SetValue("NoiDungYeuCau", dto.NoiDungYeuCauGiaiQuyet);
                fr.SetValue("DiaChi", dto.DiaChiCoSo);
                fr.SetValue("DienThoai", dto.SoDienThoai);
                fr.SetValue("Email", dto.Email);


                using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
                {

                    var modelTruongPhong = _userRepos.FirstOrDefault(x => x.Id == dto.TruongPhongId);
                    if (modelTruongPhong != null)
                    {
                        dto.TenTruongPhong = modelTruongPhong.Surname + " " + modelTruongPhong.Name;
                    }

                    var modelChuyenVien = _userRepos.FirstOrDefault(x => x.Id == dto.ChuyenVienId);
                    if (modelChuyenVien != null)
                    {
                        dto.TenChuyenVien = modelChuyenVien.Surname + " " + modelChuyenVien.Name;
                    }
                    var modelLanhDao = _userRepos.FirstOrDefault(x => x.Id == dto.LanhDaoCucId);
                    if (modelLanhDao != null)
                    {
                        dto.TenLanhDaoCuc = modelLanhDao.Surname + " " + modelLanhDao.Name;
                    }
                }


                string DuongDan = "~/PDFTemplate/ThuTuc_37/rptCongVanTuChoi.xlsx";
                var path = Server.MapPath(DuongDan);
                if (dto.NgayCongVan.HasValue)
                {
                    fr.SetValue("NgayCongVan", "Hà Nội, ngày " + dto.NgayCongVan.Value.ToString("dd") + " tháng " + dto.NgayCongVan.Value.ToString("MM") + " năm " + dto.NgayCongVan.Value.ToString("yyyy"));
                }

                if (!string.IsNullOrEmpty(dto.TenLanhDaoCuc))
                {
                    fr.SetValue("NguoiKy", dto.TenLanhDaoCuc);
                }
                else
                {
                    fr.SetValue("NguoiKy", "");
                }

                fr.SetValue("ChanKy", "");
                //Lấy ra các loại chữ ký
                var chuKy = _chuKyRepos.FirstOrDefault(x => x.LoaiChuKy == (int)CommonENum.LOAI_CHU_KY.CHU_KY && x.IsActive == true && x.UserId == dto.LanhDaoCucId); //(from t in _chuKyRepos.GetAll() where t.LoaiChuKy == (int)CommonENum.LOAI_CHU_KY.CHU_KY && t.IsActive == true && t.UserId == dto.TruongPhongId select t).ToList();
                if (chuKy != null)
                {
                    fr.SetValue("ChanKy", chuKy.ChanChuKy);
                }

                XlsFile Result = new XlsFile(true);
                Result.Open(path);
                fr.Run(Result);
                fr.Dispose();
                xls = Result;

                #region Cấu hình footer
                THeaderAndFooter headerAndFooter = new THeaderAndFooter();
                headerAndFooter.DefaultFooter = string.Format("&LMA HO SO: {0}", dto.MaHoSo);
                headerAndFooter.AlignMargins = true;
                xls.SetPageHeaderAndFooter(headerAndFooter);
                #endregion Cấu hình footer

                xls.AutoPageBreaks();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
            }
            return xls;
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult GoToViewCongVanHoSoTuChoi(ReportTT37Dto input)
        {
            ExcelFile xls = null;
            try
            {
                if (input.HoSoId > 0)
                {
                    xls = FileCongVanHoSoTuChoi(input);
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                xls = ExportExcelError();
            }
            return ViewReport(xls, "rptCongVanTuChoi.xls", false);
        }

        #endregion

        #region ------------ Công văn yêu cầu bổ sung ---------

        public ExcelFile FileCongVanBoSung(ReportTT37Dto input)
        {
            ExcelFile xls = null;
            try
            {
                var dto = (from hoso in _hoSoRepos.GetAll()
                           join hsxl in _hoSoXuLyRepos.GetAll() on hoso.HoSoXuLyId_Active equals hsxl.Id
                           join dn in _doanhNghiepRepos.GetAll() on hoso.DoanhNghiepId equals dn.Id
                           where hoso.Id == input.HoSoId
                           select new CongVanReport37Dto()
                           {
                               ThuTucId = hoso.ThuTucId,
                               HoSoXuLyId = hsxl.Id,
                               Nam = DateTime.Now.Year,
                               CoQuanTiepNhan = "Ban quản lý an toàn thực phẩm TP Hồ Chí Minh",
                               ChuyenVienId = hsxl.ChuyenVienThuLyId,
                               TruongPhongId = hsxl.TruongPhongId,
                               NoiDungCongVan = hsxl.NoiDungCV,
                               ChiCucId = hoso.ChiCucId,
                               TrangThaiXuLy = hsxl.TrangThaiXuLy,
                               LanhDaoCucId = hsxl.LanhDaoCucId,
                               HoSoIsDat = hsxl.HoSoIsDat,
                               SoDangKy = hoso.SoDangKy,
                               MaHoSo = hoso.MaHoSo,
                               PhongBanId = hoso.PhongBanId,
                               TruongPhongNgayKy = hsxl.TruongPhongNgayKy,

                               TruongPhongDaDuyet = hsxl.TruongPhongDaDuyet,

                               SoCongVan = hsxl.SoCongVan,
                               NgayCongVan = hsxl.NgayYeuCauBoSung,
                               LyDo = hsxl.LyDoYeuCauBoSung,
                               TenCanBoHoTro = hsxl.TenCanBoHoTro,
                               DienThoaiCanBo = hsxl.DienThoaiCanBo,
                               NoiDungYeuCauGiaiQuyet = hsxl.NoiDungYeuCauGiaiQuyet,

                               TenDoanhNghiep = hoso.TenNguoiDaiDien,
                               SoDienThoai = hoso.SoDienThoai,
                               DiaChiCoSo = hoso.DiaChi,
                               Email = hoso.Email,

                           }).FirstOrDefault();


                if (!string.IsNullOrEmpty(input.NoiDungCV))
                {
                    dto.NoiDungCongVan = input.NoiDungCV;
                }
                if (!string.IsNullOrEmpty(dto.NoiDungCongVan))
                {
                    dto.NoiDungCongVan = dto.NoiDungCongVan.Replace("<br>", "");
                }
                if (!string.IsNullOrEmpty(input.SoCongVan))
                {
                    dto.SoCongVan = input.SoCongVan;
                }
                if (input.NgayCongVan.HasValue)
                {
                    dto.NgayCongVan = input.NgayCongVan;
                }
                if (!string.IsNullOrEmpty(input.NoiDungYeuCauGiaiQuyet))
                {
                    dto.NoiDungYeuCauGiaiQuyet = input.NoiDungYeuCauGiaiQuyet;
                }
                if (!string.IsNullOrEmpty(input.LyDo))
                {
                    dto.LyDo = input.LyDo;
                }
                if (!string.IsNullOrEmpty(input.TenCanBoHoTro))
                {
                    dto.TenCanBoHoTro = input.TenCanBoHoTro;
                }
                if (!string.IsNullOrEmpty(input.DienThoaiCanBo))
                {
                    dto.DienThoaiCanBo = input.DienThoaiCanBo;
                }
                if (!string.IsNullOrEmpty(input.TenNguoiDaiDien))
                {
                    dto.TenDoanhNghiep = input.TenNguoiDaiDien;
                }
                if (!string.IsNullOrEmpty(input.DiaChiCoSo))
                {
                    dto.DiaChiCoSo = input.DiaChiCoSo;
                }
                if (!string.IsNullOrEmpty(input.SoDienThoai))
                {
                    dto.SoDienThoai = input.SoDienThoai;
                }
                if (!string.IsNullOrEmpty(input.Email))
                {
                    dto.Email = input.Email;
                }
                List<CongVan> listNoiDung = new List<CongVan>();
                listNoiDung = XuLyVanBan_New(dto.NoiDungCongVan);

                var fr = new BaseFlexCelReport();
                fr.SetValue("SoCongVan", dto.SoCongVan);
                fr.SetValue("HoVaTen", dto.TenDoanhNghiep);
                fr.AddTable("ListNoiDung", listNoiDung);
                fr.SetValue("NoiDungYeuCau", dto.NoiDungYeuCauGiaiQuyet);
                fr.SetValue("DiaChi", dto.DiaChiCoSo);
                fr.SetValue("DienThoai", dto.SoDienThoai);
                fr.SetValue("Email", dto.Email);
                fr.SetValue("LyDo", dto.LyDo);
                fr.SetValue("TenChuyenVien", dto.TenCanBoHoTro);
                fr.SetValue("SoDienThoaiCanBo", dto.DienThoaiCanBo);

                using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
                {

                    var modelTruongPhong = _userRepos.FirstOrDefault(x => x.Id == dto.TruongPhongId);
                    if (modelTruongPhong != null)
                    {
                        dto.TenTruongPhong = modelTruongPhong.Surname + " " + modelTruongPhong.Name;
                    }

                    var modelChuyenVien = _userRepos.FirstOrDefault(x => x.Id == dto.ChuyenVienId);
                    if (modelChuyenVien != null)
                    {
                        dto.TenChuyenVien = modelChuyenVien.Surname + " " + modelChuyenVien.Name;
                    }
                    var modelLanhDao = _userRepos.FirstOrDefault(x => x.Id == dto.LanhDaoCucId);
                    if (modelLanhDao != null)
                    {
                        dto.TenLanhDaoCuc = modelLanhDao.Surname + " " + modelLanhDao.Name;
                    }
                }


                string DuongDan = "~/PDFTemplate/ThuTuc_37/rptCongVanYeuCauBoSung.xlsx";
                var path = Server.MapPath(DuongDan);
                fr.SetValue("NgayCongVan", "Hà Nội, ngày " + DateTime.Now.ToString("dd") + " tháng " + DateTime.Now.ToString("MM") + " năm " + DateTime.Now.ToString("yyyy"));
                if (dto.NgayCongVan.HasValue)
                {
                    fr.SetValue("NgayCongVan", "Hà Nội, ngày " + dto.NgayCongVan.Value.Day + " tháng " + dto.NgayCongVan.Value.Month + " năm " + dto.NgayCongVan.Value.Year);
                }

                if (!string.IsNullOrEmpty(dto.TenLanhDaoCuc))
                {
                    fr.SetValue("NguoiKy", dto.TenLanhDaoCuc);
                }
                else
                {
                    fr.SetValue("NguoiKy", "");
                }

                fr.SetValue("ChanKy", "");
                //Lấy ra các loại chữ ký
                var chuKy = _chuKyRepos.FirstOrDefault(x => x.LoaiChuKy == (int)CommonENum.LOAI_CHU_KY.CHU_KY && x.IsActive == true && x.UserId == dto.LanhDaoCucId); //(from t in _chuKyRepos.GetAll() where t.LoaiChuKy == (int)CommonENum.LOAI_CHU_KY.CHU_KY && t.IsActive == true && t.UserId == dto.TruongPhongId select t).ToList();
                if (chuKy != null)
                {
                    fr.SetValue("ChanKy", chuKy.ChanChuKy);
                }

                XlsFile Result = new XlsFile(true);
                Result.Open(path);
                fr.Run(Result);
                fr.Dispose();
                xls = Result;

                #region Cấu hình footer
                THeaderAndFooter headerAndFooter = new THeaderAndFooter();
                headerAndFooter.DefaultFooter = string.Format("&LMA HO SO: {0}", dto.MaHoSo);
                headerAndFooter.AlignMargins = true;
                xls.SetPageHeaderAndFooter(headerAndFooter);
                #endregion Cấu hình footer

                xls.AutoPageBreaks();
                //xls.Save(@"D:\hcc_ydct_docs\testcongvan.xls");
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
            }
            return xls;
        }

        public ActionResult TemplateCongVan(long hoSoId)
        {
            if (hoSoId > 0)
            {
                var input = new ReportTT37Dto();
                input.HoSoId = hoSoId;
                //File Template
                var xls = FileCongVanBoSung(input);
                return ViewReport(xls, "rptHoSo.xls", false);
            }

            return ExportExcelNotData();
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult GoToViewCongVanBoSung(ReportTT37Dto input)
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

                xls = FileCongVanBoSung(input);
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                xls = ExportExcelError();
            }
            return ViewReport(xls, "rptCongVanBoSung.xls", false);
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult InsertPDFCongVan(ReportTT37Dto input)
        {
            try
            {
                var hoSo = _hoSoRepos.Get(input.HoSoId);
                string HCC_FILE_PDF = GetUrlFileDefaut();
                var xls = FileCongVanBoSung(input);
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
                        var folder = $"{MA_THU_TUC.THU_TUC_37}\\{strTinh}\\{maSoThue}\\{hoSo.StrThuMucHoSo}\\CongVanBoSung\\";

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
        #endregion
    }

    public class TaiLieuDinhKemReportTT37Dto
    {
        public int STT { get; set; }
        public string Name { get; set; }

    }
}