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

using XHoSo = newPSG.PMS.EntityDB.HoSo37;
using XHoSoTepDinhKem = newPSG.PMS.EntityDB.HoSoTepDinhKem37;
using XHoSoXuLy = newPSG.PMS.EntityDB.HoSoXuLy37;
using XHoSoXuLyHistory = newPSG.PMS.EntityDB.HoSoXuLyHistory37;
using XHoSoDto = newPSG.PMS.Dto.HoSo37Dto;
using XHoSoInputDto = newPSG.PMS.Dto.HoSoInput37Dto;
using XHoSoTepDinhKemDto = newPSG.PMS.Dto.HoSoTepDinhKem37Dto;
using XHoSoXuLyDto = newPSG.PMS.Dto.HoSoXuLy37Dto;
using XHoSoXuLyHistoryDto = newPSG.PMS.Dto.HoSoXuLyHistory37Dto;
#endregion

namespace newPSG.PMS.Services
{
    public interface IXuLyHoSoGridView37AppService : IApplicationService
    {
        Task<PagedResultDto<XHoSoDto>> GetListHoSoPaging(XHoSoInputDto input);
        dynamic GetListFormCase(int _formId);
        dynamic GetListFormFunction();
        Task<dynamic> GetListFormCaseCountNumber(XHoSoInputDto input);
    }
    [AbpAuthorize]
    public class XuLyHoSoGridView37AppService : PMSAppServiceBase, IXuLyHoSoGridView37AppService
    {
        private readonly int _thuTucId = (int)CommonENum.THU_TUC_ID.THU_TUC_37;
        private readonly IRepository<XHoSo, long> _hoSoRepos;
        private readonly IRepository<XHoSoXuLy, long> _hoSoXuLyRepos;
        private readonly IRepository<LoaiHoSo> _loaiHoSoRepos;
        private readonly IRepository<PhongBan> _phongBanRepos;
        private readonly IRepository<PhongBanLoaiHoSo> _phongBanLoaiHoSoRepos;
        private readonly IRepository<User, long> _userRepos;
        private readonly IAbpSession _session;
        private readonly ILichLamViecAppService _lichLamViecAppService;
        private readonly CustomSessionAppSession _mySession;
        private readonly IRepository<TT37_HoSoDoanThamDinh, long> _hoSoDoanThamDinh;

        public XuLyHoSoGridView37AppService(IRepository<XHoSo, long> hoSoRepos,
                                            IRepository<XHoSoXuLy, long> hoSoXuLyRepos,
                                            IRepository<LoaiHoSo> loaiHoSoRepos,
                                            IRepository<PhongBan> phongBanRepos,
                                            IRepository<PhongBanLoaiHoSo> phongBanLoaiHoSoRepos,
                                            IRepository<User, long> userRepos,
                                            IAbpSession session,
                                            ILichLamViecAppService lichLamViecAppService,
                                            CustomSessionAppSession mySession,
                                            IRepository<TT37_HoSoDoanThamDinh, long> hoSoDoanThamDinh)
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
            _hoSoDoanThamDinh = hoSoDoanThamDinh;
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
                    else if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.SUA_DOI_BO_SUNG && item.VanThuIsCA == true)
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
                                 HoTenNguoiDeNghi = hoso.HoTenNguoiDeNghi,
                                 DiaChiCuTru = hoso.DiaChiCuTru,
                                 DienThoaiNguoiDeNghi = hoso.DienThoaiNguoiDeNghi,
                                 EmailNguoiDeNghi = hoso.EmailNguoiDeNghi,

                                 // info hồ sơ
                                 NgayDeNghi = hoso.NgayDeNghi,
                                 NgaySinh = hoso.NgaySinh,
                                 NgayCap = hoso.NgayCap,
                                 NoiCap = hoso.NoiCap,
                                 VanBangChuyenMon = hoso.VanBangChuyenMon,
                                 DiaChi = hoso.DiaChi,
                                 SoDienThoai = hoso.SoDienThoai,
                                 Email = hoso.Email,

                                 // hsxl
                                 NoiDungCV = hsxl.NoiDungCV,
                                 LyDoTraLai = hsxl.LyDoTraLai,
                                 TrangThaiXuLy = hsxl.TrangThaiXuLy,
                                 LuongXuLy = hsxl.LuongXuLy,

                                 SoCongVan = hsxl.SoCongVan,
                                 NgayYeuCauBoSung = hsxl.NgayYeuCauBoSung,
                                 NoiDungYeuCauGiaiQuyet = hsxl.NoiDungYeuCauGiaiQuyet,
                                 LyDoYeuCauBoSung = hsxl.LyDoYeuCauBoSung,
                                 TenCanBoHoTro = hsxl.TenCanBoHoTro,
                                 DienThoaiCanBo = hsxl.DienThoaiCanBo,
                                 NgayLapDoanThamDinh = hsxl.NgayLapDoanThamDinh,
                                 NguoiLapDoanThamDinhId = hsxl.NguoiLapDoanThamDinhId,
                                 // một cửa trả giấy tiếp nhận
                                 NgayHenCap = hsxl.NgayHenCap,
                                 HinhThucCapCTJson = hsxl.HinhThucCapCTJson,
                                 TaiLieuDaNhanJson = hsxl.TaiLieuDaNhanJson,
                                 GiayTiepNhanCA = hsxl.GiayTiepNhanCA,
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
                             .WhereIf(_mySession.UserSession.DoanhNghiepId.HasValue, x => x.DoanhNghiepId == _mySession.UserSession.DoanhNghiepId)
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
                        query = FormThamXetHoSo(query, input);
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
                    case CommonENum.FORM_ID.FORM_THAM_DINH_HO_SO_TT37:
                        query = FormThamDinhHoSo(query, input);
                        break;
                    case CommonENum.FORM_ID.FORM_TONG_HOP_THAM_DINH_TT37:
                        query = FormTongHopThamDinhHoSo(query, input);
                        break;
                    case CommonENum.FORM_ID.FORM_TRUONG_PHONG_DUYET_THAM_DINH_TT37:
                        query = FormTruongPhongDuyetThamDinh(query, input);
                        break;
                    case CommonENum.FORM_ID.FORM_LANH_DAO_CUC_DUYET_THAM_DINH_TT37:
                        query = FormLanhDaoCucDuyetThamDinh(query, input);
                        break;
                    case CommonENum.FORM_ID.FORM_VAN_THU_DUYET_THAM_DINH_TT37:
                        query = FormVanThuDuyetThamDinh(query, input);
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
            var doanhNghiepId = SessionCustom.UserCurrent.DoanhNghiepId;

            query = query.Where(p => p.DoanhNghiepId == doanhNghiepId);
            if (input.FormCase == (int)CommonENum.FORM_CASE_DANG_KY_HO_SO.HO_SO_MOI)
            {
                query = query.Where(p => (p.TrangThaiHoSo.HasValue ? p.TrangThaiHoSo : 0) == (int)CommonENum.TRANG_THAI_HO_SO.DA_LUU);
            }
            if (input.FormCase == (int)CommonENum.FORM_CASE_DANG_KY_HO_SO.HO_SO_CHO_TIEP_NHAN)
            {
                query = query.Where(p => p.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_HO_SO_MOI || p.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_BO_SUNG);
            }
            if (input.FormCase == (int)CommonENum.FORM_CASE_DANG_KY_HO_SO.HO_SO_BI_TRA_LAI)
            {
                query = query.Where(p => p.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_TRA_LAI);
            }

            if (input.FormCase == (int)CommonENum.FORM_CASE_DANG_KY_HO_SO.HO_SO_DANG_XU_LY)
            {
                query = query.Where(p => p.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_LUU
                && p.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_HO_SO_MOI
                 && p.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_BO_SUNG
                 && p.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.SUA_DOI_BO_SUNG
                 && p.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_HOAN_TAT
                 && p.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_TRA_LAI);
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
            query = query.Where(x => x.TrangThaiHoSo != null && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_LUU 
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_HOAN_TAT);

            var a = query.ToList();
            if (input.FormCase == (int)CommonENum.FORM_CASE_MOT_CUA_RA_SOAT.HO_SO_CAN_RA_SOAT)
            {
                query = query.Where(x => x.LuongXuLy == (int)CommonENum.LUONG_XU_LY_TT37.LUONG_RA_SOAT);
                query = query.Where(c => c.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_HO_SO_MOI || c.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_BO_SUNG);
            }
            else if (input.FormCase == (int)CommonENum.FORM_CASE_MOT_CUA_RA_SOAT.HO_SO_BI_TRA_LAI)
            {
                query = query.Where(x => x.LuongXuLy == (int)CommonENum.LUONG_XU_LY_TT37.LUONG_RA_SOAT);
                query = query.Where(c => c.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_TRA_LAI);

            }
            else if (input.FormCase == (int)CommonENum.FORM_CASE_MOT_CUA_RA_SOAT.HO_SO_DA_TRA_GIAY_TIEP_NHAN)
            {
                query = query.Where(c => c.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_HO_SO_MOI && c.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_BO_SUNG);
                query = query.Where(c => c.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_TRA_LAI);
            }
            return query;
        }
        private IQueryable<XHoSoDto> FormMotCuaPhanCong(IQueryable<XHoSoDto> query, XHoSoInputDto input)
        {
            query = query.Where(x => x.TrangThaiHoSo != null && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_LUU 
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_HOAN_TAT
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_HO_SO_MOI
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_BO_SUNG
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_TRA_LAI
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.CHO_TRA_GIAY_TIEP_NHAN);


            var _listPhongBanLoaiHoSo = (from ploai in _phongBanLoaiHoSoRepos.GetAll()
                                         join loaihs in _loaiHoSoRepos.GetAll() on ploai.LoaiHoSoId equals loaihs.Id
                                         where loaihs.ThuTucId == _thuTucId
                                         select new PhongBanLoaiHoSo37Dto
                                         {
                                             LoaiHoSoId = ploai.LoaiHoSoId,
                                             PhongBanId = ploai.PhongBanId
                                         }).ToList();
            
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
                query = query.Where(x => x.LuongXuLy == (int)CommonENum.LUONG_XU_LY_TT37.LUONG_RA_SOAT);
                query = query.Where(c => c.IsHoSoBS != true && (c.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_DA_TIEP_NHAN || c.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.PHONG_BAN_TU_CHOI_TIEP_NHAN_HO_SO));
            }
            //if (input.FormCase == (int)CommonENum.FORM_CASE_MOT_CUA_PHAN_CONG.HO_SO_NOP_BO_SUNG)
            //{
            //    query = query.Where(x => x.LuongXuLy == (int)CommonENum.LUONG_XU_LY_TT37.LUONG_RA_SOAT);
            //    query = query.Where(p => p.IsHoSoBS == true && p.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_DA_TIEP_NHAN);
            //}
            else if (input.FormCase == (int)CommonENum.FORM_CASE_MOT_CUA_PHAN_CONG.HO_SO_DA_TIEP_NHAN)
            {
                query = query.Where(p => p.PhongBanId.HasValue && p.IsChuyenAuto != true);
                query = query.Where(x=>x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_DA_TIEP_NHAN &&
                                       x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.PHONG_BAN_TU_CHOI_TIEP_NHAN_HO_SO);
            }
            return query;
        }
        private IQueryable<XHoSoDto> FormPhongBanPhanCong(IQueryable<XHoSoDto> query, XHoSoInputDto input)
        {
            query = query.Where(p => p.PhongBanId == _mySession.UserSession.PhongBanId);
            query = query.Where(x => x.TrangThaiHoSo != null && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_LUU
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_HOAN_TAT
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_HO_SO_MOI
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_BO_SUNG
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_TRA_LAI
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.CHO_TRA_GIAY_TIEP_NHAN
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_DA_TIEP_NHAN
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.PHONG_BAN_TU_CHOI_TIEP_NHAN_HO_SO);


            if (input.FormCase == (int)CommonENum.FORM_CASE_PHONG_BAN_PHAN_CONG.CHUA_PHAN_CONG)
            {
                query = query.Where(x => x.LuongXuLy == (int)CommonENum.LUONG_XU_LY_TT37.LUONG_RA_SOAT);
                query = query.Where(p => p.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.LANH_DAO_DA_PHAN_CONG_HO_SO 
                                      || p.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.CHUYEN_VIEN_TU_CHOI_TIEP_NHAN_HO_SO);
            }
            else if (input.FormCase == (int)CommonENum.FORM_CASE_PHONG_BAN_PHAN_CONG.DA_PHAN_CONG)
            {
                query = query.Where(x => x.LuongXuLy == (int)CommonENum.LUONG_XU_LY_TT37.LUONG_RA_SOAT);
                query = query.Where(p => p.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.PHONG_BAN_DA_PHAN_CONG_TOI_CHUYEN_VIEN);
            }
            else if (input.FormCase == (int)CommonENum.FORM_CASE_PHONG_BAN_PHAN_CONG.DA_XU_LY)
            {
                query = query.Where(p => p.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.PHONG_BAN_DA_PHAN_CONG_TOI_CHUYEN_VIEN);
                query = query.Where(p => p.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.LANH_DAO_DA_PHAN_CONG_HO_SO);
            }
            return query;
        }
        private IQueryable<XHoSoDto> FormThamXetHoSo(IQueryable<XHoSoDto> query, XHoSoInputDto input)
        {
            var chuyenVienId = _session.UserId;
            query = query.Where(p => p.ChuyenVienThuLyId == chuyenVienId || p.ChuyenVienPhoiHopId == chuyenVienId);
            query = query.Where(x => x.TrangThaiHoSo != null && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_LUU
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_HOAN_TAT
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_TRA_LAI
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_HO_SO_MOI
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_BO_SUNG
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.CHO_TRA_GIAY_TIEP_NHAN
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_DA_TIEP_NHAN
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.PHONG_BAN_TU_CHOI_TIEP_NHAN_HO_SO);
            var a = query.ToList();

            if (input.FormCase == (int)CommonENum.FORM_CASE_THAM_XET_HO_SO.HO_SO_THAM_XET_MOI)
            {
                query = query.Where(x => x.LuongXuLy == (int)CommonENum.LUONG_XU_LY_TT37.LUONG_RA_SOAT);
                query = query.Where(
                    p => p.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.PHONG_BAN_DA_PHAN_CONG_TOI_CHUYEN_VIEN
                    && p.DonViGui == (int)CommonENum.DON_VI_XU_LY.PHONG_BAN_PHAN_CONG
                );
            }
            if (input.FormCase == (int)CommonENum.FORM_CASE_THAM_XET_HO_SO.HO_SO_THAM_XET_LAI)
            {
                query = query.Where(x => x.LuongXuLy == (int)CommonENum.LUONG_XU_LY_TT37.LUONG_RA_SOAT);
                query = query.Where(p => p.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.HO_SO_THAM_XET_LAI && p.DonViGui == (int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG);
            }
            if (input.FormCase == (int)CommonENum.FORM_CASE_THAM_XET_HO_SO.HO_SO_DANG_THEO_DOI)
            {
                query = query.Where(p => p.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.PHONG_BAN_DA_PHAN_CONG_TOI_CHUYEN_VIEN 
                                      && (p.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.HO_SO_THAM_XET_LAI 
                                      || (p.DonViGui == (int)CommonENum.DON_VI_XU_LY.LANH_DAO_CUC && p.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG)));
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
            var truongPhongId = _session.UserId;
            query = query.Where(x => x.IsHoSoBS.Value);
            query = query.Where(p => p.TruongPhongId == truongPhongId);
            query = query.Where(x => x.TrangThaiHoSo != null && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_LUU
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_HOAN_TAT
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_TRA_LAI
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_HO_SO_MOI
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_BO_SUNG
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.CHO_TRA_GIAY_TIEP_NHAN
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_DA_TIEP_NHAN
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.PHONG_BAN_TU_CHOI_TIEP_NHAN_HO_SO
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.PHONG_BAN_DA_PHAN_CONG_TOI_CHUYEN_VIEN
                                  /*&& x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.HO_SO_THAM_XET_LAI*/);
            query = query.Where(x => x.LuongXuLy == (int)CommonENum.LUONG_XU_LY_TT37.LUONG_RA_SOAT);
            var t = query.ToList();
            if (input.FormCase == (int)CommonENum.FORM_CASE_TRUONG_PHONG_DUYET.HO_SO_CHUA_XU_LY)
            {
                
                query = query.Where(c => c.DonViGui == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET && c.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.SUA_DOI_BO_SUNG);
            }
            else if (input.FormCase == (int)CommonENum.FORM_CASE_TRUONG_PHONG_DUYET.HO_SO_XU_LY_LAI)
            {
                query = query.Where(x => x.DonViGui == (int)CommonENum.DON_VI_XU_LY.LANH_DAO_CUC && x.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.HO_SO_THAM_XET_LAI);
            }
            else if (input.FormCase == (int)CommonENum.FORM_CASE_TRUONG_PHONG_DUYET.HO_SO_DANG_THEO_DOI)
            {
                query = query.Where(x =>x.DonViXuLy != (int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG);
                var a = query.ToList();
            }
            return query;
        }
        private IQueryable<XHoSoDto> FormLanhDaoCucDuyet(IQueryable<XHoSoDto> query, XHoSoInputDto input)
        {
            var lanhDaoCucId = _session.UserId;
            query = query.Where(x => x.IsHoSoBS.Value);
            query = query.Where(p => p.LanhDaoCucId == lanhDaoCucId);

            query = query.Where(x => x.TrangThaiHoSo != null && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_LUU
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_HOAN_TAT
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_TRA_LAI
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_HO_SO_MOI
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_BO_SUNG
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.CHO_TRA_GIAY_TIEP_NHAN
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_DA_TIEP_NHAN
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.PHONG_BAN_TU_CHOI_TIEP_NHAN_HO_SO
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.PHONG_BAN_DA_PHAN_CONG_TOI_CHUYEN_VIEN
                                  /*&& x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.HO_SO_THAM_XET_LAI*/) ;
            query = query.Where(x => !(x.DonViGui == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET && x.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG));
            query = query.Where(x => x.LuongXuLy == (int)CommonENum.LUONG_XU_LY_TT37.LUONG_RA_SOAT);
            var a = query.ToList();
            if (input.FormCase == (int)CommonENum.FORM_CASE_LANH_DAO_CUC_DUYET.HO_SO_CHO_DUYET)
            {
                query = query.Where(x => x.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.LANH_DAO_CUC);
                query = query.Where(x => x.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.SUA_DOI_BO_SUNG);
                var b = query.ToList();
            }
            if (input.FormCase == (int)CommonENum.FORM_CASE_LANH_DAO_CUC_DUYET.HO_SO_TRA_LAI)
            {
                query = query.Where(p =>(p.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG && p.DonViGui == (int)CommonENum.DON_VI_XU_LY.LANH_DAO_CUC)
                || (p.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET && p.DonViGui == (int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG));
               
                query = query.Where(p => p.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.HO_SO_THAM_XET_LAI || p.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.SUA_DOI_BO_SUNG);
            }
            if (input.FormCase == (int)CommonENum.FORM_CASE_LANH_DAO_CUC_DUYET.HO_SO_DA_DUYET)
            {
                query = query.Where(p => p.LanhDaoCucDaDuyet == true);
            }
            var c = query.ToList();
            return query;
        }
        private IQueryable<XHoSoDto> FormVanThuDuyet(IQueryable<XHoSoDto> query, XHoSoInputDto input)
        {
            //query = query.Where(x => x.LuongXuLy == (int)CommonENum.LUONG_XU_LY_TT37.LUONG_RA_SOAT);
            var vanThuId = _session.UserId;
            query = query.Where(x => x.IsHoSoBS == true);
            query = query.Where(x => x.VanThuId == vanThuId && (x.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.VAN_THU || x.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.DOANH_NGHIEP));
            query = query.Where(x => x.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.SUA_DOI_BO_SUNG);
            if (input.FormCase == (int)CommonENum.FORM_CASE_VAN_THU_DUYET.HO_SO_CHUA_DUYET)
            {
                query = query.Where(p =>p.VanThuIsCA != true);
            }
            if (input.FormCase == (int)CommonENum.FORM_CASE_VAN_THU_DUYET.HO_SO_DA_DUYET)
            {
                query = query.Where( p => p.VanThuIsCA == true);
            }
            return query;
        }

        // Thẩm định hồ sơ
        private IQueryable<XHoSoDto> FormThamDinhHoSo(IQueryable<XHoSoDto> query, XHoSoInputDto input)
        {
            query = query.Where(x => x.TrangThaiHoSo != null && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_LUU
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_HOAN_TAT
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_TRA_LAI
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_HO_SO_MOI
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_BO_SUNG
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.CHO_TRA_GIAY_TIEP_NHAN
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_DA_TIEP_NHAN
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.LANH_DAO_DA_PHAN_CONG_HO_SO
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.PHONG_BAN_TU_CHOI_TIEP_NHAN_HO_SO
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.PHONG_BAN_DA_PHAN_CONG_TOI_CHUYEN_VIEN
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.HO_SO_THAM_XET_LAI
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.SUA_DOI_BO_SUNG);
            var nguoiThamDinhId = _session.UserId;
            var queryNguoiThamDinhHoSo = (from hoso in query.ToList()
                                     join hsDooanThamDinh in _hoSoDoanThamDinh.GetAll() on hoso.Id equals hsDooanThamDinh.HoSoId
                                     where hsDooanThamDinh.UserId == nguoiThamDinhId
                                     select hsDooanThamDinh).AsQueryable();
            var nguoiThamDinh = queryNguoiThamDinhHoSo.Select(x=>x.HoSoId).ToList();
            query = query.Where(p => nguoiThamDinh.Contains(p.Id));
            if (input.FormCase == (int)CommonENum.FORM_CASE_THAM_DINH_HO_SO_TT37.HO_SO_CHO_THAM_DINH)
            {
                query = query.Where(x => x.LuongXuLy == (int)CommonENum.LUONG_XU_LY_TT37.LUONG_THAM_DINH);
                var hoSoChoThamDinh = queryNguoiThamDinhHoSo.Where(x => !x.TrangThaiXuLy.HasValue).Select(x => x.HoSoId).ToList();
                //var nguoiThamDinhHoSo =
                //    (from hoso in query.ToList()
                //     join hsDooanThamDinh in _hoSoDoanThamDinh.GetAll() on hoso.Id equals hsDooanThamDinh.HoSoId
                //     where hsDooanThamDinh.UserId == nguoiThamDinhId && !hsDooanThamDinh.TrangThaiXuLy.HasValue
                //     select hsDooanThamDinh.HoSoId).ToList();
                query = query.Where(x => hoSoChoThamDinh.Contains(x.Id));
            }
            if (input.FormCase == (int)CommonENum.FORM_CASE_THAM_DINH_HO_SO_TT37.HO_SO_DANG_THEO_DOI)
            {
                query = query.Where(x => x.LuongXuLy == (int)CommonENum.LUONG_XU_LY_TT37.LUONG_THAM_DINH);
                var hoSoTheoDoi = queryNguoiThamDinhHoSo.Where(x => x.TrangThaiXuLy.HasValue).Select(x => x.HoSoId).ToList();
                //var nguoiThamDinhHoSo =
                //    (from hoso in query.ToList()
                //     join hsDooanThamDinh in _hoSoDoanThamDinh.GetAll() on hoso.Id equals hsDooanThamDinh.HoSoId
                //     where hsDooanThamDinh.UserId == nguoiThamDinhId && hsDooanThamDinh.TrangThaiXuLy.HasValue
                //     select hsDooanThamDinh.HoSoId).ToList();
                query = query.Where(x => hoSoTheoDoi.Contains(x.Id));

            }
            return query;
        }

        private IQueryable<XHoSoDto> FormTongHopThamDinhHoSo(IQueryable<XHoSoDto> query, XHoSoInputDto input)
        {
            var chuyenVienId = _session.UserId;
            query = query.Where(p => p.ChuyenVienThuLyId == chuyenVienId || (p.ChuyenVienPhoiHopId == chuyenVienId));
            query = query.Where(x => x.LuongXuLy == (int)CommonENum.LUONG_XU_LY_TT37.LUONG_THAM_DINH);
            query = query.Where(x => x.TrangThaiHoSo != null && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_LUU
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_HOAN_TAT
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_HO_SO_MOI
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_BO_SUNG
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_TRA_LAI
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.CHO_TRA_GIAY_TIEP_NHAN
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_DA_TIEP_NHAN
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.PHONG_BAN_TU_CHOI_TIEP_NHAN_HO_SO
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.PHONG_BAN_DA_PHAN_CONG_TOI_CHUYEN_VIEN);


            var a = query.ToList();
            if (input.FormCase == (int)CommonENum.FORM_CASE_TONG_HOP_THAM_DINH_TT37.HO_SO_CHO_TONG_HOP)
            {
                query = query.Where(x => x.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_THAM_DINH_HO_SO_37.HO_SO_CHO_TONG_HOP_THAM_DINH);
                query = query.Where(x => !x.TrangThaiXuLy.HasValue);
            }
            if (input.FormCase == (int)CommonENum.FORM_CASE_TONG_HOP_THAM_DINH_TT37.HO_SO_TONG_HOP_DA_LUU)
            {
                query = query.Where(x => x.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_THAM_DINH_HO_SO_37.HO_SO_TONG_HOP_THAM_DINH_DA_LUU);
            }
            if (input.FormCase == (int)CommonENum.FORM_CASE_TONG_HOP_THAM_DINH_TT37.HO_SO_TONG_HOP_LAI)
            {
                query = query.Where(x => x.DonViGui == (int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG && x.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.HO_SO_THAM_DINH_LAI);
            }
            if (input.FormCase == (int)CommonENum.FORM_CASE_TONG_HOP_THAM_DINH_TT37.HO_SO_DANG_THEO_DOI)
            {
                query = query.Where(x => x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_THAM_DINH_HO_SO_37.HO_SO_CHO_TONG_HOP_THAM_DINH);
                query = query.Where(x => x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_THAM_DINH_HO_SO_37.HO_SO_TONG_HOP_THAM_DINH_DA_LUU);
                query = query.Where(p => p.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.HO_SO_THAM_DINH_LAI
                                      || (p.DonViGui == (int)CommonENum.DON_VI_XU_LY.LANH_DAO_CUC && p.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG));
            }

            return query;
        }

        private IQueryable<XHoSoDto> FormTruongPhongDuyetThamDinh(IQueryable<XHoSoDto> query, XHoSoInputDto input)
        {
            var truongPhongId = _session.UserId;
            query = query.Where(p => p.TruongPhongId == truongPhongId);
            query = query.Where(x => x.IsHoSoBS.Value);
            query = query.Where(x => x.LuongXuLy == (int)CommonENum.LUONG_XU_LY_TT37.LUONG_THAM_DINH);
            query = query.Where(x => x.TrangThaiHoSo != null && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_LUU
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_HOAN_TAT
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_HO_SO_MOI
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_BO_SUNG
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_TRA_LAI
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.CHO_TRA_GIAY_TIEP_NHAN
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_DA_TIEP_NHAN
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.PHONG_BAN_TU_CHOI_TIEP_NHAN_HO_SO
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.PHONG_BAN_DA_PHAN_CONG_TOI_CHUYEN_VIEN
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_THAM_DINH_HO_SO_37.HO_SO_CHO_TONG_HOP_THAM_DINH
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_THAM_DINH_HO_SO_37.HO_SO_TONG_HOP_THAM_DINH_DA_LUU);

            if (input.FormCase == (int)CommonENum.FORM_CASE_TRUONG_PHONG_DUYET_THAM_DINH_TT37.HO_SO_CHO_DUYET)
            {
                query = query.Where(c => c.DonViGui == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET_TONG_HOP && c.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.SUA_DOI_BO_SUNG);
            }
            if (input.FormCase == (int)CommonENum.FORM_CASE_TRUONG_PHONG_DUYET_THAM_DINH_TT37.HO_SO_DUYET_LAI)
            {
                query = query.Where(x => x.DonViGui == (int)CommonENum.DON_VI_XU_LY.LANH_DAO_CUC && x.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.HO_SO_THAM_DINH_LAI);
            }
            if (input.FormCase == (int)CommonENum.FORM_CASE_TRUONG_PHONG_DUYET_THAM_DINH_TT37.HO_SO_DANG_THEO_DOI)
            {
                query = query.Where(x => x.DonViXuLy != (int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG);
                //query = query.Where(x => x.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.SUA_DOI_BO_SUNG || x.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.HO_SO_THAM_DINH_LAI);
            }

            return query;
        }
        private IQueryable<XHoSoDto> FormLanhDaoCucDuyetThamDinh(IQueryable<XHoSoDto> query, XHoSoInputDto input)
        {
            var lanhDaoCucId = _session.UserId;
            query = query.Where(p => p.LanhDaoCucId == lanhDaoCucId);
            query = query.Where(x => x.IsHoSoBS.Value);
            query = query.Where(x => x.LuongXuLy == (int)CommonENum.LUONG_XU_LY_TT37.LUONG_THAM_DINH);
            query = query.Where(x => x.TrangThaiHoSo != null && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_LUU
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_HOAN_TAT
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_HO_SO_MOI
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_BO_SUNG
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_TRA_LAI
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.CHO_TRA_GIAY_TIEP_NHAN
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_DA_TIEP_NHAN
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.PHONG_BAN_TU_CHOI_TIEP_NHAN_HO_SO
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.PHONG_BAN_DA_PHAN_CONG_TOI_CHUYEN_VIEN
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_THAM_DINH_HO_SO_37.HO_SO_CHO_TONG_HOP_THAM_DINH
                                  && x.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_THAM_DINH_HO_SO_37.HO_SO_TONG_HOP_THAM_DINH_DA_LUU);

            query = query.Where(x => !(x.DonViGui == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET_TONG_HOP && x.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG));

            if (input.FormCase == (int)CommonENum.FORM_CASE_LANH_DAO_DUYET_THAM_DINH_TT37.HO_SO_CHO_DUYET)
            {
                query = query.Where( p => p.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.LANH_DAO_CUC && p.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.SUA_DOI_BO_SUNG);
            }
            if (input.FormCase == (int)CommonENum.FORM_CASE_LANH_DAO_DUYET_THAM_DINH_TT37.HO_SO_TRA_LAI)
            {
                query = query.Where(p => (p.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG && p.DonViGui == (int)CommonENum.DON_VI_XU_LY.LANH_DAO_CUC)
                || (p.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET_TONG_HOP && p.DonViGui == (int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG));
                query = query.Where(p =>p.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.SUA_DOI_BO_SUNG || p.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.HO_SO_THAM_DINH_LAI);
            }
            if (input.FormCase == (int)CommonENum.FORM_CASE_LANH_DAO_DUYET_THAM_DINH_TT37.HO_SO_DA_DUYET)
            {
                query = query.Where( p => p.LanhDaoCucDaDuyet == true);
            }
            return query;
        }
        private IQueryable<XHoSoDto> FormVanThuDuyetThamDinh(IQueryable<XHoSoDto> query, XHoSoInputDto input)
        {
            var vanThuId = _session.UserId;
            query = query.Where(x => x.LuongXuLy == (int)CommonENum.LUONG_XU_LY_TT37.LUONG_THAM_DINH);
            query = query.Where(x => x.VanThuId == vanThuId);
            query = query.Where(x => x.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_THAM_DINH_HO_SO_37.HO_SO_CHO_VAN_THU_TRA_KET_QUA || x.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_HOAN_TAT);
            if (input.FormCase == (int)CommonENum.FORM_CASE_VAN_THU_DUYET_THAM_DINH_TT37.HO_SO_CHO_DUYET)
            {
                query = query.Where(x => x.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_THAM_DINH_HO_SO_37.HO_SO_CHO_VAN_THU_TRA_KET_QUA);
            }
            if (input.FormCase == (int)CommonENum.FORM_CASE_VAN_THU_DUYET_THAM_DINH_TT37.HO_SO_HOAN_TAT)
            {
                query = query.Where(x => x.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_HOAN_TAT);
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
                _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.XEM_HO_SO);

                if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_LUU)
                {
                    _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.NOP_HO_SO_DE_RA_SOAT);
                    _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.HUY_HO_SO);
                    _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.SUA_HO_SO);
                }
                if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_TRA_LAI)
                {
                    _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.SUA_HO_SO);
                    _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.NOP_HO_SO_BI_TRA_LAI);
                    _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.HUY_HO_SO);
                }

                if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.SUA_DOI_BO_SUNG && item.VanThuIsCA == true)
                {
                    _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.NOP_HO_SO_BO_SUNG);
                    _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.SUA_HO_SO);
                    _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.HUY_HO_SO);
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
                if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_TRA_LAI)
                {
                    _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.XEM_LY_DO_TRA_LAI);
                }
                if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_DA_TIEP_NHAN)
                {
                    _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.XEM_GIAY_TIEP_NHAN);
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
                    _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.XEM_LY_DO_TRA_LAI);
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
                else if (item.FormCase == (int)CommonENum.FORM_CASE_TRUONG_PHONG_DUYET.HO_SO_XU_LY_LAI)
                {
                    _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.TRUONG_PHONG_DUYET_LAI);
                    _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.XEM_LY_DO_TRA_LAI);
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

            else if (input.FormId == (int)CommonENum.FORM_ID.FORM_THAM_DINH_HO_SO_TT37)
            {
                _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.XEM_HO_SO);
                if (input.FormCase == (int)CommonENum.FORM_CASE_THAM_DINH_HO_SO_TT37.HO_SO_CHO_THAM_DINH)
                {
                    _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.CHUYEN_VIEN_TONG_HOP_THAM_DINH);
                }
            }

            else if (input.FormId == (int)CommonENum.FORM_ID.FORM_TONG_HOP_THAM_DINH_TT37)
            {
                _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.XEM_HO_SO);
                if (input.FormCase == (int)CommonENum.FORM_CASE_TONG_HOP_THAM_DINH_TT37.HO_SO_CHO_TONG_HOP)
                {
                    var ListDoanThamDinhHoSo = _hoSoDoanThamDinh.GetAll().Where(x => x.HoSoId == item.Id).ToList();
                    if (ListDoanThamDinhHoSo.All(x=>x.TrangThaiXuLy.HasValue))
                    {
                        _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.CHUYEN_VIEN_TONG_HOP_THAM_DINH);
                    }
                }
                if (input.FormCase == (int)CommonENum.FORM_CASE_TONG_HOP_THAM_DINH_TT37.HO_SO_TONG_HOP_LAI)
                {
                    _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.CHUYEN_VIEN_TONG_HOP_THAM_DINH_LAI);
                }

            }
            else if (input.FormId == (int)CommonENum.FORM_ID.FORM_TRUONG_PHONG_DUYET_THAM_DINH_TT37)
            {
                _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.XEM_HO_SO);
                if (input.FormCase == (int)CommonENum.FORM_CASE_TRUONG_PHONG_DUYET_THAM_DINH_TT37.HO_SO_CHO_DUYET)
                {
                    _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.TRUONG_PHONG_DUYET_THAM_DINH);
                }
                if (input.FormCase == (int)CommonENum.FORM_CASE_TRUONG_PHONG_DUYET_THAM_DINH_TT37.HO_SO_DUYET_LAI)
                {
                    _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.TRUONG_PHONG_DUYET_THAM_DINH_LAI);
                }

            }
            else if (input.FormId == (int)CommonENum.FORM_ID.FORM_LANH_DAO_CUC_DUYET_THAM_DINH_TT37)
            {
                _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.XEM_HO_SO);
                if (input.FormCase == (int)CommonENum.FORM_CASE_LANH_DAO_DUYET_THAM_DINH_TT37.HO_SO_CHO_DUYET)
                {
                    _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.LANH_DAO_CUC_DUYET_THAM_DINH);
                }
            }
            else if (input.FormId == (int)CommonENum.FORM_ID.FORM_VAN_THU_DUYET_THAM_DINH_TT37)
            {
                _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.XEM_HO_SO);
                if (input.FormCase == (int)CommonENum.FORM_CASE_VAN_THU_DUYET_THAM_DINH_TT37.HO_SO_CHO_DUYET)
                {
                    _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.LANH_DAO_CUC_DUYET_THAM_DINH);
                }
            }

            return _arrChucNang;
        }

        private string GetTrangThaiXuLyHoSo(XHoSoDto item)
        {
            var nguoiThamDinhHoSo =  _hoSoDoanThamDinh.GetAll().Where(x=>x.HoSoId == item.Id && x.ThuTucId == (int)CommonENum.THU_TUC_ID.THU_TUC_37).Select(x=>x.UserId).ToList();
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
            else if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_BO_SUNG)
            {
                trangThaiXuLy = "Hồ sơ nộp bổ sung";
            }
            else if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.CHO_KE_TOAN_XAC_NHAN)
            {
                trangThaiXuLy = "Hồ sơ đã gửi thanh toán, đang chờ xác nhận";
            }
            else if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.THANH_TOAN_THAT_BAI)
            {
                trangThaiXuLy = "Hồ sơ thanh toán thất bại";
            }
            else if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_HO_SO_MOI && item.GiayTiepNhanCA == null)
            {
                trangThaiXuLy = "Hồ sơ chờ tiếp nhận";
            }
            else if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_HO_SO_MOI && item.GiayTiepNhanCA != null)
            {
                trangThaiXuLy = "Hồ sơ chờ gửi phân công";
            }
            else if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_TRA_LAI)
            {
                trangThaiXuLy = "Hồ sơ bị trả lại";
            }
            else if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.CHO_TRA_GIAY_TIEP_NHAN)
            {
                trangThaiXuLy = "Chờ trả giấy tiếp nhận";
            }
            else if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.PHONG_BAN_DA_PHAN_CONG_TOI_CHUYEN_VIEN)
            {
                trangThaiXuLy = "Hồ sơ chờ rà soát";
            }
            else if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_DA_TIEP_NHAN || item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.LANH_DAO_DA_PHAN_CONG_HO_SO)
            {
                trangThaiXuLy = "Hồ sơ chờ phân công ";
            }
            else if(item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.PHONG_BAN_TU_CHOI_TIEP_NHAN_HO_SO || item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.CHUYEN_VIEN_TU_CHOI_TIEP_NHAN_HO_SO)
            {
                trangThaiXuLy = "Phân công lại hồ sơ ";
            }
            else if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.SUA_DOI_BO_SUNG)
            {
                trangThaiXuLy = "Hồ sơ cần bổ sung";
            }
            else if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_THAM_DINH_HO_SO_37.HO_SO_CHO_TONG_HOP_THAM_DINH && nguoiThamDinhHoSo.Contains(_session.UserId))
            {
                trangThaiXuLy = "Hồ sơ chờ thẩm định";
            }
            else if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_THAM_DINH_HO_SO_37.HO_SO_CHO_TONG_HOP_THAM_DINH && item.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET_TONG_HOP)
            {
                trangThaiXuLy = "Hồ sơ chờ tổng hợp thẩm định";
            }
            else if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_THAM_DINH_HO_SO_37.HO_SO_CHO_VAN_THU_TRA_KET_QUA)
            {
                trangThaiXuLy = "Hồ sơ chờ văn thư trả kết quả";
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

        private SoNgayQuaHanChiTiet37Dto TinhSoNgayQuaHanChiTiet(int _soNgayLamViec, DateTime? _ngayGui)
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
            return new SoNgayQuaHanChiTiet37Dto()
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