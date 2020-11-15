using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using newPSG.PMS.Authorization.Users;
using newPSG.PMS.Dto;
using newPSG.PMS.Editions;
using System;
using System.Linq;
using System.Threading.Tasks;

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
    #region INTERFACE
    public interface IXuLyHoSoLanhDaoCuc37AppService : IApplicationService
    {
        Task<dynamic> LoadLanhDaoCucDuyet(LanhDaoXuLy37Input input);
        Task<int> KyVaChuyenVanThu(LanhDaoXuLy37Input input);
        Task<int> ChuyenLaiTruongPhong(LanhDaoXuLy37Input input);

        Task<int> KyVaChuyenVanThuLuongThamDinh(LanhDaoXuLy37Input input);
        Task<int> ChuyenLaiTruongPhongThamDinhLai(LanhDaoXuLy37Input input);
    }
    #endregion

    #region MAIN
    [AbpAuthorize]
    public class XuLyHoSoLanhDaoCuc37AppService : PMSAppServiceBase, IXuLyHoSoLanhDaoCuc37AppService
    {
        private readonly IRepository<XHoSo, long> _hoSoRepos;
        private readonly IRepository<XHoSoXuLy, long> _hoSoXuLyRepos;
        private readonly IRepository<XHoSoXuLyHistory, long> _hoSoXuLyHistoryRepos;
        private readonly IRepository<User, long> _userRepos;
        private readonly IAbpSession _session;

        public XuLyHoSoLanhDaoCuc37AppService(IRepository<XHoSo, long> hoSoRepos,
                                              IRepository<XHoSoXuLy, long> hoSoXuLyRepos,
                                              IRepository<XHoSoXuLyHistory, long> hoSoXuLyHistoryRepos,
                                              IRepository<User, long> userRepos,
                                              IAbpSession session)
        {
            _hoSoRepos = hoSoRepos;
            _hoSoXuLyRepos = hoSoXuLyRepos;
            _hoSoXuLyHistoryRepos = hoSoXuLyHistoryRepos;
            _userRepos = userRepos;
            _session = session;
        }

        public async Task<dynamic> LoadLanhDaoCucDuyet(LanhDaoXuLy37Input input)
        {
            try
            {
                var hosoxl = await _hoSoXuLyRepos.GetAsync(input.HoSoXuLyId);
                if (hosoxl != null && hosoxl.Id > 0)
                {
                    var _yKienTruongPhong = (from yk in _hoSoXuLyHistoryRepos.GetAll()
                                             join r_us in _userRepos.GetAll() on yk.NguoiXuLyId equals r_us.Id into tb_us //Left Join
                                             from us in tb_us.DefaultIfEmpty()
                                             where yk.HoSoXuLyId == hosoxl.Id && (yk.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG || yk.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.PHO_PHONG)
                                             orderby yk.Id descending
                                             select new
                                             {
                                                 yk.DonViXuLy,
                                                 yk.NoiDungYKien,
                                                 TenTruongPhong = us.Surname + " " + us.Name
                                             }).FirstOrDefault();

                    if (hosoxl.HoSoXuLyHistoryId_Active.HasValue)
                    {
                        var xetDuyet = await _hoSoXuLyHistoryRepos.GetAsync(hosoxl.HoSoXuLyHistoryId_Active.Value);
                        return new
                        {
                            hoSoXuLy = hosoxl,
                            yKienTruongPhong = _yKienTruongPhong,
                            duyetHoSo = xetDuyet
                        };
                    }

                    return new
                    {
                        hoSoXuLy = hosoxl,
                        yKienTruongPhong = _yKienTruongPhong
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                return null;
            }
        }

        public async Task<int> KyVaChuyenVanThu(LanhDaoXuLy37Input input)
        {
            try
            {
                var hoso = await _hoSoRepos.GetAsync(input.HoSoId);
                var hoSoXuLy = await _hoSoXuLyRepos.GetAsync(input.HoSoXuLyId);
                if (hoSoXuLy != null)
                {
                    #region Lưu lịch sử
                    var _history = new XHoSoXuLyHistory();
                    _history.HoSoXuLyId = hoSoXuLy.Id;
                    _history.ThuTucId = hoSoXuLy.ThuTucId;
                    _history.HoSoId = hoSoXuLy.HoSoId;
                    _history.IsHoSoBS = hoSoXuLy.IsHoSoBS;
                    _history.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.LANH_DAO_CUC;
                    _history.NguoiXuLyId = _session.UserId;
                    _history.HoSoIsDat_Pre = hoSoXuLy.HoSoIsDat;
                    _history.HoSoIsDat = hoSoXuLy.HoSoIsDat;
                    _history.TrangThaiCV = input.TrangThaiCV;
                    _history.NoiDungCV = null;
                    _history.NgayXuLy = DateTime.Now;
                    _history.NoiDungYKien = input.NoiDungYKien;
                    _history.ActionEnum = (int)CommonENum.FORM_FUNCTION.LANH_DAO_CUC_DUYET;
                    _history.DonViKeTiep = input.DonViKeTiep;

                    await _hoSoXuLyHistoryRepos.InsertAndGetIdAsync(_history);
                    #endregion
                    
                    hoSoXuLy.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.VAN_THU;
                    hoSoXuLy.DonViGui = (int)CommonENum.DON_VI_XU_LY.LANH_DAO_CUC;
                    hoSoXuLy.NguoiGuiId = _session.UserId;
                    hoSoXuLy.NguoiXuLyId = null;
                    hoSoXuLy.LanhDaoCucDaDuyet = true;

                    if (!string.IsNullOrEmpty(input.DuongDanTepCA))
                    {
                        hoSoXuLy.LanhDaoCucIsCA = true;
                        hoSoXuLy.LanhDaoCucNgayKy = DateTime.Now;
                        hoSoXuLy.DuongDanTepCA = input.DuongDanTepCA;
                    }
                    
                    hoSoXuLy.LyDoTraLai = _history.NoiDungYKien;
                    await _hoSoXuLyRepos.UpdateAsync(hoSoXuLy);

                    //hoso.TrangThaiHoSo = (int)CommonENum.TRANG_THAI_HO_SO.LANH_DAO_DA_DUYET_THAM_XET;
                    //await _hoSoRepos.UpdateAsync(hoso);
                }
                return 1;
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                return 0;
            }
        }


        public async Task<int> ChuyenLaiTruongPhong(LanhDaoXuLy37Input input)
        {
            try
            {
                var hoso = await _hoSoRepos.GetAsync(input.HoSoId);
                var hoSoXuLy = await _hoSoXuLyRepos.FirstOrDefaultAsync(x => x.Id == input.HoSoXuLyId);
                if (hoSoXuLy != null)
                {
                    #region Lưu lịch sử
                    var _history = new XHoSoXuLyHistory();
                    _history.HoSoXuLyId = hoSoXuLy.Id;
                    _history.ThuTucId = hoSoXuLy.ThuTucId;
                    _history.HoSoId = hoSoXuLy.HoSoId;
                    _history.IsHoSoBS = hoSoXuLy.IsHoSoBS;
                    _history.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.LANH_DAO_CUC;
                    _history.DonViKeTiep = input.DonViKeTiep;
                    _history.NguoiXuLyId = _session.UserId;
                    _history.HoSoIsDat_Pre = hoSoXuLy.HoSoIsDat;
                    _history.HoSoIsDat = input.HoSoIsDat;
                    _history.NgayXuLy = DateTime.Now;
                    _history.TrangThaiCV = input.TrangThaiCV;
                    _history.NoiDungCV = null;
                    _history.NoiDungYKien = input.NoiDungYKien;
                    _history.ActionEnum = (int)CommonENum.FORM_FUNCTION.LANH_DAO_CUC_DUYET;

                    await _hoSoXuLyHistoryRepos.InsertAndGetIdAsync(_history);
                    #endregion

                    hoSoXuLy.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG;
                    hoSoXuLy.DonViGui = (int)CommonENum.DON_VI_XU_LY.LANH_DAO_CUC;
                    hoSoXuLy.NguoiGuiId = _session.UserId;
                    hoSoXuLy.LyDoTraLai = input.NoiDungYKien;
                    hoSoXuLy.NguoiXuLyId = hoSoXuLy.TruongPhongId;
                    hoSoXuLy.LanhDaoCucDaDuyet = null;

                    hoso.TrangThaiHoSo = (int)CommonENum.TRANG_THAI_HO_SO.HO_SO_THAM_XET_LAI;
                    await _hoSoRepos.UpdateAsync(hoso);
                    await _hoSoXuLyRepos.UpdateAsync(hoSoXuLy);
                }

                return 1;
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                return 0;
            }
        }

        public async Task<int> KyVaChuyenVanThuLuongThamDinh(LanhDaoXuLy37Input input)
        {
            try
            {
                var hoso = await _hoSoRepos.GetAsync(input.HoSoId);
                var hoSoXuLy = await _hoSoXuLyRepos.GetAsync(input.HoSoXuLyId);
                if (hoSoXuLy != null)
                {
                    #region Lưu lịch sử
                    var _history = new XHoSoXuLyHistory();
                    _history.HoSoXuLyId = hoSoXuLy.Id;
                    _history.ThuTucId = hoSoXuLy.ThuTucId;
                    _history.HoSoId = hoSoXuLy.HoSoId;
                    _history.IsHoSoBS = hoSoXuLy.IsHoSoBS;
                    _history.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.LANH_DAO_CUC;
                    _history.NguoiXuLyId = _session.UserId;
                    _history.HoSoIsDat_Pre = hoSoXuLy.HoSoIsDat;
                    _history.HoSoIsDat = hoSoXuLy.HoSoIsDat;
                    _history.TrangThaiCV = input.TrangThaiCV;
                    _history.NoiDungCV = null;
                    _history.NgayXuLy = DateTime.Now;
                    _history.NoiDungYKien = input.NoiDungYKien;
                    _history.ActionEnum = (int)CommonENum.FORM_FUNCTION.LANH_DAO_CUC_DUYET_THAM_DINH;
                    _history.DonViKeTiep = input.DonViKeTiep;

                    await _hoSoXuLyHistoryRepos.InsertAndGetIdAsync(_history);
                    #endregion

                    hoSoXuLy.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.VAN_THU;
                    hoSoXuLy.DonViGui = (int)CommonENum.DON_VI_XU_LY.LANH_DAO_CUC;
                    hoSoXuLy.NguoiGuiId = _session.UserId;
                    hoSoXuLy.NguoiXuLyId = hoSoXuLy.VanThuId;
                    hoSoXuLy.LanhDaoCucDaDuyet = true;

                    if (!string.IsNullOrEmpty(input.DuongDanTepCA))
                    {
                        hoSoXuLy.LanhDaoCucIsCA = true;
                        hoSoXuLy.LanhDaoCucNgayKy = DateTime.Now;
                        hoSoXuLy.DuongDanTepCA = input.DuongDanTepCA;
                    }

                    await _hoSoXuLyRepos.UpdateAsync(hoSoXuLy);

                }
                return 1;
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                return 0;
            }
        }

        public async Task<int> ChuyenLaiTruongPhongThamDinhLai(LanhDaoXuLy37Input input)
        {
            try
            {
                var hoso = await _hoSoRepos.GetAsync(input.HoSoId);
                var hoSoXuLy = await _hoSoXuLyRepos.FirstOrDefaultAsync(x => x.Id == input.HoSoXuLyId);
                if (hoSoXuLy != null)
                {
                    #region Lưu lịch sử
                    var _history = new XHoSoXuLyHistory();
                    _history.HoSoXuLyId = hoSoXuLy.Id;
                    _history.ThuTucId = hoSoXuLy.ThuTucId;
                    _history.HoSoId = hoSoXuLy.HoSoId;
                    _history.IsHoSoBS = hoSoXuLy.IsHoSoBS;
                    _history.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.LANH_DAO_CUC;
                    _history.DonViKeTiep = input.DonViKeTiep;
                    _history.NguoiXuLyId = _session.UserId;
                    _history.HoSoIsDat_Pre = hoSoXuLy.HoSoIsDat;
                    _history.HoSoIsDat = input.HoSoIsDat;
                    _history.TrangThaiCV = input.TrangThaiCV;
                    _history.NoiDungCV = null;
                    _history.NgayXuLy = DateTime.Now;
                    _history.NoiDungYKien = input.NoiDungYKien;
                    _history.ActionEnum = (int)CommonENum.FORM_FUNCTION.LANH_DAO_CUC_DUYET_THAM_DINH;

                    await _hoSoXuLyHistoryRepos.InsertAndGetIdAsync(_history);
                    #endregion

                    hoSoXuLy.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG;
                    hoSoXuLy.DonViGui = (int)CommonENum.DON_VI_XU_LY.LANH_DAO_CUC;
                    hoSoXuLy.NguoiGuiId = _session.UserId;
                    hoSoXuLy.LyDoTraLai = input.NoiDungYKien;
                    hoSoXuLy.NguoiXuLyId = hoSoXuLy.TruongPhongId;
                    hoSoXuLy.LanhDaoCucDaDuyet = null;

                    hoso.TrangThaiHoSo = (int)CommonENum.TRANG_THAI_HO_SO.HO_SO_THAM_DINH_LAI;
                    await _hoSoRepos.UpdateAsync(hoso);
                    await _hoSoXuLyRepos.UpdateAsync(hoSoXuLy);
                }

                return 1;
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                return 0;
            }
        }
    }
    #endregion
}