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
using Newtonsoft.Json;

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
    public interface IXuLyHoSoChuyenVien99AppService : IApplicationService
    {
        Task<dynamic> InitThamDinh(InitThamnDinh99InputDto input);
        Task<GetBienBanThamDinh99Dto> GetBienBanThamDinh(long HoSoXuLyId);

        #region Thẩm định hồ sơ
        Task<dynamic> LoadThamDinh(LoadThamDinh99InputDto input);
        Task<int> ThamDinh_Luu(LuuThamDinh99InputDto input);
        Task<int> ThamDinh_Chuyen(LuuThamDinh99InputDto input);
        #endregion

        #region Tổng hợp thẩm định
        Task<dynamic> LoadTongHopThamDinh(LoadThamDinh99InputDto input);
        Task<int> TongHopThamDinh_Luu(LuuThamDinh99InputDto input);
        Task<int> TongHopThamDinh_Chuyen(LuuThamDinh99InputDto input);
        #endregion

        #region Thẩm định lại

        Task<dynamic> LoadThamDinhLai(LoadThamDinh99InputDto input);
        Task<int> ThamDinhLai_Luu(LuuThamDinh99InputDto input);
        Task<int> ThamDinhLai_Chuyen(LuuThamDinh99InputDto input);

        #endregion

        #region Thẩm định bổ sung
        Task<dynamic> LoadThamDinhBoSung(LoadThamDinh99InputDto input);
        Task<int> ThamDinhBoSung_Luu(LuuThamDinh99InputDto input);
        Task<int> ThamDinhBoSung_Chuyen(LuuThamDinh99InputDto input);
        #endregion
    }
    #endregion

    #region MAIN
    [AbpAuthorize]
    public class XuLyHoSoChuyenVien99AppService : PMSAppServiceBase, IXuLyHoSoChuyenVien99AppService
    {
        private readonly IRepository<XHoSoXuLy, long> _hoSoXuLyRepos;
        private readonly IRepository<XHoSoXuLyHistory, long> _hoSoXuLyHistoryRepos;
        private readonly IRepository<XBienBanThamDinh, long> _BienBanThamDinhRepos;
        private readonly IRepository<User, long> _userRepos;
        private readonly IAbpSession _session;
        public XuLyHoSoChuyenVien99AppService(IRepository<XHoSoXuLy, long> hoSoXuLyRepos,
                                              IRepository<XHoSoXuLyHistory, long> hoSoXuLyHistoryRepos,
                                              IRepository<XBienBanThamDinh, long> BienBanThamDinhRepos,
                                              IRepository<User, long> userRepos,
                                              IAbpSession session)
        {
            _hoSoXuLyRepos = hoSoXuLyRepos;
            _hoSoXuLyHistoryRepos = hoSoXuLyHistoryRepos;
            _BienBanThamDinhRepos = BienBanThamDinhRepos;
            _userRepos = userRepos;
            _session = session;
        }

        public async Task<dynamic> InitThamDinh(InitThamnDinh99InputDto input)
        {
            try
            {
                var _queryTruongPhong = from u in _userRepos.GetAll()
                                        where u.PhongBanId == input.PhongBanId && u.RoleLevel == (int)CommonENum.ROLE_LEVEL.TRUONG_PHONG && u.IsActive == true
                                        select new ItemDto<long>
                                        {
                                            Id = u.Id,
                                            Name = u.Surname + " " + u.Name
                                        };

                var _queryPhoPhong = (from u in _userRepos.GetAll()
                                      where u.PhongBanId == input.PhongBanId
                                      && (u.RoleLevel == (int)CommonENum.ROLE_LEVEL.PHO_PHONG)
                                      && u.IsActive == true
                                      select new ItemDto<long>
                                      {
                                          Id = u.Id,
                                          Name = u.Surname + " " + u.Name
                                      });
                var res = new
                {
                    listTruongPhong = await _queryTruongPhong.ToListAsync(),
                    listPhoPhong = await _queryPhoPhong.ToListAsync(),
                };

                return res;
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                return null;
            }
        }

        public async Task<GetBienBanThamDinh99Dto> GetBienBanThamDinh(long HoSoXuLyId)
        {
            try
            {
                if (HoSoXuLyId == 0)
                {
                    return null;
                }
                var hosoxl = await _hoSoXuLyRepos.FirstOrDefaultAsync(HoSoXuLyId);
                if (hosoxl == null || hosoxl.Id == 0)
                {
                    return null;
                }

                var tenChuyenVienThuLy = string.Empty;
                var tenChuyenVienPhoiHop = string.Empty;
                long? chuyenVienPhoiHopId = 0;
                var tenNguoiGui = string.Empty;
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
                    chuyenVienPhoiHopId = hosoxl.ChuyenVienPhoiHopId;
                    var cv2 = await _userRepos.FirstOrDefaultAsync(hosoxl.ChuyenVienPhoiHopId.Value);
                    if (cv2 != null)
                    {
                        tenChuyenVienPhoiHop = cv2.Surname + " " + cv2.Name;
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

                var nguoiDuyet = new NguoiDuyet99Dto
                {
                    ChuyenVienThuLyId = hosoxl.ChuyenVienThuLyId,
                    TenChuyenVienThuLy = tenChuyenVienThuLy,
                    ChuyenVienPhoiHopId = chuyenVienPhoiHopId,
                    TenChuyenVienPhoiHop = tenChuyenVienPhoiHop,
                    NguoiGuiId = hosoxl.NguoiGuiId,
                    TenNguoiGui = tenNguoiGui
                };

                //Nội dung biên bản thẩm định
                var BienBanThamDinhDto = new XBienBanThamDinhDto();
                var BienBanThamDinhCV1Dto = new XBienBanThamDinhDto();
                var BienBanThamDinhCV2Dto = new XBienBanThamDinhDto();
                if (hosoxl.BienBanThamDinhId_ChuyenVienThuLy.HasValue)
                {
                    var bienBan = await _BienBanThamDinhRepos.GetAsync(hosoxl.BienBanThamDinhId_ChuyenVienThuLy.Value);
                    bienBan.MapTo(BienBanThamDinhCV1Dto);
                }
                if (hosoxl.BienBanThamDinhId_ChuyenVienPhoiHop.HasValue)
                {
                    var bienBan = await _BienBanThamDinhRepos.GetAsync(hosoxl.BienBanThamDinhId_ChuyenVienPhoiHop.Value);
                    bienBan.MapTo(BienBanThamDinhCV2Dto);
                }
                BienBanThamDinhDto.NoiDungThamDinhJson = JsonConvert.SerializeObject(BienBanThamDinhDto.ArrNoiDungThamDinh);

                var ret = new GetBienBanThamDinh99Dto
                {
                    HoSoXuLy = hosoxl.MapTo<XHoSoXuLyDto>(),
                    BienBanThamDinh = BienBanThamDinhDto,
                    BienBanThamDinhCV1 = BienBanThamDinhCV1Dto,
                    BienBanThamDinhCV2 = BienBanThamDinhCV2Dto,
                    NguoiDuyet = nguoiDuyet
                };

                return ret;
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                return null;
            }
        }

        #region Thẩm định hồ sơ
        public async Task<dynamic> LoadThamDinh(LoadThamDinh99InputDto input)
        {
            try
            {
                if (input.HoSoXuLyId > 0)
                {
                    var objGetThamDinh = await GetBienBanThamDinh(input.HoSoXuLyId);
                    if (objGetThamDinh != null)
                    {
                        if (objGetThamDinh.HoSoXuLy.HoSoXuLyHistoryId_Active.HasValue)
                        {
                            var _history = await _hoSoXuLyHistoryRepos.GetAsync(objGetThamDinh.HoSoXuLy.HoSoXuLyHistoryId_Active.Value);

                            objGetThamDinh.DuyetHoSo = _history.MapTo<XHoSoXuLyHistoryDto>();
                        }

                        return objGetThamDinh;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                return null;
            }
        }
        public async Task<int> ThamDinh_Luu(LuuThamDinh99InputDto input)
        {
            try
            {
                if (input.HoSoXuLyId > 0) //input.Id: HoSoXuLyId_Active
                {
                    var hosoxl = await _hoSoXuLyRepos.FirstOrDefaultAsync(input.HoSoXuLyId);
                    if (hosoxl != null)
                    {
                        #region Lưu thẩm định
                        if (_session.UserId == hosoxl.ChuyenVienThuLyId)
                        {
                            if (hosoxl.BienBanThamDinhId_ChuyenVienThuLy.HasValue)
                            {
                                var updateData = await _BienBanThamDinhRepos.GetAsync(hosoxl.BienBanThamDinhId_ChuyenVienThuLy.Value);
                                input.BienBanThamDinh.MapTo(updateData);
                                updateData.ThuTucId = hosoxl.ThuTucId;
                                updateData.HoSoId = hosoxl.HoSoId;
                                updateData.HoSoXuLyId = hosoxl.Id;
                                await _BienBanThamDinhRepos.UpdateAsync(updateData);
                            }
                            else
                            {
                                var insertInput = input.BienBanThamDinh.MapTo<XBienBanThamDinh>();
                                insertInput.ThuTucId = hosoxl.ThuTucId;
                                insertInput.HoSoId = hosoxl.HoSoId;
                                insertInput.HoSoXuLyId = hosoxl.Id;
                                long _BienBanThamDinhId = await _BienBanThamDinhRepos.InsertAndGetIdAsync(insertInput);
                                hosoxl.BienBanThamDinhId_ChuyenVienThuLy = _BienBanThamDinhId;
                            }
                        }
                        else if (_session.UserId == hosoxl.ChuyenVienPhoiHopId)
                        {
                            if (hosoxl.BienBanThamDinhId_ChuyenVienPhoiHop.HasValue)
                            {
                                var updateData = await _BienBanThamDinhRepos.GetAsync(hosoxl.BienBanThamDinhId_ChuyenVienPhoiHop.Value);
                                input.BienBanThamDinh.MapTo(updateData);
                                updateData.ThuTucId = hosoxl.ThuTucId;
                                updateData.HoSoId = hosoxl.HoSoId;
                                updateData.HoSoXuLyId = hosoxl.Id;
                                await _BienBanThamDinhRepos.UpdateAsync(updateData);
                            }
                            else
                            {
                                var insertInput = input.BienBanThamDinh.MapTo<XBienBanThamDinh>();
                                insertInput.ThuTucId = hosoxl.ThuTucId;
                                insertInput.HoSoId = hosoxl.HoSoId;
                                insertInput.HoSoXuLyId = hosoxl.Id;
                                long _BienBanThamDinhId = await _BienBanThamDinhRepos.InsertAndGetIdAsync(insertInput);
                                hosoxl.BienBanThamDinhId_ChuyenVienPhoiHop = _BienBanThamDinhId;
                            }
                        }
                        #endregion

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
        public async Task<int> ThamDinh_Chuyen(LuuThamDinh99InputDto input)
        {
            try
            {
                if (input.HoSoXuLyId > 0) //input.Id: HoSoXuLyId_Active
                {
                    var hosoxl = await _hoSoXuLyRepos.FirstOrDefaultAsync(input.HoSoXuLyId);
                    if (hosoxl != null)
                    {
                        #region Lưu thẩm định
                        if (_session.UserId == hosoxl.ChuyenVienThuLyId)
                        {
                            if (hosoxl.BienBanThamDinhId_ChuyenVienThuLy.HasValue)
                            {
                                var updateData = await _BienBanThamDinhRepos.GetAsync(hosoxl.BienBanThamDinhId_ChuyenVienThuLy.Value);
                                updateData.ThuTucId = hosoxl.ThuTucId;
                                updateData.HoSoId = hosoxl.HoSoId;
                                updateData.HoSoXuLyId = hosoxl.Id;
                                input.BienBanThamDinh.MapTo(updateData);
                                await _BienBanThamDinhRepos.UpdateAsync(updateData);
                            }
                            else
                            {
                                var insertInput = input.BienBanThamDinh.MapTo<XBienBanThamDinh>();
                                insertInput.ThuTucId = hosoxl.ThuTucId;
                                insertInput.HoSoId = hosoxl.HoSoId;
                                insertInput.HoSoXuLyId = hosoxl.Id;
                                long _BienBanThamDinhId = await _BienBanThamDinhRepos.InsertAndGetIdAsync(insertInput);
                                hosoxl.BienBanThamDinhId_ChuyenVienThuLy = _BienBanThamDinhId;
                            }
                            hosoxl.ChuyenVienThuLyDaDuyet = true;
                            hosoxl.DonViGui = (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET;
                        }
                        else if (_session.UserId == hosoxl.ChuyenVienPhoiHopId)
                        {
                            if (hosoxl.BienBanThamDinhId_ChuyenVienPhoiHop.HasValue)
                            {
                                var updateData = await _BienBanThamDinhRepos.GetAsync(hosoxl.BienBanThamDinhId_ChuyenVienPhoiHop.Value);
                                updateData.ThuTucId = hosoxl.ThuTucId;
                                updateData.HoSoId = hosoxl.HoSoId;
                                updateData.HoSoXuLyId = hosoxl.Id;
                                input.BienBanThamDinh.MapTo(updateData);
                                await _BienBanThamDinhRepos.UpdateAsync(updateData);
                            }
                            else
                            {
                                var insertInput = input.BienBanThamDinh.MapTo<XBienBanThamDinh>();
                                insertInput.ThuTucId = hosoxl.ThuTucId;
                                insertInput.HoSoId = hosoxl.HoSoId;
                                insertInput.HoSoXuLyId = hosoxl.Id;
                                long _BienBanThamDinhId = await _BienBanThamDinhRepos.InsertAndGetIdAsync(insertInput);
                                hosoxl.BienBanThamDinhId_ChuyenVienPhoiHop = _BienBanThamDinhId;
                            }
                            hosoxl.ChuyenVienPhoiHopDaDuyet = true;
                            hosoxl.DonViGui = (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_PHOI_HOP_THAM_XET;
                        }
                        #endregion

                        hosoxl.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET_TONG_HOP;

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
        #endregion

        #region Tổng hợp thẩm định
        public async Task<dynamic> LoadTongHopThamDinh(LoadThamDinh99InputDto input)
        {
            try
            {
                if (input.HoSoXuLyId > 0)
                {
                    var objGetThamDinh = await GetBienBanThamDinh(input.HoSoXuLyId);
                    if (objGetThamDinh != null)
                    {
                        if (objGetThamDinh.HoSoXuLy.HoSoXuLyHistoryId_Active.HasValue)
                        {
                            var _history = await _hoSoXuLyHistoryRepos.GetAsync(objGetThamDinh.HoSoXuLy.HoSoXuLyHistoryId_Active.Value);

                            objGetThamDinh.DuyetHoSo = _history.MapTo<XHoSoXuLyHistoryDto>();
                        }

                        return objGetThamDinh;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                return null;
            }
        }

        public async Task<int> TongHopThamDinh_Luu(LuuThamDinh99InputDto input)
        {
            try
            {
                if (input.HoSoXuLyId > 0)
                {
                    var hosoxl = await _hoSoXuLyRepos.FirstOrDefaultAsync(input.HoSoXuLyId);
                    if (hosoxl != null && _session.UserId == hosoxl.ChuyenVienThuLyId)
                    {
                        #region Lưu lịch sử
                        var _history = new XHoSoXuLyHistory();

                        if (hosoxl.HoSoXuLyHistoryId_Active.HasValue)
                        {
                            _history = _hoSoXuLyHistoryRepos.Get(hosoxl.HoSoXuLyHistoryId_Active.Value);
                        }

                        _history.HoSoXuLyId = hosoxl.Id;
                        _history.HoSoId = hosoxl.HoSoId;
                        _history.IsHoSoBS = hosoxl.IsHoSoBS;
                        _history.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET_TONG_HOP;
                        _history.NguoiXuLyId = _session.UserId;
                        _history.HoSoIsDat_Pre = hosoxl.HoSoIsDat;
                        _history.HoSoIsDat = input.HoSoIsDat_Input;
                        _history.TrangThaiCV = input.TrangThaiCV;
                        _history.TieuDeCV = input.TieuDeCV_Input;
                        _history.NoiDungCV = input.NoiDungCV_Input;
                        _history.IsChuyenNhanh = input.IsChuyenNhanh;
                        _history.LyDoChuyenNhanh = input.LyDoChuyenNhanh;
                        #endregion

                        var _historyId = await _hoSoXuLyHistoryRepos.InsertOrUpdateAndGetIdAsync(_history);
                        hosoxl.HoSoXuLyHistoryId_Active = _historyId;

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

        public async Task<int> TongHopThamDinh_Chuyen(LuuThamDinh99InputDto input)
        {
            try
            {
                if (input.HoSoXuLyId > 0)
                {
                    var hosoxl = await _hoSoXuLyRepos.FirstOrDefaultAsync(input.HoSoXuLyId);
                    if (hosoxl != null && _session.UserId == hosoxl.ChuyenVienThuLyId)
                    {
                        #region Lưu lịch sử
                        var _history = new XHoSoXuLyHistory();

                        if (hosoxl.HoSoXuLyHistoryId_Active.HasValue)
                        {
                            _history = _hoSoXuLyHistoryRepos.Get(hosoxl.HoSoXuLyHistoryId_Active.Value);
                        }

                        _history.HoSoXuLyId = hosoxl.Id;
                        _history.HoSoId = hosoxl.HoSoId;
                        _history.IsHoSoBS = hosoxl.IsHoSoBS;
                        _history.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET_TONG_HOP;
                        _history.NguoiXuLyId = _session.UserId;
                        _history.HoSoIsDat_Pre = hosoxl.HoSoIsDat;
                        _history.HoSoIsDat = input.HoSoIsDat_Input;
                        _history.TrangThaiCV = input.TrangThaiCV;
                        _history.TieuDeCV = input.TieuDeCV_Input;
                        _history.NoiDungCV = input.NoiDungCV_Input;
                        _history.ActionEnum = (int)CommonENum.FORM_FUNCTION.CHUYEN_VIEN_DUYET_THAM_XET;

                        //Thêm lý do khi thực hiện chuyển nhanh
                        if (input.IsChuyenNhanh != null && input.IsChuyenNhanh.Value)
                        {
                            _history.LyDoChuyenNhanh = input.LyDoChuyenNhanh;
                            hosoxl.YKienGui = input.LyDoChuyenNhanh;
                            hosoxl.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG;
                            hosoxl.NguoiXuLyId = hosoxl.TruongPhongId;
                        }
                        else
                        {
                            hosoxl.YKienGui = null;
                            hosoxl.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.PHO_PHONG;
                            hosoxl.PhoPhongId = input.PhoPhongId;
                            hosoxl.NguoiXuLyId = input.PhoPhongId;
                        }
                        #endregion

                        var _historyId = await _hoSoXuLyHistoryRepos.InsertOrUpdateAndGetIdAsync(_history);
                        hosoxl.HoSoXuLyHistoryId_Active = null;
                        hosoxl.NguoiGuiId = _session.UserId;

                        hosoxl.HoSoIsDat = input.HoSoIsDat_Input;
                        if (input.HoSoIsDat_Input != null)
                        {
                            hosoxl.NoiDungCV = input.NoiDungCV_Input;
                        }

                        hosoxl.DonViGui = (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET_TONG_HOP;

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
        #endregion

        #region Thẩm định lại
        public async Task<dynamic> LoadThamDinhLai(LoadThamDinh99InputDto input)
        {
            try
            {
                if (input.HoSoXuLyId > 0)
                {
                    var objGetThamDinh = await GetBienBanThamDinh(input.HoSoXuLyId);
                    if (objGetThamDinh != null)
                    {
                        if (objGetThamDinh.HoSoXuLy.HoSoXuLyHistoryId_Active.HasValue)
                        {
                            var _history = await _hoSoXuLyHistoryRepos.GetAsync(objGetThamDinh.HoSoXuLy.HoSoXuLyHistoryId_Active.Value);

                            objGetThamDinh.DuyetHoSo = _history.MapTo<XHoSoXuLyHistoryDto>();
                        }

                        return objGetThamDinh;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                return null;
            }
        }

        public async Task<int> ThamDinhLai_Luu(LuuThamDinh99InputDto input)
        {
            try
            {
                if (input.HoSoXuLyId > 0)
                {
                    var hosoxl = await _hoSoXuLyRepos.FirstOrDefaultAsync(input.HoSoXuLyId);
                    if (hosoxl != null && _session.UserId == hosoxl.ChuyenVienThuLyId)
                    {
                        #region Lưu lịch sử
                        var _history = new XHoSoXuLyHistory();

                        if (hosoxl.HoSoXuLyHistoryId_Active.HasValue)
                        {
                            _history = _hoSoXuLyHistoryRepos.Get(hosoxl.HoSoXuLyHistoryId_Active.Value);
                        }

                        _history.HoSoXuLyId = hosoxl.Id;
                        _history.HoSoId = hosoxl.HoSoId;
                        _history.IsHoSoBS = hosoxl.IsHoSoBS;
                        _history.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET_TONG_HOP;
                        _history.NoiDungYKien = input.LyDoChuyenNhanh;
                        _history.NguoiXuLyId = _session.UserId;
                        _history.HoSoIsDat_Pre = hosoxl.HoSoIsDat;
                        _history.HoSoIsDat = input.HoSoIsDat_Input;
                        _history.TrangThaiCV = input.TrangThaiCV;
                        _history.TieuDeCV = input.TieuDeCV_Input;
                        _history.NoiDungCV = input.NoiDungCV_Input;
                        _history.IsChuyenNhanh = input.IsChuyenNhanh;
                        _history.LyDoChuyenNhanh = input.LyDoChuyenNhanh;
                        #endregion

                        var _historyId = await _hoSoXuLyHistoryRepos.InsertOrUpdateAndGetIdAsync(_history);
                        hosoxl.HoSoXuLyHistoryId_Active = _historyId;

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

        public async Task<int> ThamDinhLai_Chuyen(LuuThamDinh99InputDto input)
        {
            try
            {
                if (input.HoSoXuLyId > 0)
                {
                    var hosoxl = await _hoSoXuLyRepos.FirstOrDefaultAsync(input.HoSoXuLyId);
                    if (hosoxl != null && _session.UserId == hosoxl.ChuyenVienThuLyId)
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
                        _history.IsHoSoBS = hosoxl.IsHoSoBS;
                        _history.NguoiXuLyId = _session.UserId;
                        _history.ActionEnum = (int)CommonENum.FORM_FUNCTION.THAM_XET_LAI;
                        _history.NguoiXuLyId = _session.UserId;
                        _history.HoSoIsDat_Pre = hosoxl.HoSoIsDat;
                        _history.HoSoIsDat = input.HoSoIsDat_Input;
                        _history.TrangThaiCV = input.TrangThaiCV;
                        _history.TieuDeCV = input.TieuDeCV_Input;
                        _history.NoiDungCV = input.NoiDungCV_Input;
                        _history.IsChuyenNhanh = input.IsChuyenNhanh;
                        _history.LyDoChuyenNhanh = input.LyDoChuyenNhanh;
                        _history.TruongPhongId = input.TruongPhongId;
                        _history.PhoPhongId = input.PhoPhongId;
                        _history.NoiDungYKien = input.LyDoChuyenNhanh;
                        #endregion

                        var _historyId = await _hoSoXuLyHistoryRepos.InsertOrUpdateAndGetIdAsync(_history);
                        hosoxl.HoSoXuLyHistoryId_Active = null;

                        hosoxl.HoSoIsDat = input.HoSoIsDat_Input;
                        if (input.HoSoIsDat_Input != null)
                        {
                            hosoxl.NoiDungCV = input.NoiDungCV_Input;
                        }

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
        #endregion

        #region Thẩm định bổ sung
        public async Task<dynamic> LoadThamDinhBoSung(LoadThamDinh99InputDto input)
        {
            try
            {
                if (input.HoSoXuLyId > 0)
                {
                    var objGetThamDinh = await GetBienBanThamDinh(input.HoSoXuLyId);
                    if (objGetThamDinh != null)
                    {
                        if (objGetThamDinh.HoSoXuLy.HoSoXuLyHistoryId_Active.HasValue)
                        {
                            var _history = await _hoSoXuLyHistoryRepos.GetAsync(objGetThamDinh.HoSoXuLy.HoSoXuLyHistoryId_Active.Value);

                            objGetThamDinh.DuyetHoSo = _history.MapTo<XHoSoXuLyHistoryDto>();
                        }

                        return objGetThamDinh;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                return null;
            }
        }

        public async Task<int> ThamDinhBoSung_Luu(LuuThamDinh99InputDto input)
        {
            try
            {
                if (input.Id > 0) //input.Id: HoSoXuLyId_Active
                {
                    var hosoxl = await _hoSoXuLyRepos.FirstOrDefaultAsync(input.Id);
                    if (hosoxl == null)
                    {
                        return 0;
                    }
                    //Thẩm định 1 bổ sung
                    if (_session.UserId == hosoxl.ChuyenVienThuLyId && hosoxl.IsHoSoBS == true)
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
                        _history.IsHoSoBS = hosoxl.IsHoSoBS;
                        _history.NguoiXuLyId = _session.UserId;
                        _history.ActionEnum = (int)CommonENum.FORM_FUNCTION.THAM_XET_HO_SO_BO_SUNG;
                        _history.NguoiXuLyId = _session.UserId;
                        _history.HoSoIsDat_Pre = hosoxl.HoSoIsDat;
                        _history.HoSoIsDat = input.HoSoIsDat_Input;
                        _history.TrangThaiCV = input.TrangThaiCV;
                        _history.TieuDeCV = input.TieuDeCV_Input;
                        _history.NoiDungCV = input.NoiDungCV_Input;
                        _history.IsChuyenNhanh = input.IsChuyenNhanh;
                        _history.LyDoChuyenNhanh = input.LyDoChuyenNhanh;
                        _history.TruongPhongId = input.TruongPhongId;
                        _history.PhoPhongId = input.PhoPhongId;
                        _history.NoiDungYKien = input.LyDoChuyenNhanh;
                        #endregion

                        var _historyId = await _hoSoXuLyHistoryRepos.InsertOrUpdateAndGetIdAsync(_history);
                        hosoxl.HoSoXuLyHistoryId_Active = _historyId;

                        hosoxl.HoSoIsDat = input.HoSoIsDat_Input;
                        if (input.HoSoIsDat_Input != null)
                        {
                            hosoxl.NoiDungCV = input.NoiDungCV_Input;
                        }

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

        public async Task<int> ThamDinhBoSung_Chuyen(LuuThamDinh99InputDto input)
        {
            try
            {
                if (input.Id == 0)
                {
                    return 0;
                }
                var hosoxl = await _hoSoXuLyRepos.FirstOrDefaultAsync(input.Id);
                if (hosoxl == null)
                {
                    return 0;
                }
                //Thẩm định 1 bổ sung
                if (_session.UserId == hosoxl.ChuyenVienThuLyId && hosoxl.IsHoSoBS == true)
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
                    _history.IsHoSoBS = hosoxl.IsHoSoBS;
                    _history.NguoiXuLyId = _session.UserId;
                    _history.ActionEnum = (int)CommonENum.FORM_FUNCTION.THAM_XET_HO_SO_BO_SUNG;
                    _history.NguoiXuLyId = _session.UserId;
                    _history.HoSoIsDat_Pre = hosoxl.HoSoIsDat;
                    _history.HoSoIsDat = input.HoSoIsDat_Input;
                    _history.TrangThaiCV = input.TrangThaiCV;
                    _history.TieuDeCV = input.TieuDeCV_Input;
                    _history.NoiDungCV = input.NoiDungCV_Input;
                    _history.IsChuyenNhanh = input.IsChuyenNhanh;
                    _history.LyDoChuyenNhanh = input.LyDoChuyenNhanh;
                    _history.TruongPhongId = input.TruongPhongId;
                    _history.PhoPhongId = input.PhoPhongId;
                    _history.NoiDungYKien = input.LyDoChuyenNhanh;
                    #endregion

                    var _historyId = await _hoSoXuLyHistoryRepos.InsertOrUpdateAndGetIdAsync(_history);
                    hosoxl.HoSoXuLyHistoryId_Active = null;

                    hosoxl.HoSoIsDat = input.HoSoIsDat_Input;
                    if (input.HoSoIsDat_Input != null)
                    {
                        hosoxl.NoiDungCV = input.NoiDungCV_Input;
                    }

                    await _hoSoXuLyRepos.UpdateAsync(hosoxl);
                }

                return 1;
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                return 0;
            }
        }
        #endregion
    }
    #endregion
}