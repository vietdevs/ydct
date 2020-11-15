using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using newPSG.PMS.Authorization.Users;
using newPSG.PMS.Common.Dto;
using newPSG.PMS.Dto;
using newPSG.PMS.EntityDB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;

#region Class Riêng Cho Từng Thủ tục

using XHoSo = newPSG.PMS.EntityDB.HoSo98;
using XHoSoTepDinhKem = newPSG.PMS.EntityDB.HoSoTepDinhKem98;
using XHoSoXuLy = newPSG.PMS.EntityDB.HoSoXuLy98;
using XHoSoXuLyHistory = newPSG.PMS.EntityDB.HoSoXuLyHistory98;
using XHoSoDto = newPSG.PMS.Dto.HoSo98Dto;
using XHoSoInputDto = newPSG.PMS.Dto.HoSoInput98Dto;
using XHoSoTepDinhKemDto = newPSG.PMS.Dto.HoSoTepDinhKem98Dto;
using XHoSoXuLyDto = newPSG.PMS.Dto.HoSoXuLy98Dto;
using XHoSoXuLyHistoryDto = newPSG.PMS.Dto.HoSoXuLyHistory98Dto;
#endregion

namespace newPSG.PMS.Services
{
    public interface IXuLyHoSoGridView98AppService : IApplicationService
    {
        Task<PagedResultDto<XHoSoDto>> GetListHoSoPaging(XHoSoInputDto input);
        dynamic GetListFormCase(int _formId);
        dynamic GetListFormFunction();
        Task<dynamic> GetListFormCaseCountNumber(XHoSoInputDto input);
    }
    [AbpAuthorize]
    public class XuLyHoSoGridView98AppService : PMSAppServiceBase, IXuLyHoSoGridView98AppService
    {
        private readonly int _thuTucId = (int)CommonENum.THU_TUC_ID.THU_TUC_98;
        private readonly IRepository<XHoSo, long> _hoSoRepos;
        private readonly IRepository<XHoSoXuLy, long> _hoSoXuLyRepos;
        private readonly IRepository<LoaiHoSo> _loaiHoSoRepos;
        private readonly IRepository<PhongBan> _phongBanRepos;
        private readonly IRepository<PhongBanLoaiHoSo> _phongBanLoaiHoSoRepos;
        private readonly IRepository<User, long> _userRepos;
        private readonly IAbpSession _session;
        private readonly ILichLamViecAppService _lichLamViecAppService;
        private readonly CustomSessionAppSession _mySession;

        public XuLyHoSoGridView98AppService(IRepository<XHoSo, long> hoSoRepos,
                                            IRepository<XHoSoXuLy, long> hoSoXuLyRepos,
                                            IRepository<LoaiHoSo> loaiHoSoRepos,
                                            IRepository<PhongBan> phongBanRepos,
                                            IRepository<PhongBanLoaiHoSo> phongBanLoaiHoSoRepos,
                                            IRepository<User, long> userRepos,
                                            IAbpSession session,
                                            ILichLamViecAppService lichLamViecAppService,
                                            CustomSessionAppSession mySession)
        {
            _hoSoRepos = hoSoRepos;
            _hoSoXuLyRepos = hoSoXuLyRepos;
            _loaiHoSoRepos = loaiHoSoRepos;
            _phongBanRepos = phongBanRepos;
            _phongBanLoaiHoSoRepos = phongBanLoaiHoSoRepos;
            _userRepos = userRepos;
            _session = session;
            _lichLamViecAppService = lichLamViecAppService;
            _mySession = mySession;
        }
        
        public async Task<PagedResultDto<XHoSoDto>> GetListHoSoPaging(XHoSoInputDto input)
        {
            try
            {
                var query = QueryGetAllHoSo(input);

                #region Sắp xếp hồ sơ theo từng form

                //Form 1
                if (input.FormId == (int)CommonENum.FORM_ID.FORM_DANG_KY_HO_SO)
                {
                    query = query.OrderByDescending(p => p.CreationTime);
                }
                else
                {
                    query = query.OrderBy(p => p.NgayTiepNhan);
                }
                #endregion

                var _total = await query.CountAsync();
                if (input.IsOnlyToTal.HasValue && input.IsOnlyToTal.Value)
                {
                    return new PagedResultDto<XHoSoDto>(_total, null);
                }

                var dataGrids = await query
                    .PageBy(input)
                    .ToListAsync();

                foreach (var item in dataGrids)
                {
                    #region Trạng thái hồ sơ
                    item.StrTrangThai = GetTrangThaiXuLyHoSo(item);
                    #endregion

                    item.StrDonViXuLy = item.DonViXuLy != null ? CommonENum.GetEnumDescription((CommonENum.DON_VI_XU_LY)(int)item.DonViXuLy) : "";
                    item.StrDonViGui = item.DonViGui != null ? CommonENum.GetEnumDescription((CommonENum.DON_VI_XU_LY)(int)item.DonViGui) : "";

                    using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
                    {
                        if (item.ChuyenVienThuLyId.HasValue)
                        {
                            var chuyenVienThuLyObj = _userRepos.FirstOrDefault(item.ChuyenVienThuLyId.Value);
                            item.ChuyenVienThuLyName = chuyenVienThuLyObj.Surname + " " + chuyenVienThuLyObj.Name;
                        }
                        if (item.ChuyenVienPhoiHopId.HasValue)
                        {
                            var chuyenVienPhoiHopObj = _userRepos.FirstOrDefault(item.ChuyenVienPhoiHopId.Value);
                            item.ChuyenVienPhoiHopName = chuyenVienPhoiHopObj.Surname + " " + chuyenVienPhoiHopObj.Name;
                        }
                        if (item.NguoiGuiId.HasValue)
                        {
                            var objUser = _userRepos.FirstOrDefault(item.NguoiGuiId.Value);
                            item.TenNguoiGui = objUser.Surname + " " + objUser.Name;
                        }
                        if (item.NguoiXuLyId.HasValue)
                        {
                            var objUser = _userRepos.FirstOrDefault(item.NguoiXuLyId.Value);
                            item.TenNguoiXuLy = objUser.Surname + " " + objUser.Name;
                        }
                    }

                    item.TenHCC = ConfigurationManager.AppSettings["TEN_HCC_CUC"];

                    #region Tính số ngày quá hạn
                    item.SoNgayQuaHan = 0;
                    if (item.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_HOAN_TAT && item.TrangThaiHoSo
                        != (int)CommonENum.TRANG_THAI_HO_SO.SUA_DOI_BO_SUNG && item.TrangThaiHoSo
                        != (int)CommonENum.TRANG_THAI_HO_SO.TU_CHOI_CAP_PHEP && item.NgayHenTra != null)
                    {
                        #region Số ngày quá hạn từng bộ phận
                        if (item.NgayGui != null)
                        {
                            SoNgayQuaHanChiTiet98Dto _soNgayQuaHanChiTiet = null;
                            if (item.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.PHONG_BAN_PHAN_CONG && item.ChuyenVienThuLyId == null && item.ChuyenVienThuLyDaDuyet == null)
                            {
                                int soNgayLamViec = 1;
                                _soNgayQuaHanChiTiet = TinhSoNgayQuaHanChiTiet(soNgayLamViec, item.NgayGui);
                            }
                            else if (item.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET && item.ChuyenVienThuLyDaDuyet == null)
                            {
                                int soNgayLamViec = 2;
                                _soNgayQuaHanChiTiet = TinhSoNgayQuaHanChiTiet(soNgayLamViec, item.NgayGui);
                            }
                            else if (item.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.PHO_PHONG && item.PhoPhongDaDuyet == null)
                            {
                                int soNgayLamViec = 2;
                                _soNgayQuaHanChiTiet = TinhSoNgayQuaHanChiTiet(soNgayLamViec, item.NgayGui);
                            }
                            else if (item.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG && item.TruongPhongDaDuyet == null)
                            {
                                int soNgayLamViec = 2;
                                _soNgayQuaHanChiTiet = TinhSoNgayQuaHanChiTiet(soNgayLamViec, item.NgayGui);
                            }
                            else if (item.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.LANH_DAO_CUC && item.LanhDaoCucDaDuyet == null)
                            {
                                int soNgayLamViec = 2;
                                _soNgayQuaHanChiTiet = TinhSoNgayQuaHanChiTiet(soNgayLamViec, item.NgayGui);
                            }
                            else if (item.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.VAN_THU && item.VanThuDaDuyet == null)
                            {
                                int soNgayLamViec = 1;
                                _soNgayQuaHanChiTiet = TinhSoNgayQuaHanChiTiet(soNgayLamViec, item.NgayGui);
                            }

                            if (_soNgayQuaHanChiTiet != null)
                            {
                                item.SoNgayQuaHanChiTiet = _soNgayQuaHanChiTiet.SoNgayQuaHanChiTiet;
                                item.SoGioQuaHanChiTiet = _soNgayQuaHanChiTiet.SoGioQuaHanChiTiet;
                                item.StrQuaHanChiTiet = _soNgayQuaHanChiTiet.StrQuaHanChiTiet;
                            }
                        }
                        #endregion

                        DateTime ngay_begin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                        DateTime ngay_end = new DateTime(item.NgayHenTra.Value.Year, item.NgayHenTra.Value.Month, item.NgayHenTra.Value.Day);
                        if (ngay_begin > ngay_end)
                        {
                            TimeSpan Time = ngay_begin - ngay_end;
                            item.SoNgayQuaHan = Time.Days;
                            item.StrQuaHan = "Quá hạn " + item.SoNgayQuaHan + " ngày";
                        }
                        else
                        {
                            item.SoNgayQuaHan = _lichLamViecAppService.GetSoNgayLamViec(ngay_begin, ngay_end);
                            item.StrQuaHan = "Còn " + item.SoNgayQuaHan + " ngày làm việc";
                            item.SoNgayQuaHan = item.SoNgayQuaHan * (-1);
                        }
                    }
                    else if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.SUA_DOI_BO_SUNG)
                    {
                        item.StrQuaHan = "Đã trả công văn";
                    }
                    else if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_HOAN_TAT || item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.TU_CHOI_CAP_PHEP)
                    {
                        item.StrQuaHan = "Xử lý xong";
                    }
                    #endregion

                    #region Các phòng ban có thể xử lý hồ sơ này (một cửa phân công)
                    if (input.FormId == (int)CommonENum.FORM_ID.FORM_MOT_CUA_PHAN_CONG)
                    {

                        var _listPhongBan = (from ploai in _phongBanLoaiHoSoRepos.GetAll()
                                             join pb in _phongBanRepos.GetAll() on ploai.PhongBanId equals pb.Id
                                             where ploai.LoaiHoSoId == item.LoaiHoSoId
                                             select new ItemDto<int>
                                             {
                                                 Id = pb.Id,
                                                 Name = pb.TenPhongBan,
                                                 Checked = false
                                             }).ToList();

                        if (item.PhongBanId.HasValue && item.PhongBanId > 0)
                        {
                            foreach (var phongban in _listPhongBan)
                            {
                                if (phongban.Id == item.PhongBanId)
                                {
                                    phongban.Checked = true;
                                }
                            }
                        }

                        item.ArrPhongBanXuLy = _listPhongBan;
                    }
                    #endregion

                    #region Chức năng trên grid

                    item.FormId = input.FormId;
                    item.FormCase = input.FormCase;
                    item.FormCase2 = input.FormCase2;

                    item.ArrChucNang = ArrayChucNangGridHoSo(item, input);
                    #endregion
                }

                return new PagedResultDto<XHoSoDto>(_total, dataGrids);
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                return null;
            }
        }

        [AbpAllowAnonymous]
        public dynamic GetListFormCase(int _formId)
        {
            try
            {
                var _form = (CommonENum.FORM_ID)_formId;
                return new
                {
                    FormCase = CommonEnumExtensions.GetListFormCase(_form),
                    FormCase2 = CommonEnumExtensions.GetListFormCase2(_form)
                };
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                return null;
            }
        }

        [AbpAllowAnonymous]
        public dynamic GetListFormFunction()
        {
            try
            {
                return CommonEnumExtensions.getListFormFunction();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                return null;
            }
        }

        #region Function QueryGetAllHoSo<IQueryable>
        //MAIN
        public IQueryable<XHoSoDto> QueryGetAllHoSo(XHoSoInputDto input)
        {
            try
            {
                long hoSoId = 0;
                long.TryParse(input.Keyword, out hoSoId);
                DateTime NgayNopToi = new DateTime(), NgayNopTu = new DateTime();
                if (input.NgayNopToi.HasValue && input.NgayNopTu.HasValue)
                {
                    NgayNopToi = new DateTime(input.NgayNopToi.Value.Year, input.NgayNopToi.Value.Month, input.NgayNopToi.Value.Day, 23, 59, 59);
                    NgayNopTu = new DateTime(input.NgayNopTu.Value.Year, input.NgayNopTu.Value.Month, input.NgayNopTu.Value.Day, 0, 0, 0);
                }
                var query = (from hoso in _hoSoRepos.GetAll()
                             join r_hsxl in _hoSoXuLyRepos.GetAll() on hoso.HoSoXuLyId_Active equals r_hsxl.Id into tb_hsxl //Left Join
                             from hsxl in tb_hsxl.DefaultIfEmpty()
                             where (hoso.PId == null && hoso.IsDeleted == false)
                             select new XHoSoDto
                             {
                                 ThuTucId = hoso.ThuTucId,
                                 Id = hoso.Id,
                                 HoSoXuLyId = hsxl.Id,
                                 MaSoThue = hoso.MaSoThue,
                                 MaHoSo = hoso.MaHoSo,
                                 SoDangKy = hoso.SoDangKy,
                                 IsCA = hoso.IsCA,
                                 OnIsCA = hoso.OnIsCA,
                                 IsHoSoBS = hoso.IsHoSoBS,
                                 IsHoSoUuTien = hoso.IsHoSoUuTien,

                                 //Loại hồ sơ
                                 LoaiHoSoId = hoso.LoaiHoSoId,
                                 StrLoaiHoSo = hoso.TenLoaiHoSo,
                                 StrThuMucHoSo = hoso.StrThuMucHoSo,

                                 //Doanh nghiệp
                                 DoanhNghiepId = hoso.DoanhNghiepId,
                                 TenDoanhNghiep = hoso.TenDoanhNghiep,
                                 TinhId = hoso.TinhId,
                                 TenNguoiDaiDien = hoso.TenNguoiDaiDien,

                                 //Kế toán
                                 NgayThanhToan = hoso.NgayThanhToan,
                                 NgayXacNhanThanhToan = hoso.NgayXacNhanThanhToan,
                                 KeToanId = hoso.KeToanId,

                                 //Nộp hồ sơ
                                 NgayTiepNhan = hsxl.NgayTiepNhan,
                                 NgayHenTra = hsxl.NgayHenTra,

                                 //Trạng thái hồ sơ
                                 HoSoXuLyId_Active = hoso.HoSoXuLyId_Active,
                                 TrangThaiHoSo = hoso.TrangThaiHoSo,

                                 //Một cửa phân công
                                 IsChuyenAuto = hoso.IsChuyenAuto,
                                 NgayChuyenAuto = hoso.NgayChuyenAuto,
                                 MotCuaChuyenId = hoso.MotCuaChuyenId,
                                 NgayMotCuaChuyen = hoso.NgayMotCuaChuyen,
                                 PhongBanId = hoso.PhongBanId,

                                 //Kết quả xử lý
                                 HoSoIsDat = hsxl.HoSoIsDat,
                                 TrangThaiCV = hsxl.TrangThaiCV,
                                 DonViKeTiep = hsxl.DonViKeTiep,

                                 //Người xử lý
                                 LanhDaoBoId = hsxl.LanhDaoBoId,
                                 LanhDaoBoDaDuyet = hsxl.LanhDaoBoDaDuyet,

                                 LanhDaoCucId = hsxl.LanhDaoCucId,
                                 LanhDaoCucDaDuyet = hsxl.LanhDaoCucDaDuyet,

                                 TruongPhongId = hsxl.TruongPhongId,
                                 TruongPhongDaDuyet = hsxl.TruongPhongDaDuyet,

                                 VanThuId = hsxl.VanThuId,
                                 VanThuDaDuyet = hsxl.VanThuDaDuyet,

                                 PhoPhongId = hsxl.PhoPhongId,
                                 PhoPhongDaDuyet = hsxl.PhoPhongDaDuyet,

                                 ChuyenVienThuLyId = hsxl.ChuyenVienThuLyId,
                                 ChuyenVienThuLyDaDuyet = hsxl.ChuyenVienThuLyDaDuyet,

                                 ChuyenVienPhoiHopId = hsxl.ChuyenVienPhoiHopId,
                                 ChuyenVienPhoiHopDaDuyet = hsxl.ChuyenVienPhoiHopDaDuyet,

                                 ChuyenGiaId = hsxl.ChuyenGiaId,
                                 ChuyenGiaDaDuyet = hsxl.ChuyenGiaDaDuyet,

                                 ToTruongChuyenGiaId = hsxl.ToTruongChuyenGiaId,
                                 ToTruongChuyenGiaDaDuyet = hsxl.ToTruongChuyenGiaDaDuyet,

                                 //Đơn vị gửi
                                 DonViXuLy = hsxl.DonViXuLy,
                                 DonViGui = hsxl.DonViGui,
                                 NgayGui = hsxl.NgayGui,
                                 NguoiGuiId = hsxl.NguoiGuiId,
                                 NguoiXuLyId = hsxl.NguoiXuLyId,

                                 //Thông tin đang xử lý
                                 HoSoXuLyHistoryId_Active = hsxl.HoSoXuLyHistoryId_Active,

                                 //File PDF
                                 GiayTiepNhan = hoso.GiayTiepNhan,
                                 DuongDanTepCA = hoso.DuongDanTepCA,
                                 HsxlDuongDanTepCA = hsxl.DuongDanTepCA,

                                 //Trạng thái ký số
                                 TruongPhongIsCA = hsxl.TruongPhongIsCA,
                                 LanhDaoCucIsCA = hsxl.LanhDaoCucIsCA,
                                 LanhDaoBoIsCA = hsxl.LanhDaoBoIsCA,
                                 VanThuIsCA = hsxl.VanThuIsCA,

                                 //Sắp xếp
                                 CreationTime = hoso.CreationTime,   
                                 LastModifierUserId = hoso.LastModifierUserId,
                                 

                             })
                             .WhereIf(!string.IsNullOrEmpty(input.Keyword), x => x.SoDangKy.LocDauLowerCaseDB().Contains(input.Keyword.LocDauLowerCaseDB())
                                                                             || x.MaHoSo.LocDauLowerCaseDB().Contains(input.Keyword)
                                                                             || x.TenDoanhNghiep.LocDauLowerCaseDB().Contains(input.Keyword.LocDauLowerCaseDB())
                                                                             || x.Id == hoSoId)
                             .WhereIf(input.NgayNopTu.HasValue && input.NgayNopToi.HasValue, x => x.NgayTiepNhan >= NgayNopTu && x.NgayTiepNhan <= NgayNopToi)
                             .WhereIf(_mySession.UserSession.DoanhNghiepId.HasValue,x=>x.DoanhNghiepId == _mySession.UserSession.DoanhNghiepId)
                             .WhereIf(input.TinhId.HasValue, x => x.TinhId == input.TinhId.Value);
                var a = query.ToList();
                #region Truy vấn cho từng form 
                switch ((CommonENum.FORM_ID)input.FormId)
                {
                    case CommonENum.FORM_ID.FORM_DANG_KY_HO_SO:
                        query = FormDangKyHoSo(query, input);
                        break;
                    case CommonENum.FORM_ID.FORM_MOT_CUA_RA_SOAT:
                        query = FormMotCuaRaSoat(query, input);
                        break;
                    case CommonENum.FORM_ID.FORM_MOT_CUA_PHAN_CONG:
                        query = FormMotCuaPhanCong(query, input);
                        break;
                    case CommonENum.FORM_ID.FORM_PHONG_BAN_PHAN_CONG:
                        query = FormPhongBanPhanCong(query, input);
                        break;
                    case CommonENum.FORM_ID.FORM_THAM_XET_HO_SO:
                        query = FormThamDinhHoSo(query, input);
                        break;
                    case CommonENum.FORM_ID.FORM_PHO_PHONG_DUYET:
                        query = FormPhoPhongDuyet(query, input);
                        break;
                    case CommonENum.FORM_ID.FORM_TRUONG_PHONG_DUYET:
                        query = FormTruongPhongDuyet(query, input);
                        break;
                    case CommonENum.FORM_ID.FORM_LANH_DAO_CUC_DUYET:
                        query = FormLanhDaoCucDuyet(query, input);
                        break;
                    case CommonENum.FORM_ID.FORM_VAN_THU_DUYET:
                        query = FormVanThuDuyet(query, input);
                        break;
                    case CommonENum.FORM_ID.FORM_TRA_CUU_HO_SO:
                        query = FormTraCuuHoSo(query, input);
                        break;
                }
                #endregion
                return query;
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                throw;
            }
        }
        //SUB
        private IQueryable<XHoSoDto> FormDangKyHoSo(IQueryable<XHoSoDto> query, XHoSoInputDto input)
        {
            if (input.FormCase == (int)CommonENum.FORM_CASE_DANG_KY_HO_SO.HO_SO_MOI)
            {
                query = query.Where(p => (p.TrangThaiHoSo.HasValue ? p.TrangThaiHoSo : 0) == (int)CommonENum.TRANG_THAI_HO_SO.DA_LUU);
            }
            if (input.FormCase == (int)CommonENum.FORM_CASE_DANG_KY_HO_SO.HO_SO_DANG_XU_LY)
            {
                query = query.Where(p => p.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.CHO_TRA_GIAY_TIEP_NHAN || p.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_DA_TIEP_NHAN || p.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_BO_SUNG);
            }
            if (input.FormCase == (int)CommonENum.FORM_CASE_DANG_KY_HO_SO.HO_SO_CAN_BO_SUNG)
            {
                query = query.Where(p => p.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.SUA_DOI_BO_SUNG);
            }
            if (input.FormCase == (int)CommonENum.FORM_CASE_DANG_KY_HO_SO.HO_SO_HOAN_TAT)
            {
                query = query.Where(p => ((p.TrangThaiHoSo != null ? p.TrangThaiHoSo.Value : 0) == (int)CommonENum.TRANG_THAI_HO_SO.DA_HOAN_TAT)
                || p.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.TU_CHOI_CAP_PHEP);
            }
            return query;
        }
        private IQueryable<XHoSoDto> FormMotCuaRaSoat(IQueryable<XHoSoDto> query, XHoSoInputDto input)
        {
            query = query.Where(x => x.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_DA_TIEP_NHAN
                                     || x.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.SUA_DOI_BO_SUNG
                                     || x.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_BO_SUNG
                                     || x.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.TU_CHOI_CAP_PHEP
                                     || x.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_HOAN_TAT);
            if (input.FormCase == (int)CommonENum.FORM_CASE_MOT_CUA_RA_SOAT.HO_SO_CAN_RA_SOAT)
            {
                query = query.Where(c => c.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_DA_TIEP_NHAN || c.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_BO_SUNG);
            }
            if (input.FormCase == (int)CommonENum.FORM_CASE_MOT_CUA_RA_SOAT.HO_SO_BI_TRA_LAI)
            {
                query = query.Where(c => c.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.SUA_DOI_BO_SUNG && c.HoSoIsDat != true);
            }
            else if (input.FormCase == (int)CommonENum.FORM_CASE_MOT_CUA_RA_SOAT.HO_SO_DA_TRA_GIAY_TIEP_NHAN)
            {
                query = query.Where(c => c.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_HOAN_TAT || c.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.TU_CHOI_CAP_PHEP);
            }
            return query;
        }
        private IQueryable<XHoSoDto> FormMotCuaPhanCong(IQueryable<XHoSoDto> query, XHoSoInputDto input)
        {
            var _listPhongBanLoaiHoSo = (from ploai in _phongBanLoaiHoSoRepos.GetAll()
                                         join loaihs in _loaiHoSoRepos.GetAll() on ploai.LoaiHoSoId equals loaihs.Id
                                         where loaihs.ThuTucId == _thuTucId
                                         select new PhongBanLoaiHoSo98Dto
                                         {
                                             LoaiHoSoId = ploai.LoaiHoSoId,
                                             PhongBanId = ploai.PhongBanId
                                         }).ToList();
            query = query.Where(p => p.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_BO_SUNG || p.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_DA_TIEP_NHAN || p.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_HO_SO_MOI);
            var listPhongBanLoaiHoSoNotAuto = _listPhongBanLoaiHoSo
                        .GroupBy(n => n.LoaiHoSoId)
                        .Select(n => new
                        {
                            LoaiHoSoId = n.Key,
                            SoLuongPBXuLy = n.Count()
                        })
                        .Where(x => x.SoLuongPBXuLy > 1)
                        .Select(p => p.LoaiHoSoId).ToList();

            if (listPhongBanLoaiHoSoNotAuto.Count > 0)
            {
                query = query.Where(p => listPhongBanLoaiHoSoNotAuto.Any(id => id == p.LoaiHoSoId));
            }
            if (input.FormCase == (int)CommonENum.FORM_CASE_MOT_CUA_PHAN_CONG.HO_SO_NOP_MOI)
            {
                query = query.Where(c => c.IsHoSoBS != true && c.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.MOT_CUA_TIEP_NHAN);
            }
            //if (input.FormCase == (int)CommonENum.FORM_CASE_MOT_CUA_PHAN_CONG.HO_SO_NOP_BO_SUNG)
            //{
            //    query = query.Where(p => p.IsHoSoBS == true && p.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.MOT_CUA_TIEP_NHAN);
            //}
            else if (input.FormCase == (int)CommonENum.FORM_CASE_MOT_CUA_PHAN_CONG.HO_SO_DA_TIEP_NHAN)
            {
                query = query.Where(p => p.PhongBanId.HasValue && p.IsChuyenAuto != true
                && p.DonViXuLy != (int)CommonENum.DON_VI_XU_LY.MOT_CUA_TIEP_NHAN && p.DonViXuLy != (int)CommonENum.DON_VI_XU_LY.DOANH_NGHIEP);
            }
            return query;
        }
        private IQueryable<XHoSoDto> FormPhongBanPhanCong(IQueryable<XHoSoDto> query, XHoSoInputDto input)
        {
            query = query.Where(p => p.PhongBanId == _mySession.UserSession.PhongBanId);
            query = query.Where(p => p.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_DA_TIEP_NHAN || p.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_BO_SUNG);
            if (input.FormCase == (int)CommonENum.FORM_CASE_PHONG_BAN_PHAN_CONG.CHUA_PHAN_CONG)
            {
                query = query.Where(p => p.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.PHONG_BAN_PHAN_CONG);
            }
            else if (input.FormCase == (int)CommonENum.FORM_CASE_PHONG_BAN_PHAN_CONG.DA_PHAN_CONG)
            {
                query = query.Where(p => (p.DonViXuLy != null ? p.DonViXuLy.Value : 0) != (int)CommonENum.DON_VI_XU_LY.PHONG_BAN_PHAN_CONG && (p.ChuyenVienThuLyDaDuyet != true));
            }
            else if (input.FormCase == (int)CommonENum.FORM_CASE_PHONG_BAN_PHAN_CONG.DA_XU_LY)
            {
                query = query.Where(p => (p.DonViXuLy != null ? p.DonViXuLy.Value : 0) != (int)CommonENum.DON_VI_XU_LY.PHONG_BAN_PHAN_CONG && (p.ChuyenVienThuLyDaDuyet == true || p.ChuyenVienPhoiHopDaDuyet == true));
            }
            return query;
        }
        private IQueryable<XHoSoDto> FormThamDinhHoSo(IQueryable<XHoSoDto> query, XHoSoInputDto input)
        {
            var chuyenVienId = _session.UserId;
            query = query.Where(p => p.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_DA_TIEP_NHAN || p.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_BO_SUNG);
            query = query.Where(p => p.ChuyenVienThuLyId == chuyenVienId || (p.ChuyenVienPhoiHopId == chuyenVienId));

            if (input.FormCase == (int)CommonENum.FORM_CASE_THAM_XET_HO_SO.HO_SO_THAM_XET_MOI)
            {
                query = query
                    .Where(p => p.IsHoSoBS != true)
                    .Where(p =>
                                //CV2 chưa thẩm định
                                ((p.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET || p.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET_TONG_HOP) && (p.ChuyenVienPhoiHopId == chuyenVienId && p.ChuyenVienPhoiHopDaDuyet != true))
                                ||
                                //CV1 chưa thẩm định
                                ((p.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET || p.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET_TONG_HOP) && (p.ChuyenVienThuLyId == chuyenVienId && p.ChuyenVienThuLyDaDuyet != true))
                                ||
                                //CV2 đã thẩm định & CV1 chưa tổng hợp
                                (p.DonViGui == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_PHOI_HOP_THAM_XET &&
                                p.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET_TONG_HOP && (p.ChuyenVienThuLyId == chuyenVienId && p.ChuyenVienThuLyDaDuyet != true))
                    );
            }
            if (input.FormCase == (int)CommonENum.FORM_CASE_THAM_XET_HO_SO.HO_SO_DA_THAM_XET)
            {
                query = query
                    .Where(p =>
                        //CV2 đã thẩm định
                        (p.ChuyenVienPhoiHopId == chuyenVienId && p.ChuyenVienPhoiHopDaDuyet == true && p.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET_TONG_HOP && (p.DonViGui == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET || p.DonViGui == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_PHOI_HOP_THAM_XET))
                        ||
                        //CV1 đã thẩm định
                        (p.ChuyenVienThuLyId == chuyenVienId && p.ChuyenVienThuLyDaDuyet == true && p.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET_TONG_HOP && (p.DonViGui == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET || p.DonViGui == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_PHOI_HOP_THAM_XET))
                );
            }
            if (input.FormCase == (int)CommonENum.FORM_CASE_THAM_XET_HO_SO.HO_SO_THAM_XET_BO_SUNG)
            {
                query = query
                   //CV1 Chưa thẩm định or CV2 chưa thẩm định
                   .Where(p => p.IsHoSoBS == true &&
                        ((p.ChuyenVienThuLyId == chuyenVienId && p.ChuyenVienThuLyDaDuyet != true &&
                            ((p.DonViGui == (int)CommonENum.DON_VI_XU_LY.PHONG_BAN_PHAN_CONG && p.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET)
                                || (p.DonViGui == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_PHOI_HOP_THAM_XET && p.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET_TONG_HOP)))
                        ||
                        (p.ChuyenVienPhoiHopId == chuyenVienId && p.ChuyenVienPhoiHopDaDuyet != true &&
                            ((p.DonViGui == (int)CommonENum.DON_VI_XU_LY.PHONG_BAN_PHAN_CONG && p.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET)
                            || (p.DonViGui == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET && p.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET_TONG_HOP))))
               );
            }
            if (input.FormCase == (int)CommonENum.FORM_CASE_THAM_XET_HO_SO.HO_SO_THAM_XET_LAI)
            {
                query = query
                    //CV1 Đã thẩm định
                    .Where(p => ((p.ChuyenVienThuLyId == chuyenVienId || p.ChuyenVienPhoiHopId == chuyenVienId) && p.ChuyenVienThuLyDaDuyet == true)
                    //Thẩm định lại
                    && ((p.DonViGui == (int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG || p.DonViGui == (int)CommonENum.DON_VI_XU_LY.PHO_PHONG) && p.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET_TONG_HOP)
                );
            }
            if (input.FormCase == (int)CommonENum.FORM_CASE_THAM_XET_HO_SO.HO_SO_DANG_THEO_DOI)
            {
                query = query
                    //CV2 Đã thẩm định
                    .Where(p =>
                    //CV1 đã thẩm định và Không là Thẩm định lại và Không là chưa tổng hợp
                    (
                        ((p.ChuyenVienPhoiHopId == chuyenVienId && p.ChuyenVienPhoiHopDaDuyet == true) || (p.ChuyenVienThuLyId == chuyenVienId && p.ChuyenVienThuLyDaDuyet == true))
                            && !(p.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET_TONG_HOP && (p.DonViGui == (int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG || p.DonViGui == (int)CommonENum.DON_VI_XU_LY.PHO_PHONG))
                            && !(p.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET_TONG_HOP && (p.DonViGui == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET || p.DonViGui == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_PHOI_HOP_THAM_XET))
                    )
                );
            }

            if (input.FormCase2.HasValue)
            {
                switch (input.FormCase2)
                {
                    case (int)CommonENum.FORM_CASE2_THAM_XET_HO_SO.VI_TRI_CHUYEN_VIEN_THU_LY:
                        query = query.Where(p => p.ChuyenVienThuLyId == chuyenVienId);
                        break;
                    case (int)CommonENum.FORM_CASE2_THAM_XET_HO_SO.VI_TRI_CHUYEN_VIEN_PHOI_HOP:
                        query = query.Where(p => p.ChuyenVienPhoiHopId == chuyenVienId);
                        break;
                }
            }

            return query;
        }
        private IQueryable<XHoSoDto> FormPhoPhongDuyet(IQueryable<XHoSoDto> query, XHoSoInputDto input)
        {
            var phoPhongId = _session.UserId;

            query = query.Where(
                    p => p.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_DA_TIEP_NHAN
                    || p.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_BO_SUNG
                );

            query = query.Where(p => p.PhoPhongId == phoPhongId && (p.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.PHO_PHONG || p.PhoPhongDaDuyet == true));

            if (input.FormCase == (int)CommonENum.FORM_CASE_PHO_PHONG_DUYET.HO_SO_CHUA_DUYET)
            {
                query = query.Where(
                        p => p.IsHoSoBS != true && p.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.PHO_PHONG && p.PhoPhongDaDuyet != true && p.DonViGui == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET_TONG_HOP
                    );
            }
            if (input.FormCase == (int)CommonENum.FORM_CASE_PHO_PHONG_DUYET.HO_SO_DA_DUYET)
            {
                query = query.Where(
                        p => p.DonViXuLy != (int)CommonENum.DON_VI_XU_LY.PHO_PHONG && p.PhoPhongDaDuyet == true && p.DonViGui != (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET
                        || (p.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG && p.PhoPhongDaDuyet == true && p.DonViGui == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET)
                    );
            }
            return query;
        }
        private IQueryable<XHoSoDto> FormTruongPhongDuyet(IQueryable<XHoSoDto> query, XHoSoInputDto input)
        {
            
            query = query.Where(
                    p => p.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_HO_SO_MOI
                    || p.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_HOAN_TAT
                    || p.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_TRA_LAI
                );
            //query = query.Where(p => p.TruongPhongId == truongPhongId && (p.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG || p.TruongPhongDaDuyet == true));

            if (input.FormCase == (int)CommonENum.FORM_CASE_TRUONG_PHONG_DUYET.HO_SO_CHUA_XU_LY)
            {
                query = query.Where(c => c.TruongPhongDaDuyet != true);
            }
            if (input.FormCase == (int)CommonENum.FORM_CASE_TRUONG_PHONG_DUYET.HO_SO_DANG_THEO_DOI)
            {
                query = query.Where(x=>x.TruongPhongDaDuyet == true);
            }
            return query;
        }
        private IQueryable<XHoSoDto> FormLanhDaoCucDuyet(IQueryable<XHoSoDto> query, XHoSoInputDto input)
        {
            var lanhDaoCucId = _session.UserId;

            query = query.Where(
                    p => p.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_DA_TIEP_NHAN
                    || p.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_BO_SUNG
                    || p.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_HOAN_TAT
                );
            query = query.Where(p => p.LanhDaoCucId == lanhDaoCucId);

            if (input.FormCase == (int)CommonENum.FORM_CASE_LANH_DAO_CUC_DUYET.HO_SO_CHO_DUYET)
            {
                query = query.Where(
                        p => p.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.LANH_DAO_CUC
                    );
            }
            if (input.FormCase == (int)CommonENum.FORM_CASE_LANH_DAO_CUC_DUYET.HO_SO_TRA_LAI)
            {
                query = query.Where(
                       p => (p.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET || p.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.PHO_PHONG
                       || p.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG) && p.LanhDaoCucDaDuyet == true
                    );
            }
            if (input.FormCase == (int)CommonENum.FORM_CASE_LANH_DAO_CUC_DUYET.HO_SO_DA_DUYET)
            {
                query = query.Where(
                        p => p.DonViXuLy != (int)CommonENum.DON_VI_XU_LY.LANH_DAO_CUC
                        && !((p.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET || p.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.PHO_PHONG
                        || p.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG) && p.LanhDaoCucDaDuyet == true)
                     );
            }
            return query;
        }
        private IQueryable<XHoSoDto> FormVanThuDuyet(IQueryable<XHoSoDto> query, XHoSoInputDto input)
        {
            var vanThuId = _session.UserId;
            query = query.Where(
                    p => p.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_LUU
                    && p.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.THANH_TOAN_THAT_BAI
                );
            query = query.Where(
                p => p.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.VAN_THU
                || p.DonViGui == (int)CommonENum.DON_VI_XU_LY.VAN_THU
                );
            if (input.FormCase == (int)CommonENum.FORM_CASE_VAN_THU_DUYET.HO_SO_CHUA_DUYET)
            {
                query = query.Where(p => p.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.VAN_THU && p.VanThuIsCA != true);
            }
            if (input.FormCase == (int)CommonENum.FORM_CASE_VAN_THU_DUYET.HO_SO_DA_DUYET)
            {
                query = query.Where(p => p.DonViXuLy != (int)CommonENum.DON_VI_XU_LY.VAN_THU && p.VanThuIsCA == true);
            }
            return query;
        }
        private IQueryable<XHoSoDto> FormTraCuuHoSo(IQueryable<XHoSoDto> query, XHoSoInputDto input)
        {
            query = query.Where(
                            p => p.TrangThaiHoSo != null
                        );

            if (input.FormCase == (int)CommonENum.FORM_CASE_TRA_CUU_HO_SO.HO_SO_MOI)
            {
                query = query.Where(
                             p => p.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_LUU
                         );
            }
            if (input.FormCase == (int)CommonENum.FORM_CASE_TRA_CUU_HO_SO.HO_SO_DANG_XU_LY)
            {
                query = query.Where(
                          p => p.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_LUU
                          && p.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.SUA_DOI_BO_SUNG
                          && p.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_HOAN_TAT
                      );
            }
            if (input.FormCase == (int)CommonENum.FORM_CASE_TRA_CUU_HO_SO.HO_SO_YEU_CAU_BO_SUNG)
            {
                query = query.Where(
                              p => p.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.SUA_DOI_BO_SUNG
                          );
            }
            if (input.FormCase == (int)CommonENum.FORM_CASE_TRA_CUU_HO_SO.HO_SO_HOAN_THANH)
            {
                query = query.Where(
                              p => p.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_HOAN_TAT
                          );
            }

            return query;
        }
        #endregion

        #region Function Chức năng cho FORM
        private List<int> ArrayChucNangGridHoSo(XHoSoDto item, XHoSoInputDto input)
        {
            List<int> _arrChucNang = new List<int>();
            //FORM 1
            if (input.FormId == (int)CommonENum.FORM_ID.FORM_DANG_KY_HO_SO)
            {
                if (item.TrangThaiHoSo != null)
                {
                    _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.XEM_HO_SO);
                    _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.XEM_BAN_DANG_KY);
                    _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.TAO_BAN_SAO_HO_SO);
                }
                if (item.IsCA != true && item.TrangThaiHoSo != null && item.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_HOAN_TAT)
                {
                    _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.DOANH_NGHIEP_KY_SO_HO_SO);
                }
                if (item.IsCA == true && item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_HOAN_TAT)
                {
                    _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.XEM_CONG_VAN);
                }
                if (item.IsCA == true && item.OnIsCA == true)
                {
                    _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.DOANH_NGHIEP_KY_SO_HO_SO);
                }
                if (item.IsHoSoBS == true)
                {
                    if (item.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_HOAN_TAT)
                    {
                        _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.XEM_CONG_VAN);
                    }

                    if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.SUA_DOI_BO_SUNG)
                    {
                        _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.SUA_HO_SO);
                        if (item.IsCA == true && item.LastModifierUserId == _session.UserId)
                        {
                            _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.NOP_HO_SO_BO_SUNG);
                        }
                    }
                }
                else
                {
                    if ((item.TrangThaiHoSo.HasValue ? item.TrangThaiHoSo : 0) == (int)CommonENum.TRANG_THAI_HO_SO.DA_LUU)
                    {
                        _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.SUA_HO_SO);
                        _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.HUY_HO_SO);
                        if (item.IsCA == true)
                            _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.DOANH_NGHIEP_NOP_HO_SO);

                    }
                    if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.CHO_KE_TOAN_XAC_NHAN)
                    {
                        _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.XEM_THANH_TOAN_HO_SO);
                    }
                    if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.THANH_TOAN_THAT_BAI)
                    {
                        _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.NOP_HO_SO_THANH_TOAN_THAT_BAI);
                        _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.XEM_THANH_TOAN_HO_SO);
                        _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.SUA_HO_SO);
                        _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.HUY_HO_SO);
                    }
                    if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_TRA_LAI)
                    {
                        _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.SUA_HO_SO);
                        _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.HUY_HO_SO);
                        _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.NOP_HO_SO_BI_TRA_LAI);
                        _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.XEM_CONG_VAN);
                    }
                    if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.CHO_DOANH_NGHIEP_THANH_TOAN)
                    {
                        _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.HUY_HO_SO);
                        _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.DOANH_NGHIEP_THANH_TOAN);
                    }

                }
            }

            //FORM 22
            else if (input.FormId == (int)CommonENum.FORM_ID.FORM_MOT_CUA_RA_SOAT)
            {
                _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.XEM_HO_SO);
                if (item.FormCase == (int)CommonENum.FORM_CASE_MOT_CUA_RA_SOAT.HO_SO_CAN_RA_SOAT)
                {
                    _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.MOT_CUA_RA_SOAT_HO_SO);
                }
                if(item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_TRA_LAI || item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_HOAN_TAT)
                {
                    _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.XEM_CONG_VAN);
                }
            }

            //FORM 2
            else if (input.FormId == (int)CommonENum.FORM_ID.FORM_MOT_CUA_PHAN_CONG)
            {
                _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.XEM_HO_SO);
                _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.XEM_BAN_DANG_KY);

                if (!item.TruongPhongId.HasValue)
                {
                    _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.MOT_CUA_PHAN_CONG_HO_SO);
                }
            }

            //FORM 21
            else if (input.FormId == (int)CommonENum.FORM_ID.FORM_PHONG_BAN_PHAN_CONG)
            {
                _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.XEM_HO_SO);
                _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.XEM_BAN_DANG_KY);

                if (!item.DonViXuLy.HasValue || item.DonViXuLy.Value == (int)CommonENum.DON_VI_XU_LY.PHONG_BAN_PHAN_CONG)
                {
                    _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.PHONG_BAN_PHAN_CONG);
                }
                else if (item.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET && item.DonViGui == (int)CommonENum.DON_VI_XU_LY.PHONG_BAN_PHAN_CONG)
                {
                    _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.PHAN_CONG_LAI_HO_SO_CHUA_XU_LY);
                }
                else
                {
                    //_arrChucNang.Add((int)CommonENum.FORM_FUNCTION.PHAN_CONG_LAI_HO_SO_DA_XU_LY);
                }
            }

            //FORM 3
            else if (input.FormId == (int)CommonENum.FORM_ID.FORM_THAM_XET_HO_SO)
            {
                var chuyenVienId = _session.UserId;

                _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.XEM_HO_SO);
                _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.XEM_BAN_DANG_KY);

                if (input.FormCase == (int)CommonENum.FORM_CASE_THAM_XET_HO_SO.HO_SO_THAM_XET_MOI)
                {
                    _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.THAM_XET_HO_SO_MOI);
                }
                if (input.FormCase == (int)CommonENum.FORM_CASE_THAM_XET_HO_SO.HO_SO_DA_THAM_XET)
                {
                    if (item.ChuyenVienThuLyDaDuyet == true && item.ChuyenVienPhoiHopDaDuyet == true && item.ChuyenVienThuLyId == chuyenVienId)
                    {
                        _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.CHUYEN_VIEN_TONG_HOP_THAM_DINH);
                    }
                }
                if (input.FormCase == (int)CommonENum.FORM_CASE_THAM_XET_HO_SO.HO_SO_THAM_XET_LAI)
                {
                    _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.THAM_XET_LAI);
                }
                else if (input.FormCase == (int)CommonENum.FORM_CASE_THAM_XET_HO_SO.HO_SO_THAM_XET_BO_SUNG)
                {
                    _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.THAM_XET_HO_SO_BO_SUNG);
                }
            }

            //FORM 31
            else if (input.FormId == (int)CommonENum.FORM_ID.FORM_PHO_PHONG_DUYET)
            {
                _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.XEM_HO_SO);
                _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.XEM_BAN_DANG_KY);
                if (item.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.PHO_PHONG)
                {
                    _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.PHO_PHONG_DUYET);
                }
            }

            //FORM 4
            else if (input.FormId == (int)CommonENum.FORM_ID.FORM_TRUONG_PHONG_DUYET)
            {
                _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.XEM_HO_SO);
                _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.XEM_BAN_DANG_KY);
                if (item.FormCase == (int)CommonENum.FORM_CASE_TRUONG_PHONG_DUYET.HO_SO_CHUA_XU_LY)
                {
                    _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.TRUONG_PHONG_DUYET);
                }
                if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_TRA_LAI || item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_HOAN_TAT)
                {
                    _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.XEM_CONG_VAN);
                }
            }

            //FORM 5
            else if (input.FormId == (int)CommonENum.FORM_ID.FORM_LANH_DAO_CUC_DUYET)
            {
                _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.XEM_HO_SO);
                _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.XEM_BAN_DANG_KY);
                if (input.FormCase == (int)CommonENum.FORM_CASE_LANH_DAO_CUC_DUYET.HO_SO_CHO_DUYET)
                {
                    _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.LANH_DAO_CUC_DUYET);
                    _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.LANH_DAO_CUC_KY_SO);
                }
            }

            //FORM 6
            else if (input.FormId == (int)CommonENum.FORM_ID.FORM_LANH_DAO_BO_DUYET)
            {
                _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.XEM_HO_SO);
                _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.XEM_BAN_DANG_KY);

                if (item.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.LANH_DAO_BO)
                {
                    _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.LANH_DAO_BO_DUYET);
                    if (item.HoSoIsDat == true)
                    {
                        _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.LANH_DAO_BO_KY_SO);
                    }
                    else
                    {
                        _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.XEM_CONG_VAN);
                    }
                }
            }

            //FORM 7
            else if (input.FormId == (int)CommonENum.FORM_ID.FORM_VAN_THU_DUYET)
            {
                _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.XEM_HO_SO);
                _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.XEM_BAN_DANG_KY);

                if (item.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.VAN_THU)
                {
                    if (item.HoSoIsDat == true)
                    {
                        _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.VAN_THU_DUYET);
                        _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.VAN_THU_DONG_DAU);
                    }
                    else
                    {
                        _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.XEM_CONG_VAN);
                        _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.VAN_THU_DUYET);
                        _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.VAN_THU_DONG_DAU);
                        _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.VAN_THU_BAO_CAO_CONG_VAN_CO_SAI_SOT);
                    }
                }
            }

            return _arrChucNang;
        }

        private string GetTrangThaiXuLyHoSo(XHoSoDto item)
        {
            string trangThaiXuLy = "";
            if (!item.TrangThaiHoSo.HasValue)
            {
                trangThaiXuLy = "Hồ sơ lưu nháp";
            }
            else if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_LUU)
            {
                trangThaiXuLy = "Hồ sơ soạn mới";
            }
            else if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.CHO_DOANH_NGHIEP_THANH_TOAN)
            {
                trangThaiXuLy = "Chờ thanh toán";
            }
            else if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_HOAN_TAT || item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.TU_CHOI_CAP_PHEP)
            {
                trangThaiXuLy = "Hồ sơ đã hoàn tất";
            }
            else if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.SUA_DOI_BO_SUNG)
            {
                trangThaiXuLy = "Hồ sơ cần bổ sung";
            }
            else if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.CHO_KE_TOAN_XAC_NHAN)
            {
                trangThaiXuLy = "Hồ sơ đã gửi thanh toán, đang chờ xác nhận";
            }
            else if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.THANH_TOAN_THAT_BAI)
            {
                trangThaiXuLy = "Hồ sơ thanh toán thất bại";
            }
            else if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_HO_SO_MOI)
            {
                trangThaiXuLy = "Hồ sơ chờ tiếp nhận";
            }
            else if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_TRA_LAI)
            {
                trangThaiXuLy = "Hồ sơ bị trả lại";
            }
            else
            {
                if (item.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.DOANH_NGHIEP && item.DonViGui == (int)CommonENum.DON_VI_XU_LY.MOT_CUA_TIEP_NHAN)
                {
                    trangThaiXuLy = "Hồ sơ bị trả lại";
                }
                else if (item.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.PHONG_BAN_PHAN_CONG && item.DonViGui == (int)CommonENum.DON_VI_XU_LY.MOT_CUA_TIEP_NHAN)
                {
                    trangThaiXuLy = "Hồ sơ được tiếp nhận";
                }
                else
                {
                    trangThaiXuLy = "Hồ sơ đang thẩm định";
                }
            }
            return trangThaiXuLy;
        }

        private SoNgayQuaHanChiTiet98Dto TinhSoNgayQuaHanChiTiet(int _soNgayLamViec, DateTime? _ngayGui)
        {
            int? soNgayQuaHanChiTiet;
            int? soGioQuaHanChiTiet = null;
            string strQuaHanChiTiet = "";

            DateTime ngayBegin = DateTime.Now;
            DateTime ngayEnd = new DateTime(_ngayGui.Value.Year, _ngayGui.Value.Month, _ngayGui.Value.Day + _soNgayLamViec, _ngayGui.Value.Hour, _ngayGui.Value.Minute, _ngayGui.Value.Second);
            if (ngayBegin > ngayEnd)
            {
                TimeSpan timeQuaHam = ngayBegin - ngayEnd;
                soNgayQuaHanChiTiet = timeQuaHam.Days;
                strQuaHanChiTiet = "Quá " + soNgayQuaHanChiTiet + " ngày XLHS";
                if (soNgayQuaHanChiTiet == 1 || (soNgayQuaHanChiTiet == 0 && timeQuaHam.Minutes > 0))
                {
                    soGioQuaHanChiTiet = timeQuaHam.Hours;
                    strQuaHanChiTiet = "Quá " + timeQuaHam.Hours.ToString() + "h" + timeQuaHam.Minutes.ToString() + "p" + " XLHS";
                }
                else if (soNgayQuaHanChiTiet == 0)
                {
                    strQuaHanChiTiet = "";
                }
            }
            else
            {
                soNgayQuaHanChiTiet = _lichLamViecAppService.GetSoNgayLamViec(ngayBegin, ngayEnd);
                strQuaHanChiTiet = "Còn " + soNgayQuaHanChiTiet + " ngày làm việc";
                TimeSpan time = ngayEnd - ngayBegin;
                if (soNgayQuaHanChiTiet == 1 || (soNgayQuaHanChiTiet == 0 && time.Minutes > 0))
                {
                    soGioQuaHanChiTiet = time.Hours;
                    strQuaHanChiTiet = "Còn " + time.Hours.ToString() + "h" + time.Minutes.ToString() + "p" + " XLHS";
                }
                else if (soNgayQuaHanChiTiet == 0)
                {
                    strQuaHanChiTiet = "";
                }
                soNgayQuaHanChiTiet = soNgayQuaHanChiTiet * (-1);
            }
            return new SoNgayQuaHanChiTiet98Dto()
            {
                SoGioQuaHanChiTiet = soGioQuaHanChiTiet,
                SoNgayQuaHanChiTiet = soNgayQuaHanChiTiet,
                StrQuaHanChiTiet = strQuaHanChiTiet
            };
        }

        #endregion        

        public async Task<dynamic> GetListFormCaseCountNumber(XHoSoInputDto input)
        {
            try
            {
                var _form = (CommonENum.FORM_ID)input.FormId.Value;
                var formCase = CommonEnumExtensions.GetListFormCase(_form);
                if (formCase != null && formCase.Count > 0)
                {
                    foreach (var item in formCase)
                    {
                        input.FormCase = item.Id;
                        input.IsOnlyToTal = true;
                        var query = QueryGetAllHoSo(input);
                        item.TotalCount = await query.CountAsync();
                    }
                }
                return formCase;
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                return null;
            }
        }
    }
}