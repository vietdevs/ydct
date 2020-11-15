using Abp.Application.Services;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using newPSG.PMS.Authorization.Users;
using newPSG.PMS.Dto;
using newPSG.PMS.Editions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Net.Mail;

#region Class Riêng Cho Từng Thủ tục

using XHoSo = newPSG.PMS.EntityDB.HoSo37;
using XHoSoTepDinhKem = newPSG.PMS.EntityDB.HoSoTepDinhKem37;
using XHoSoXuLy = newPSG.PMS.EntityDB.HoSoXuLy37;
using XHoSoXuLyHistory = newPSG.PMS.EntityDB.HoSoXuLyHistory37;
using XHoSoDto = newPSG.PMS.Dto.HoSo37Dto;
using XHoSoInputDto = newPSG.PMS.Dto.HoSoInput37Dto;
using XHoSoTepDinhKemDto = newPSG.PMS.Dto.HoSoTepDinhKem37Dto;
using XHoSoXuLyDto = newPSG.PMS.Dto.HoSoXuLy37Dto;
using XHoSoXuLyHistoryDto = newPSG.PMS.Dto.HoSoXuLyHistory37Dto;
using System.Collections.Generic;
using FlexCel.Core;
using FlexCel.XlsAdapter;
using System.Text;
using newPSG.PMS.EntityDB;
using FlexCel.Report;
using Newtonsoft.Json;
using Abp.Domain.Uow;
using newPSG.PMS.MultiTenancy;
using newPSG.PMS.Web;
using newPSG.PMS.Emailing;
#endregion

namespace newPSG.PMS.Services
{
    #region INTERFACE
    public interface IXuLyHoSoVanThu37AppService : IApplicationService
    {
        Task<dynamic> LoadVanThuDuyet(VanThuXuLy37InputDto input);

        Task<long> VanThuRaSoatHoSo(VanThuRaSoatHoSo37Input input);
        Task<byte[]> GoToViewPhieuTiepNhan(PhieuTiepNhanInputDto input);
        Task VanThuKyVaTraGiayTiepNhan(PhieuTiepNhanInputDto input);
        Task GuiLanhDaoCucPhanCongHoSo(long hoSoId);
        Task DongDau(VanThuXuLy37InputDto input);
        Task VanThuTraKetQua(long hoSoId);
        Task GuiMailThongBaoBoSungHoSo(long hoSoId);
    }
    #endregion

    #region MAIN
    [AbpAuthorize]
    public class XuLyHoSoVanThu37AppService : PMSAppServiceBase, IXuLyHoSoVanThu37AppService
    {
        private readonly int _thuTucId = (int)CommonENum.THU_TUC_ID.THU_TUC_37;
        private readonly IRepository<XHoSo, long> _hoSoRepos;
        private readonly IRepository<XHoSoTepDinhKem, long> _hoSoTepDinhKemRepos;
        private readonly IRepository<XHoSoXuLy, long> _hoSoXuLyRepos;
        private readonly IRepository<XHoSoXuLyHistory, long> _hoSoXuLyHistoryRepos;
        private readonly IRepository<User, long> _userRepos;
        private readonly IAbpSession _session;
        private readonly IRepository<Tinh> _tinhRepos;
        private readonly IAppFolders _appFolders;
        private readonly ICustomTennantAppService _customTennantAppService;
        private readonly IRepository<PhongBanLoaiHoSo> _phongBanLoaiHoSoRepos;
        private readonly IRepository<ThanhToan, long> _thanhToanRepos;
        private readonly ICurrentUnitOfWorkProvider _unitOfWorkProvider;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IEmailSender _emailSender;
        private readonly IWebUrlService _webUrlService;
        private readonly IRepository<DoanhNghiep, long> _doanhNghiepRepos;
        private readonly IEmailTemplateProvider _emailTemplateProvider;
        public XuLyHoSoVanThu37AppService(IRepository<XHoSo, long> hoSoRepos,
                                          IRepository<XHoSoTepDinhKem, long> hoSoTepDinhKemRepos,
                                          IRepository<XHoSoXuLy, long> hoSoXuLyRepos,
                                          IRepository<XHoSoXuLyHistory, long> hoSoXuLyHistoryRepos,
                                          IRepository<User, long> userRepos,
                                          IAbpSession session,
                                          IRepository<Tinh> tinhRepos,
                                          IAppFolders appFolders,
                                          ICustomTennantAppService customTennantAppService,
                                          IRepository<PhongBanLoaiHoSo> phongBanLoaiHoSoRepos,
                                          ICurrentUnitOfWorkProvider unitOfWorkProvider,
                                          IRepository<Tenant> tenantRepository,
                                          IEmailSender emailSender,
                                          IWebUrlService webUrlService,
                                          IRepository<DoanhNghiep, long> doanhNghiepRepos,
                                          IEmailTemplateProvider emailTemplateProvider,
                                          IRepository<ThanhToan, long> thanhToanRepos)
        {
            _hoSoRepos = hoSoRepos;
            _hoSoTepDinhKemRepos = hoSoTepDinhKemRepos;
            _hoSoXuLyRepos = hoSoXuLyRepos;
            _hoSoXuLyHistoryRepos = hoSoXuLyHistoryRepos;
            _userRepos = userRepos;
            _session = session;
            _tinhRepos = tinhRepos;
            _appFolders = appFolders;
            _customTennantAppService = customTennantAppService;
            _phongBanLoaiHoSoRepos = phongBanLoaiHoSoRepos;
            _thanhToanRepos = thanhToanRepos;
            _unitOfWorkProvider = unitOfWorkProvider;
            _tenantRepository = tenantRepository;
            _emailSender = emailSender;
            _webUrlService = webUrlService;
            _doanhNghiepRepos = doanhNghiepRepos;
            _emailTemplateProvider = emailTemplateProvider;
        }

        public async Task<dynamic> LoadVanThuDuyet(VanThuXuLy37InputDto input)
        {
            try
            {
                var hosoxl = await _hoSoXuLyRepos.FirstOrDefaultAsync(input.HoSoXuLyId);
                if (hosoxl != null && hosoxl.Id > 0)
                {
                    string _tenLanhDaoCuc = "";
                    string _tenTruongPhong = "";

                    if (hosoxl.LanhDaoCucId.HasValue && hosoxl.LanhDaoCucId > 0)
                    {
                        var lanhDaoCucObj = _userRepos.FirstOrDefault(hosoxl.LanhDaoCucId.Value);
                        _tenLanhDaoCuc = lanhDaoCucObj.Surname + " " + lanhDaoCucObj.Name;
                    }

                    var truongPhongObj = _userRepos.FirstOrDefault(hosoxl.TruongPhongId.Value);
                    _tenTruongPhong = truongPhongObj.Surname + " " + truongPhongObj.Name;

                    dynamic objInfo = new
                    {
                        TenLanhDaoCuc = _tenLanhDaoCuc,
                        TenTruongPhong = _tenTruongPhong
                    };

                    return new
                    {
                        hoSoXuLy = hosoxl,
                        objInfo
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
            }
            return null;
        }

        #region một cửa rà soát hồ sơ và trả phiếu tiếp nhận

        public async Task<byte[]> GoToViewPhieuTiepNhan(PhieuTiepNhanInputDto input)
        {
            try
            {
                byte[] fileTemplate = null;
                ExcelFile xls = null;
                XlsFile Result = new XlsFile(true);

                using (FlexCelReport fr = new FlexCelReport(true))
                {
                    string DuongDan = "/ThuTuc_37/PhieuTiepNhan.xlsx";
                    var path = _appFolders.PDFTemplate + DuongDan;

                    var hoso = await _hoSoRepos.GetAsync(input.HoSoId);

                    fr.SetValue("HoVaTen", hoso.TenNguoiDaiDien);
                    fr.SetValue("DiaChi", hoso.DiaChi);
                    fr.SetValue("DienThoai", hoso.SoDienThoai);
                    fr.SetValue("SoGiayTiepNhan", Utility.GetCodeRandom());
                    if (input.ListTaiLieuDaNhan.Count > 0)
                    {
                        fr.AddTable("ListTaiLieuDaNhan", input.ListTaiLieuDaNhan);
                    }

                    fr.SetValue("NgayHen", input.NgayHenCap.Value.ToString("dd/MM/yyyy"));

                    var listHoSoXyLyBoSung = _hoSoXuLyRepos.GetAll().Where(x => x.HoSoId == hoso.Id && x.IsHoSoBS == true).ToList();
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
                    var currentUser = _userRepos.Get(_session.UserId.Value);
                    fr.SetValue("NguoiKy", currentUser.Surname + " " + currentUser.Name);
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
                    string NgayKy = "Hà nội, ngày " + day + " tháng " + month + " năm " + DateTime.Today.Year;
                    if (currentUser.TinhId.HasValue)
                    {
                        var tinh = _tinhRepos.Get(currentUser.TinhId.Value);
                        NgayKy = tinh.Ten + ", ngày " + DateTime.Today.Day + " tháng " + DateTime.Today.Month + " năm " + DateTime.Today.Year;
                    }
                    fr.SetValue("NgayKy", NgayKy);
                    Result.Open(path);
                    fr.Run(Result);
                    fr.Dispose();
                    xls = Result;

                    var outS = new System.IO.MemoryStream();
                    using (FlexCel.Render.FlexCelPdfExport pdf = new FlexCel.Render.FlexCelPdfExport(xls, true))
                    {
                        pdf.Export(outS);
                    }
                    fileTemplate = outS.ToArray();
                    return fileTemplate;
                }

            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                return null;
            }
        }

        public async Task<long> VanThuRaSoatHoSo(VanThuRaSoatHoSo37Input input)
        {
            var hoso = await _hoSoRepos.FirstOrDefaultAsync(x => x.Id == input.HoSoId);
            var hsxl = await _hoSoXuLyRepos.FirstOrDefaultAsync(x => x.Id == input.HoSoXuLyId);
            if (hoso != null && hsxl != null)
            {
                if (input.TrangThaiXuLy == (int)CommonENum.KET_QUA_XU_LY.DAT)
                {
                    if (hoso.IsHoSoBS != true)
                    {
                        int tenantId = GetTenantIdFromHoSo(hoso);
                        var thanhToan = new ThanhToan()
                        {
                            HoSoId = hoso.Id,
                            DoanhNghiepId = hoso.DoanhNghiepId,
                            PhiDaNop = input.PhiDaNop,
                            NgayGiaoDich = DateTime.Now,
                            PhiXacNhan = input.PhiDaNop,
                            TenantId = tenantId,
                            PhanHeId = _thuTucId,
                            TrangThaiNganHang = (int)CommonENum.TRANG_THAI_GIAO_DICH.GIAO_DICH_THANH_CONG
                        };
                        hoso.ThanhToanId_Active = await _thanhToanRepos.InsertAndGetIdAsync(thanhToan);
                    }
                    hoso.TrangThaiHoSo = (int)CommonENum.TRANG_THAI_HO_SO.CHO_TRA_GIAY_TIEP_NHAN;

                    hsxl.VanThuId = _session.UserId;
                    hsxl.DonViGui = (int)CommonENum.DON_VI_XU_LY.MOT_CUA_TIEP_NHAN;
                    hsxl.NguoiGuiId = SessionCustom.UserCurrent.Id;
                    hsxl.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.MOT_CUA_TIEP_NHAN;
                    hsxl.NguoiXuLyId = SessionCustom.UserCurrent.Id;
                    hsxl.NgayGui = DateTime.Now;

                    await _hoSoRepos.UpdateAsync(hoso);
                    await _hoSoXuLyRepos.UpdateAsync(hsxl);

                }
                else if (input.TrangThaiXuLy == (int)CommonENum.KET_QUA_XU_LY.KHONG_DAT)
                {
                    hsxl.DonViGui = (int)CommonENum.DON_VI_XU_LY.MOT_CUA_TIEP_NHAN;
                    hsxl.NguoiGuiId = SessionCustom.UserCurrent.Id;
                    hsxl.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.DOANH_NGHIEP;
                    hsxl.LyDoTraLai = input.LyDoTuChoi;
                    hsxl.NgayGui = DateTime.Now;
                    hsxl.VanThuId = _session.UserId;
                    hoso.TrangThaiHoSo = (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_TRA_LAI;
                    await _hoSoRepos.UpdateAsync(hoso);
                    await _hoSoXuLyRepos.UpdateAsync(hsxl);
                }

                #region Lưu lịch sử
                var _history = new XHoSoXuLyHistory();
                _history.HoSoId = hoso.Id;
                _history.HoSoXuLyId = hsxl.Id;
                _history.LoaiHoSoId = hoso.LoaiHoSoId;
                _history.NguoiXuLyId = SessionCustom.UserCurrent.Id;
                _history.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.MOT_CUA_TIEP_NHAN;
                _history.ActionEnum = (int)CommonENum.FORM_FUNCTION.MOT_CUA_RA_SOAT_HO_SO;

                _history.NgayXuLy = DateTime.Now;
                var _historyId = _hoSoXuLyHistoryRepos.InsertOrUpdateAndGetId(_history);
                #endregion
            }
            return hoso.Id;
        }
        
        public async Task VanThuKyVaTraGiayTiepNhan(PhieuTiepNhanInputDto input)
        {
            var hoso = await _hoSoRepos.GetAsync(input.HoSoId);
            var hsxl = await _hoSoXuLyRepos.GetAsync(hoso.HoSoXuLyId_Active.Value);

            if (hoso.IsHoSoBS != true)
            {
                int tenantId = GetTenantIdFromHoSo(hoso);
                var thanhToan = new ThanhToan()
                {
                    HoSoId = hoso.Id,
                    DoanhNghiepId = hoso.DoanhNghiepId,
                    PhiDaNop = input.PhiDaNop,
                    NgayGiaoDich = DateTime.Now,
                    PhiXacNhan = input.PhiDaNop,
                    TenantId = tenantId,
                    PhanHeId = _thuTucId,
                    TrangThaiNganHang = (int)CommonENum.TRANG_THAI_GIAO_DICH.GIAO_DICH_THANH_CONG
                };
                hoso.ThanhToanId_Active = await _thanhToanRepos.InsertAndGetIdAsync(thanhToan);
            }

            hsxl.VanThuId = _session.UserId;
            hsxl.DonViGui = (int)CommonENum.DON_VI_XU_LY.MOT_CUA_TIEP_NHAN;
            hsxl.NguoiGuiId = SessionCustom.UserCurrent.Id;
            hsxl.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.MOT_CUA_TIEP_NHAN;
            hsxl.NguoiXuLyId = SessionCustom.UserCurrent.Id;
            hsxl.NgayGui = DateTime.Now;
            hsxl.GiayTiepNhanCA = input.GiayTiepNhanCA;
            hsxl.NgayHenCap = input.NgayHenCap;
            hsxl.NgayHenTra = input.NgayHenCap;
            hsxl.HinhThucCapCTJson = JsonConvert.SerializeObject(input.HinhThucCapChungChi);
            hsxl.TaiLieuDaNhanJson = JsonConvert.SerializeObject(input.ListTaiLieuDaNhan);
            hsxl.NgayTraGiayTiepNhan = DateTime.Now;

            #region Lưu lịch sử
            var _history = new XHoSoXuLyHistory();
            _history.HoSoId = hoso.Id;
            _history.HoSoXuLyId = hsxl.Id;
            _history.LoaiHoSoId = hoso.LoaiHoSoId;
            _history.NguoiXuLyId = SessionCustom.UserCurrent.Id;
            _history.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.MOT_CUA_TIEP_NHAN;
            _history.ActionEnum = (int)CommonENum.FORM_FUNCTION.MOT_CUA_RA_SOAT_HO_SO;

            _history.NgayXuLy = DateTime.Now;
            var _historyId = _hoSoXuLyHistoryRepos.InsertOrUpdateAndGetId(_history);
            #endregion

        }
        public async Task GuiLanhDaoCucPhanCongHoSo(long hoSoId)
        {
            var hoso = await _hoSoRepos.GetAsync(hoSoId);
            var hsxl = await _hoSoXuLyRepos.GetAsync(hoso.HoSoXuLyId_Active.Value);
            int phongBanIdXuLy = GetPhongBanIdXuLyHoSo(hoso);

            if (phongBanIdXuLy > 0)
            {
                hoso.PhongBanId = phongBanIdXuLy;
                hoso.IsChuyenAuto = true;
                hoso.NgayChuyenAuto = DateTime.Now;
                hoso.TrangThaiHoSo = (int)CommonENum.TRANG_THAI_HO_SO.LANH_DAO_DA_PHAN_CONG_HO_SO;

                hsxl.DonViGui = (int)CommonENum.DON_VI_XU_LY.LANH_DAO_CUC;
                hsxl.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.PHONG_BAN_PHAN_CONG;
            }
            else
            {
                hoso.TrangThaiHoSo = (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_DA_TIEP_NHAN;
                hsxl.DonViGui = (int)CommonENum.DON_VI_XU_LY.MOT_CUA_TIEP_NHAN;
                hsxl.NguoiGuiId = _session.UserId;
                hsxl.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.LANH_DAO_CUC;
                hsxl.NguoiXuLyId = null;

                #region Lưu lịch sử
                var _history = new XHoSoXuLyHistory();
                _history.HoSoId = hoso.Id;
                _history.HoSoXuLyId = hsxl.Id;
                _history.LoaiHoSoId = hoso.LoaiHoSoId;
                _history.NguoiXuLyId = SessionCustom.UserCurrent.Id;
                _history.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.MOT_CUA_TIEP_NHAN;
                _history.ActionEnum = (int)CommonENum.FORM_FUNCTION.MOT_CUA_GUI_LANH_DAO;
                _history.NgayXuLy = DateTime.Now;
                var _historyId = _hoSoXuLyHistoryRepos.InsertOrUpdateAndGetId(_history);
                #endregion
            }

            await _hoSoRepos.UpdateAsync(hoso);
            await _hoSoXuLyRepos.UpdateAsync(hsxl);
        }


        #endregion End một cửa rà soát hồ sơ và trả phiếu tiếp nhận


        #region đóng giấu công văn yêu cầu bổ xung
        public async Task DongDau(VanThuXuLy37InputDto input)
        {
            var vanThuId = _session.UserId;
            try
            {
                var hoSo = await _hoSoRepos.FirstOrDefaultAsync(x => x.Id == input.HoSoId);
                var hosoxl = await _hoSoXuLyRepos.FirstOrDefaultAsync(input.HoSoXuLyId);
                if (hosoxl != null && hoSo != null && hosoxl.Id > 0)
                {

                    //hoSo.TrangThaiHoSo = (int)CommonENum.TRANG_THAI_HO_SO.SUA_DOI_BO_SUNG;

                    hosoxl.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.DOANH_NGHIEP;
                    hosoxl.DonViGui = (int)CommonENum.DON_VI_XU_LY.VAN_THU;
                    hosoxl.NguoiGuiId = _session.UserId;
                    hosoxl.NguoiXuLyId = hoSo.CreatorUserId;
                    hosoxl.NgayGui = DateTime.Now;
                    hosoxl.VanThuDaDuyet = true;

                    hosoxl.VanThuId = vanThuId;
                    hosoxl.VanThuIsCA = true;
                    hosoxl.VanThuNgayDongDau = DateTime.Now;
                    hosoxl.DuongDanTepCA = input.DuongDanTepCA;
                    hosoxl.YKienGui = null;

                    await _hoSoRepos.UpdateAsync(hoSo);
                    await _hoSoXuLyRepos.UpdateAsync(hosoxl);

                    if (hosoxl.HoSoIsDat != true)
                    {
                        #region HoSo_Clone

                        var hoSoClone = new XHoSoDto();
                        hoSo.MapTo(hoSoClone);
                        hoSoClone.PId = hoSo.PId != null ? hoSo.PId : hoSo.Id;
                        var insertInput = hoSoClone.MapTo<XHoSo>();
                        long idClone = await _hoSoRepos.InsertAndGetIdAsync(insertInput);
                        CurrentUnitOfWork.SaveChanges();
                        var teps = _hoSoTepDinhKemRepos.GetAll()
                            .Where(x => x.HoSoId == input.HoSoId);

                        foreach (var tep in teps)
                        {
                            var tepNew = new XHoSoTepDinhKem
                            {
                                HoSoId = insertInput.Id,
                                IsActive = tep.IsActive,
                                DuongDanTep = tep.DuongDanTep,
                                IsCA = tep.IsCA,
                                DaTaiLen = tep.DaTaiLen,
                                LoaiTepDinhKem = tep.LoaiTepDinhKem,
                                MoTaTep = tep.MoTaTep,
                                TenTep = tep.TenTep
                            };
                            await _hoSoTepDinhKemRepos.InsertAsync(tepNew);
                        }
                        #endregion
                    }

                    #region Lưu lịch sử
                    var _history = new XHoSoXuLyHistory();
                    _history.ThuTucId = hosoxl.ThuTucId;
                    _history.HoSoXuLyId = hosoxl.Id;
                    _history.HoSoId = hosoxl.HoSoId;
                    _history.IsHoSoBS = hosoxl.IsHoSoBS;
                    _history.NguoiXuLyId = _session.UserId;
                    _history.NoiDungYKien = null;
                    _history.TrangThaiCV = hosoxl.TrangThaiCV;
                    _history.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.VAN_THU;
                    _history.ActionEnum = (int)CommonENum.FORM_FUNCTION.VAN_THU_DONG_DAU;

                    _history.HoSoIsDat = hosoxl.HoSoIsDat;
                    _history.HoSoIsDat_Pre = hosoxl.HoSoIsDat;

                    await _hoSoXuLyHistoryRepos.InsertAndGetIdAsync(_history);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
            }
        }
        #endregion

        #region trả kết quả cho người đăng ký cấp chứng chỉ hành nghề

        public async Task VanThuTraKetQua(long hoSoId)
        {
            var hoso = await _hoSoRepos.GetAsync(hoSoId);
            var hsxl = await _hoSoXuLyRepos.GetAsync(hoso.HoSoXuLyId_Active.Value);

            hsxl.NguoiGuiId = _session.UserId;
            hsxl.NguoiXuLyId = hoso.CreatorUserId;
            hsxl.VanThuNgayDuyet = DateTime.Now;
            hsxl.VanThuId = _session.UserId;
            hsxl.DonViGui = (int)CommonENum.DON_VI_XU_LY.VAN_THU;
            hsxl.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.DOANH_NGHIEP;

            hoso.TrangThaiHoSo = (int)CommonENum.TRANG_THAI_HO_SO.DA_HOAN_TAT;
            hoso.NgayTraKetQua = DateTime.Now;
            await _hoSoXuLyRepos.UpdateAsync(hsxl);
            await _hoSoRepos.UpdateAsync(hoso);

            var _history = new XHoSoXuLyHistory();
            _history.NgayXuLy = DateTime.Now;
            _history.HoSoId = hoso.Id;
            _history.HoSoXuLyId = hsxl.Id;
            _history.NgayXuLy = DateTime.Now;
            _history.NguoiXuLyId = _session.UserId;
            _history.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.VAN_THU;
            _history.ActionEnum = (int)CommonENum.FORM_FUNCTION.VAN_THU_DUYET_THAM_DINH;
            _history.IsKetThuc = true;

            await _hoSoXuLyHistoryRepos.InsertAsync(_history);
        }

        #endregion

        // private function
        private int GetTenantIdFromHoSo(XHoSo hoSo)
        {
            int tenantId = _customTennantAppService.GetTenantIdCucHCC();
            if (hoSo != null && hoSo.IsChiCuc == true && hoSo.ChiCucId.HasValue)
            {
                tenantId = hoSo.ChiCucId.Value;
            }
            return tenantId;
        }
        public int GetPhongBanIdXuLyHoSo(XHoSo hoSo)
        {
            //Loai ho so
            int tenantId = GetTenantIdFromHoSo(hoSo);
            //tenantId = 1;
            int phongBanId = -1;

            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant))
            {
                var _listPhongBanXuLy = (from pn in _phongBanLoaiHoSoRepos.GetAll()
                                         where pn.LoaiHoSoId == hoSo.LoaiHoSoId && pn.TenantId == tenantId
                                         select pn.PhongBanId).ToList();


                if (_listPhongBanXuLy != null && _listPhongBanXuLy.Count == 1)
                {
                    phongBanId = _listPhongBanXuLy[0];
                }
            }

            return phongBanId;
        }

        public async Task GuiMailThongBaoBoSungHoSo(long hoSoId)
        {
            try
            {
                var hoso = await _hoSoRepos.GetAsync(hoSoId);
                var hosoxl = await _hoSoXuLyRepos.GetAsync(hoso.HoSoXuLyId_Active.Value);
                var subject = "Thông báo bổ sung hồ sơ";
                var tenancyName = GetTenancyNameOrNull(_session.TenantId);
                var thuTucStr = CommonENum.GetEnumDescription((CommonENum.THU_TUC_TEXT)hoso.ThuTucId);
                var emailTemplate = new StringBuilder(_emailTemplateProvider.GetDefaultTemplate(_session.TenantId));
                emailTemplate.Replace("{EMAIL_TITLE}", "Thông báo");
                emailTemplate.Replace("{EMAIL_SUB_TITLE}", "Cục quản lý Y Dược cổ truyền Hà Nội thông báo");

                var doanhNghiep = await _doanhNghiepRepos.GetAsync(hoso.DoanhNghiepId.Value);

                var link = _webUrlService.GetSiteRootAddress(tenancyName);
                var linkViewCongVan = _webUrlService.GetSiteRootAddress(tenancyName) + "File/GoToViewTaiLieu?url=" + hosoxl.DuongDanTepCA;

                var mailMessage = new StringBuilder();

                mailMessage.AppendLine("<b>" + "Yêu cầu " + doanhNghiep.TenDoanhNghiep + " chỉnh sửa bổ sung hồ sơ " + hoso.MaHoSo + " của thủ tục " + thuTucStr + "</b>" + "<br />");

                if (!String.IsNullOrEmpty(hosoxl.DuongDanTepCA))
                {
                    mailMessage.AppendLine("<b>" + "Nội dung yêu cầu bổ sung vui lòng xem chi tiết tại công văn yêu cầu bổ sung" + "</b>" + "<br />");
                    mailMessage.AppendLine("Click vào link phía dưới để xem công văn yêu cầu bổ sung" + "<br />");
                    mailMessage.AppendLine("<a href=\"" + linkViewCongVan + "\">" + linkViewCongVan + "</a>");
                    mailMessage.AppendLine("<br />");
                }

                mailMessage.AppendLine("Click vào link phía dưới để đăng nhập vào hệ thống" + "<br />");
                mailMessage.AppendLine("<a href=\"" + link + "\">" + link + "</a>");
                emailTemplate.Replace("{EMAIL_BODY}", mailMessage.ToString());

                await _emailSender.SendAsync(doanhNghiep.EmailDoanhNghiep, subject, emailTemplate.ToString());
            }
            catch (Exception ex)
            {
                Logger.Error($"{DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy")} GuiMailThongBaoBoSungHoSo {ex.Message} {JsonConvert.SerializeObject(ex)}");
            }
        }

        private string GetTenancyNameOrNull(int? tenantId)
        {
            if (tenantId == null)
            {
                return null;
            }

            using (_unitOfWorkProvider.Current.SetTenantId(null))
            {
                return _tenantRepository.Get(tenantId.Value).TenancyName;
            }
        }

    }
    #endregion

}