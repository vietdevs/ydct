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

using XHoSo = newPSG.PMS.EntityDB.HoSo99;
using XHoSoTepDinhKem = newPSG.PMS.EntityDB.HoSoTepDinhKem99;
using XHoSoXuLy = newPSG.PMS.EntityDB.HoSoXuLy99;
using XHoSoXuLyHistory = newPSG.PMS.EntityDB.HoSoXuLyHistory99;
using XHoSoDto = newPSG.PMS.Dto.HoSo99Dto;
using XHoSoInputDto = newPSG.PMS.Dto.HoSoInput99Dto;
using XHoSoTepDinhKemDto = newPSG.PMS.Dto.HoSoTepDinhKem99Dto;
using XHoSoXuLyDto = newPSG.PMS.Dto.HoSoXuLy99Dto;
using XHoSoXuLyHistoryDto = newPSG.PMS.Dto.HoSoXuLyHistory99Dto;
#endregion

namespace newPSG.PMS.Services
{
    #region INTERFACE
    public interface IXuLyHoSoLanhDaoCuc99AppService : IApplicationService
    {
        Task<dynamic> LoadLanhDaoCucDuyet(LanhDaoXuLy99Input input);
        Task<int> KyVaChuyenVanThu(LanhDaoXuLy99Input input);
        Task<int> ChuyenLaiTruongPhong(LanhDaoXuLy99Input input);
    }
    #endregion

    #region MAIN
    [AbpAuthorize]
    public class XuLyHoSoLanhDaoCuc99AppService : PMSAppServiceBase, IXuLyHoSoLanhDaoCuc99AppService
    {
        private readonly IRepository<XHoSo, long> _hoSoRepos;
        private readonly IRepository<XHoSoXuLy, long> _hoSoXuLyRepos;
        private readonly IRepository<XHoSoXuLyHistory, long> _hoSoXuLyHistoryRepos;
        private readonly IRepository<User, long> _userRepos;
        private readonly IAbpSession _session;

        public XuLyHoSoLanhDaoCuc99AppService(IRepository<XHoSo, long> hoSoRepos,
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

        public async Task<dynamic> LoadLanhDaoCucDuyet(LanhDaoXuLy99Input input)
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

        public async Task<int> KyVaChuyenVanThu(LanhDaoXuLy99Input input)
        {
            try
            {
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
                    if (!string.IsNullOrEmpty(input.GiayTiepNhanCA))
                    {
                        hoSoXuLy.LanhDaoCucIsCA = true;
                        hoSoXuLy.LanhDaoCucNgayKy = DateTime.Now;
                        hoSoXuLy.GiayTiepNhanCA = input.GiayTiepNhanCA;
                    }
                    hoSoXuLy.YKienGui = _history.NoiDungYKien;

                    if (hoSoXuLy.HoSoIsDat == true)
                    {
                        var hoSo = await _hoSoRepos.GetAsync(hoSoXuLy.HoSoId);
                        hoSo.SoGiayTiepNhan = input.SoTiepNhan;
                        await _hoSoRepos.UpdateAsync(hoSo);
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

        public async Task<int> ChuyenLaiTruongPhong(LanhDaoXuLy99Input input)
        {
            try
            {
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
                    _history.NoiDungYKien = input.NoiDungYKien;
                    _history.ActionEnum = (int)CommonENum.FORM_FUNCTION.LANH_DAO_CUC_DUYET;

                    await _hoSoXuLyHistoryRepos.InsertAndGetIdAsync(_history);
                    #endregion

                    hoSoXuLy.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET;
                    hoSoXuLy.DonViGui = (int)CommonENum.DON_VI_XU_LY.LANH_DAO_CUC;
                    hoSoXuLy.NguoiGuiId = _session.UserId;
                    hoSoXuLy.YKienGui = _history.NoiDungYKien;
                    hoSoXuLy.NguoiXuLyId = hoSoXuLy.ChuyenVienThuLyId;
                    hoSoXuLy.LanhDaoCucDaDuyet = true;

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