using Abp.Application.Services;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using newPSG.PMS.Authorization.Roles;
using newPSG.PMS.Authorization.Users;
using newPSG.PMS.Common.Dto;
using newPSG.PMS.Dto;
using newPSG.PMS.Editions;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Abp.AutoMapper;

#region Class Riêng Cho Từng Thủ tục

using XHoSo = newPSG.PMS.EntityDB.HoSo99;
using XHoSoTepDinhKem = newPSG.PMS.EntityDB.HoSoTepDinhKem99;
using XHoSoXuLy = newPSG.PMS.EntityDB.HoSoXuLy99;
using XHoSoXuLyHistory = newPSG.PMS.EntityDB.HoSoXuLyHistory99;
using XBienBanThamDinh = newPSG.PMS.EntityDB.BienBanThamDinh99;
using XHoSoDto = newPSG.PMS.Dto.HoSo99Dto;
using XHoSoInputDto = newPSG.PMS.Dto.HoSoInput99Dto;
using XHoSoTepDinhKemDto = newPSG.PMS.Dto.HoSoTepDinhKem99Dto;
using XHoSoXuLyDto = newPSG.PMS.Dto.HoSoXuLy99Dto;
using XHoSoXuLyHistoryDto = newPSG.PMS.Dto.HoSoXuLyHistory99Dto;
using XBienBanThamDinhDto = newPSG.PMS.Dto.BienBanThamDinh99Dto;
#endregion

namespace newPSG.PMS.Services
{
    #region INTERFACE
    public interface IXuLyHoSoTruongPhong99AppService : IApplicationService
    {
        Task<dynamic> InitTruongPhongDuyet(InitTruongPhongDuyet99InputDto input);
        Task<dynamic> LoadTruongPhongDuyet(LoadTruongPhongDuyet99InputDto input);
        Task<int> TruongPhongDuyet_Luu(LuuTruongPhongDuyet99InputDto input);
        Task<int> TruongPhongDuyet_Chuyen(LuuTruongPhongDuyet99InputDto input);
    }
    #endregion

    #region MAIN
    [AbpAuthorize]
    public class XuLyHoSoTruongPhong99AppService : PMSAppServiceBase, IXuLyHoSoTruongPhong99AppService
    {
        private readonly IRepository<XHoSoXuLy, long> _hoSoXuLyRepos;
        private readonly IRepository<XHoSoXuLyHistory, long> _hoSoXuLyHistoryRepos;
        private readonly IRepository<User, long> _userRepos;
        private readonly IAbpSession _session;

        public XuLyHoSoTruongPhong99AppService(IRepository<XHoSoXuLy, long> hoSoXuLyRepos,
                                               IRepository<XHoSoXuLyHistory, long> hoSoXuLyHistoryRepos,
                                               IRepository<User, long> userRepos,
                                               IAbpSession session)
        {
            _hoSoXuLyRepos = hoSoXuLyRepos;
            _hoSoXuLyHistoryRepos = hoSoXuLyHistoryRepos;
            _userRepos = userRepos;
            _session = session;
        }

        public async Task<dynamic> InitTruongPhongDuyet(InitTruongPhongDuyet99InputDto input)
        {
            try
            {
                var _queryLanhDaoCuc = from u in _userRepos.GetAll()
                                       where u.RoleLevel == (int)CommonENum.ROLE_LEVEL.LANH_DAO_CUC
                                       orderby u.Stt descending
                                       select new ItemDto<long>
                                       {
                                           Id = u.Id,
                                           Name = u.Surname + " " + u.Name
                                       };

                var res = new
                {
                    listLanhDaoCuc = await _queryLanhDaoCuc.ToListAsync()
                };

                return res;
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                return null;
            }
        }

        public async Task<dynamic> LoadTruongPhongDuyet(LoadTruongPhongDuyet99InputDto input)
        {
            try
            {
                var hosoxl = await _hoSoXuLyRepos.FirstOrDefaultAsync(input.HoSoXuLyId);
                if (hosoxl != null && hosoxl.Id > 0)
                {
                    var tenChuyenVienThuLy = "";
                    var chuyenVienPhoiHop = "";
                    var tenNguoiGui = "";
                    if (hosoxl.ChuyenVienThuLyId.HasValue)
                    {
                        var cv1 = await _userRepos.FirstOrDefaultAsync(hosoxl.ChuyenVienThuLyId.Value);
                        if (cv1 != null)
                        {
                            tenChuyenVienThuLy = cv1.Surname + " " + cv1.Name;
                        }
                    }
                    if (hosoxl.ChuyenVienPhoiHopId.HasValue)
                    {
                        var cv2 = await _userRepos.FirstOrDefaultAsync(hosoxl.ChuyenVienPhoiHopId.Value);
                        if (cv2 != null)
                        {
                            tenChuyenVienThuLy = cv2.Surname + " " + cv2.Name;
                        }
                    }

                    if (hosoxl.NguoiGuiId.HasValue)
                    {
                        var u = await _userRepos.FirstOrDefaultAsync(hosoxl.NguoiGuiId.Value);
                        if (u != null)
                        {
                            tenNguoiGui = u.Surname + " " + u.Name;
                        }
                    }

                    var nguoiDuyet = new
                    {
                        chuyenVienThuLyId = hosoxl.ChuyenVienThuLyId,
                        tenChuyenVienThuLy,
                        chuyenVienPhoiHopId = hosoxl.ChuyenVienPhoiHopId,
                        chuyenVienPhoiHop,
                        nguoiGuiId = hosoxl.NguoiGuiId,
                        tenNguoiGui
                    };

                    var _hoSoXuLyDto = hosoxl.MapTo<XHoSoXuLyDto>();
                    //if (_hoSoXuLyDto.BienBanThamDinhId_CVThuLy.HasValue && _hoSoXuLyDto.BienBanThamDinhId_CVThuLy > 0)
                    //{
                    //    XBienBanThamDinh bienBanThamDinh = null;
                    //    bienBanThamDinh = await _BienBanThamDinhRepos.GetAsync(_hoSoXuLyDto.BienBanThamDinhId_CVThuLy.Value);
                    //    _hoSoXuLyDto.BienBanThamDinh_ChuyenVienPhoiHop = new BienBanThamDinhPhoiHop99Dto()
                    //    {
                    //        IsCopyThamXet = bienBanThamDinh.IsCopyThamXet
                    //    };
                    //}
                    if (_hoSoXuLyDto.HoSoXuLyHistoryId_Active.HasValue)
                    {
                        XHoSoXuLyHistory _history = null;
                        if (_hoSoXuLyDto.HoSoXuLyHistoryId_Active.HasValue)
                        {
                            _history = await _hoSoXuLyHistoryRepos.GetAsync(_hoSoXuLyDto.HoSoXuLyHistoryId_Active.Value);
                        }

                        return new
                        {
                            hoSoXuLy = _hoSoXuLyDto,
                            nguoiDuyet,
                            duyetHoSo = _history
                        };
                    }

                    return new
                    {
                        hoSoXuLy = _hoSoXuLyDto,
                        nguoiDuyet
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

        public async Task<int> TruongPhongDuyet_Luu(LuuTruongPhongDuyet99InputDto input)
        {
            try
            {
                if (input.HoSoXuLyId > 0)
                {
                    var hosoxl = await _hoSoXuLyRepos.FirstOrDefaultAsync(input.HoSoXuLyId);
                    if (hosoxl != null && _session.UserId == hosoxl.TruongPhongId)
                    {
                        #region Lưu lịch sử
                        var _history = new XHoSoXuLyHistory();

                        if (hosoxl.HoSoXuLyHistoryId_Active.HasValue)
                        {
                            _history = _hoSoXuLyHistoryRepos.Get(hosoxl.HoSoXuLyHistoryId_Active.Value);
                        }

                        _history.ThuTucId = hosoxl.ThuTucId;
                        _history.HoSoXuLyId = hosoxl.Id;
                        _history.HoSoId = hosoxl.HoSoId;
                        _history.ThuTucId = hosoxl.ThuTucId;
                        _history.IsHoSoBS = hosoxl.IsHoSoBS;
                        _history.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG;
                        _history.DonViKeTiep = input.DonViKeTiep;
                        _history.NoiDungYKien = input.NoiDungYKien;
                        _history.NguoiXuLyId = _session.UserId;
                        _history.HoSoIsDat = input.HoSoIsDat;
                        _history.HoSoIsDat_Pre = hosoxl.HoSoIsDat;
                        _history.TrangThaiCV = input.TrangThaiCV;
                        _history.IsSuaCV = input.IsSuaCV;
                        if (input.HoSoIsDat != true && input.IsSuaCV.HasValue && input.IsSuaCV.Value)
                        {
                            _history.NoiDungCV = input.NoiDungCV;
                        }
                        else
                        {
                            _history.NoiDungCV = null;
                        }

                        var _historyId = await _hoSoXuLyHistoryRepos.InsertOrUpdateAndGetIdAsync(_history);
                        hosoxl.HoSoXuLyHistoryId_Active = _historyId;
                        #endregion

                        if (input.HoSoIsDat != true && input.IsSuaCV.HasValue && input.IsSuaCV.Value)
                        {
                            hosoxl.NoiDungCV = input.NoiDungCV;
                        }
                        //hosoxl.DonViKeTiep = input.DonViKeTiep;
                        //hosoxl.TrangThaiCV = input.TrangThaiCV;

                        if (hosoxl.DonViKeTiep == (int)CommonENum.DON_VI_XU_LY.LANH_DAO_CUC)
                        {
                            hosoxl.LanhDaoCucId = input.LanhDaoCucId;
                        }

                        hosoxl.YKienChung = input.YKienChung;

                        await _hoSoXuLyRepos.UpdateAsync(hosoxl);
                    }
                }

                return 1;
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                return 0;
            }
        }

        public async Task<int> TruongPhongDuyet_Chuyen(LuuTruongPhongDuyet99InputDto input)
        {
            try
            {
                if (input.HoSoXuLyId > 0)
                {
                    var hosoxl = await _hoSoXuLyRepos.FirstOrDefaultAsync(input.HoSoXuLyId);
                    if (hosoxl != null && _session.UserId == hosoxl.TruongPhongId)
                    {
                        #region Lưu lịch sử
                        var _history = new XHoSoXuLyHistory();

                        if (hosoxl.HoSoXuLyHistoryId_Active.HasValue)
                        {
                            _history = _hoSoXuLyHistoryRepos.Get(hosoxl.HoSoXuLyHistoryId_Active.Value);
                        }

                        _history.ThuTucId = hosoxl.ThuTucId;
                        _history.HoSoXuLyId = hosoxl.Id;
                        _history.HoSoId = hosoxl.HoSoId;
                        _history.ThuTucId = hosoxl.ThuTucId;
                        _history.IsHoSoBS = hosoxl.IsHoSoBS;
                        _history.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG;
                        _history.ActionEnum = (int)CommonENum.FORM_FUNCTION.TRUONG_PHONG_DUYET;
                        _history.DonViKeTiep = input.DonViKeTiep;
                        _history.NoiDungYKien = input.NoiDungYKien;
                        _history.NguoiXuLyId = _session.UserId;
                        _history.HoSoIsDat_Pre = hosoxl.HoSoIsDat;
                        _history.HoSoIsDat = input.HoSoIsDat;
                        _history.TrangThaiCV = input.TrangThaiCV;
                        _history.IsSuaCV = input.IsSuaCV;
                        if (input.HoSoIsDat != true && input.IsSuaCV.HasValue && input.IsSuaCV.Value)
                        {
                            _history.NoiDungCV = input.NoiDungCV;
                        }
                        else
                        {
                            _history.NoiDungCV = null;
                        }

                        var _historyId = await _hoSoXuLyHistoryRepos.InsertOrUpdateAndGetIdAsync(_history);
                        hosoxl.HoSoXuLyHistoryId_Active = _historyId;
                        #endregion

                        if (input.HoSoIsDat != true && input.IsSuaCV.HasValue && input.IsSuaCV.Value)
                        {
                            hosoxl.NoiDungCV = input.NoiDungCV;
                        }
                        hosoxl.DonViKeTiep = input.DonViKeTiep;
                        hosoxl.HoSoIsDat = input.HoSoIsDat;
                        hosoxl.TrangThaiCV = input.TrangThaiCV;

                        if (hosoxl.DonViKeTiep == (int)CommonENum.DON_VI_XU_LY.LANH_DAO_CUC)
                        {
                            hosoxl.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.LANH_DAO_CUC;
                            hosoxl.DonViGui = (int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG;
                            hosoxl.LanhDaoCucId = input.LanhDaoCucId;
                            hosoxl.NguoiXuLyId = input.LanhDaoCucId;
                        }
                        else if (hosoxl.DonViKeTiep == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET_TONG_HOP)
                        {
                            hosoxl.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET_TONG_HOP;
                            hosoxl.NguoiXuLyId = hosoxl.ChuyenVienThuLyId;
                            hosoxl.DonViGui = (int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG;
                        }

                        hosoxl.HoSoXuLyHistoryId_Active = null;
                        hosoxl.NguoiGuiId = _session.UserId;
                        hosoxl.YKienGui = _history.NoiDungYKien;
                        hosoxl.TruongPhongDaDuyet = true;

                        hosoxl.YKienChung = input.YKienChung;

                        //Lưu hồ sơ
                        hosoxl.NgayGui = DateTime.Now;

                        await _hoSoXuLyRepos.UpdateAsync(hosoxl);
                    }
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