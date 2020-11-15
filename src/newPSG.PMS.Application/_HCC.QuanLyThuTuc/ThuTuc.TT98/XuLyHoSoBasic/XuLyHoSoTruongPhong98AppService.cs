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
    #region INTERFACE
    public interface IXuLyHoSoTruongPhong98AppService : IApplicationService
    {
        Task<dynamic> InitTruongPhongDuyet(InitTruongPhongDuyet98InputDto input);
        Task<dynamic> LoadTruongPhongDuyet(LoadTruongPhongDuyet98InputDto input);
        Task<int> TruongPhongDuyet(LuuTruongPhongDuyet98InputDto input);
    }
    #endregion

    #region MAIN
    [AbpAuthorize]
    public class XuLyHoSoTruongPhong98AppService : PMSAppServiceBase, IXuLyHoSoTruongPhong98AppService
    {
        private readonly IRepository<XHoSoXuLy, long> _hoSoXuLyRepos;
        private readonly IRepository<XHoSoXuLyHistory, long> _hoSoXuLyHistoryRepos;
        private readonly IRepository<User, long> _userRepos;
        private readonly IAbpSession _session;

        public XuLyHoSoTruongPhong98AppService(IRepository<XHoSoXuLy, long> hoSoXuLyRepos,
                                               IRepository<XHoSoXuLyHistory, long> hoSoXuLyHistoryRepos,
                                               IRepository<User, long> userRepos,
                                               IAbpSession session)
        {
            _hoSoXuLyRepos = hoSoXuLyRepos;
            _hoSoXuLyHistoryRepos = hoSoXuLyHistoryRepos;
            _userRepos = userRepos;
            _session = session;
        }

        public async Task<dynamic> InitTruongPhongDuyet(InitTruongPhongDuyet98InputDto input)
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

        public async Task<dynamic> LoadTruongPhongDuyet(LoadTruongPhongDuyet98InputDto input)
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
                    //    _hoSoXuLyDto.BienBanThamDinh_ChuyenVienPhoiHop = new BienBanThamDinhPhoiHop98Dto()
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
        public async Task<int> TruongPhongDuyet(LuuTruongPhongDuyet98InputDto input)
        {
            try
            {
                if (input.HoSoXuLyId > 0)
                {
                    var hosoxl = await _hoSoXuLyRepos.FirstOrDefaultAsync(input.HoSoXuLyId);
                    if (hosoxl != null)
                    {
                        #region Lưu lịch sử
                        var _history = new XHoSoXuLyHistory();

                        if (hosoxl.HoSoXuLyHistoryId_Active.HasValue)
                        {
                            _history = _hoSoXuLyHistoryRepos.Get(hosoxl.HoSoXuLyHistoryId_Active.Value);
                        }

                        _history.ThuTucId = hosoxl.ThuTucId;
                        _history.HoSoXuLyId = hosoxl.Id;
                        _history.HoSoId = input.HoSoId;
                        _history.IsHoSoBS = hosoxl.IsHoSoBS;
                        _history.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG;
                        _history.ActionEnum = (int)CommonENum.FORM_FUNCTION.TRUONG_PHONG_DUYET;
                        _history.NguoiXuLyId = _session.UserId;
                        _history.HoSoIsDat_Pre = true;
                        _history.HoSoIsDat = true;
                        
                        var _historyId = await _hoSoXuLyHistoryRepos.InsertOrUpdateAndGetIdAsync(_history);
                        hosoxl.HoSoXuLyHistoryId_Active = _historyId;
                        #endregion

                        hosoxl.HoSoXuLyHistoryId_Active = null;
                        hosoxl.TruongPhongDaDuyet = true;

                        //Lưu hồ sơ

                        await _hoSoXuLyRepos.UpdateAsync(hosoxl);
                        return 1;
                    }
                }

                return 0;
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