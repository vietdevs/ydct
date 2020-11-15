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
using newPSG.PMS.EntityDB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    public interface IXuLyHoSoPhanCong99AppService : IApplicationService
    {
        #region Một cửa tiếp nhận
        dynamic LoadPhanCongPhongBan(LoadPhanCongPhongBan99Dto input);
        Task<int> PhanCongPhongBan(PhanCongPhongBan99InputDto input);
        Task<int> YeuCauThanhToan(PhanCongPhongBan99InputDto input);
        Task<int> TuChoiTiepNhan(PhanCongPhongBan99InputDto input);
        Task<int> KyGiayTiepNhanVaTraDoanhNghiep(MotCuaTraGiayTiepNhan99InputDto input);
        #endregion

        #region Phòng ban phân công
        dynamic GetThongKePhanCong(List<ItemDto<long>> chuyenViens);
        Task<dynamic> InitPhanCongCanBo(InitPhanCong99InputDto input);
        Task<int> PhanCongThamDinh(PhanCongThamDinh99InputDto input);
        #endregion
    }
    #endregion

    #region MAIN
    [AbpAuthorize]
    public class XuLyHoSoPhanCong99AppService : PMSAppServiceBase, IXuLyHoSoPhanCong99AppService
    {
        private readonly IRepository<XHoSo, long> _hoSoRepos;
        private readonly IRepository<XHoSoXuLy, long> _hoSoXuLyRepos;
        private readonly IRepository<XHoSoXuLyHistory, long> _hoSoXuLyHistoryRepos;
        private readonly IRepository<User, long> _userRepos;
        private readonly IAbpSession _session;

        public XuLyHoSoPhanCong99AppService(IRepository<XHoSo, long> hoSoRepos,
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

        #region Một cửa phân công
        public async Task<int> KyGiayTiepNhanVaTraDoanhNghiep(MotCuaTraGiayTiepNhan99InputDto input)
        {
            try
            {
                var hoSo = await _hoSoRepos.GetAsync(input.HoSoId);
                hoSo.SoGiayTiepNhan = input.SoTiepNhan;
                if (!string.IsNullOrEmpty(input.GiayTiepNhanCA))
                {
                    hoSo.GiayTiepNhan = input.GiayTiepNhanCA;
                }
                await _hoSoRepos.UpdateAsync(hoSo);
                var hoSoXuLy = await _hoSoXuLyRepos.GetAsync(input.HoSoXuLyId);
                if (hoSoXuLy != null)
                {
                    #region Lưu lịch sử
                    var _history = new XHoSoXuLyHistory();
                    _history.HoSoXuLyId = hoSoXuLy.Id;
                    _history.ThuTucId = hoSo.ThuTucId;
                    _history.HoSoId = hoSoXuLy.HoSoId;
                    _history.IsHoSoBS = hoSoXuLy.IsHoSoBS;
                    _history.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.MOT_CUA_TIEP_NHAN;
                    _history.NguoiXuLyId = _session.UserId;
                    _history.HoSoIsDat_Pre = hoSoXuLy.HoSoIsDat;
                    _history.HoSoIsDat = hoSoXuLy.HoSoIsDat;
                    _history.TrangThaiCV = input.TrangThaiCV;
                    _history.NoiDungCV = null;
                    _history.NoiDungYKien = "Bộ phận một cửa trả giấy tiếp nhận";
                    _history.ActionEnum = (int)CommonENum.FORM_FUNCTION.MOT_CUA_PHAN_CONG_HO_SO;
                    await _hoSoXuLyHistoryRepos.InsertAndGetIdAsync(_history);
                    #endregion
                    hoSoXuLy.NguoiGuiId = _session.UserId;
                    hoSoXuLy.NguoiXuLyId = null;
                    hoSoXuLy.DonViKeTiep = input.DonViKeTiep;
                    if (!string.IsNullOrEmpty(input.GiayTiepNhanCA))
                    {
                        hoSoXuLy.GiayTiepNhanCA = input.GiayTiepNhanCA;
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

        public dynamic LoadPhanCongPhongBan(LoadPhanCongPhongBan99Dto input)
        {
            try
            {
                var _list = from pb in input.ArrPhongBanXuLy
                            select new
                            {
                                PhongBanId = pb.Id,
                                TenPhongBan = pb.Name,
                                SumHoSoXuLy = (from p in _hoSoRepos.GetAll()
                                               where
                                                    p.LoaiHoSoId == input.LoaiHoSoId
                                                    && p.PhongBanId == pb.Id
                                                    && p.PId == null
                                                    && (p.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_DA_TIEP_NHAN
                                                    || p.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_BO_SUNG)
                                               select 1).Count()
                            };

                var res = new
                {
                    listThongKePhanCong = _list.ToList()
                };

                return res;
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                return null;
            }
        }

        public async Task<int> PhanCongPhongBan(PhanCongPhongBan99InputDto input)
        {
            try
            {
                int count = 0;
                foreach (var _id in input.ArrHoSoId)
                {
                    var hoSo = _hoSoRepos.FirstOrDefault(_id);
                    if (hoSo.Id > 0)
                    {

                        if (hoSo.HoSoXuLyId_Active.HasValue)
                        {
                            var hosoxl = _hoSoXuLyRepos.FirstOrDefault(hoSo.HoSoXuLyId_Active.Value);
                            hosoxl.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.PHONG_BAN_PHAN_CONG;
                            hosoxl.DonViGui = (int)CommonENum.DON_VI_XU_LY.MOT_CUA_TIEP_NHAN;
                            hosoxl.NguoiGuiId = _session.UserId;
                            hosoxl.NguoiXuLyId = null;
                            hosoxl.NgayGui = DateTime.Now;

                            #region Lưu lịch sử
                            var _history = new XHoSoXuLyHistory();
                            _history.HoSoXuLyId = hosoxl.Id;
                            _history.ThuTucId = hoSo.ThuTucId;
                            _history.HoSoId = hoSo.Id;
                            _history.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.MOT_CUA_TIEP_NHAN;
                            _history.NguoiXuLyId = _session.UserId;
                            _history.NoiDungYKien = string.Format("Chuyển hồ sơ tới phòng: [{0}] {1}", input.PhongBanId, input.TenPhongBan);
                            _history.ActionEnum = (int)CommonENum.FORM_FUNCTION.MOT_CUA_PHAN_CONG_HO_SO;
                            
                            var _historyId = await _hoSoXuLyHistoryRepos.InsertOrUpdateAndGetIdAsync(_history);
                            hosoxl.HoSoXuLyHistoryId_Active = _historyId;
                            #endregion

                            await _hoSoXuLyRepos.UpdateAsync(hosoxl);
                        }
                        hoSo.MotCuaChuyenId = _session.UserId;
                        hoSo.NgayMotCuaChuyen = DateTime.Now;
                        hoSo.PhongBanId = input.PhongBanId;
                        hoSo.TrangThaiHoSo = (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_DA_TIEP_NHAN;
                        await _hoSoRepos.UpdateAsync(hoSo);

                        count++;
                    }
                }

                return count;
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                return 0;
            }
        }

        public async Task<int> YeuCauThanhToan(PhanCongPhongBan99InputDto input)
        {
            try
            {
                int count = 0;
                foreach (var _id in input.ArrHoSoId)
                {
                    var hoSo = _hoSoRepos.FirstOrDefault(_id);
                    if (hoSo.Id > 0)
                    {

                        if (hoSo.HoSoXuLyId_Active.HasValue)
                        {
                            var hosoxl = _hoSoXuLyRepos.FirstOrDefault(hoSo.HoSoXuLyId_Active.Value);
                            hosoxl.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.DOANH_NGHIEP;
                            hosoxl.DonViGui = (int)CommonENum.DON_VI_XU_LY.MOT_CUA_TIEP_NHAN;
                            hosoxl.NguoiGuiId = _session.UserId;
                            hosoxl.NguoiXuLyId = null;
                            hosoxl.NgayGui = DateTime.Now;

                            #region Lưu lịch sử
                            var _history = new XHoSoXuLyHistory();
                            _history.HoSoXuLyId = hosoxl.Id;
                            _history.ThuTucId = hoSo.ThuTucId;
                            _history.HoSoId = hoSo.Id;
                            _history.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.MOT_CUA_TIEP_NHAN;
                            _history.NguoiXuLyId = _session.UserId;
                            _history.NoiDungYKien = "Đề nghị thanh toán lệ phí hồ sơ!";
                            _history.ActionEnum = (int)CommonENum.FORM_FUNCTION.MOT_CUA_PHAN_CONG_HO_SO;
                            if (hoSo.PhongBanId.HasValue && hoSo.PhongBanId.Value > 0)
                            {
                                _history.ActionEnum = (int)CommonENum.FORM_FUNCTION.MOT_CUA_PHAN_CONG_LAI_HO_SO;
                            }

                            var _historyId = await _hoSoXuLyHistoryRepos.InsertOrUpdateAndGetIdAsync(_history);
                            hosoxl.HoSoXuLyHistoryId_Active = _historyId;
                            #endregion

                            await _hoSoXuLyRepos.UpdateAsync(hosoxl);
                        }
                        hoSo.TrangThaiHoSo = (int)CommonENum.TRANG_THAI_HO_SO.CHO_DOANH_NGHIEP_THANH_TOAN;
                        await _hoSoRepos.UpdateAsync(hoSo);
                        count++;
                    }
                }

                return count;
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                return 0;
            }
        }

        public async Task<int> TuChoiTiepNhan(PhanCongPhongBan99InputDto input)
        {
            try
            {
                int count = 0;
                foreach (var _id in input.ArrHoSoId)
                {
                    var hoSo = _hoSoRepos.FirstOrDefault(_id);

                    if (hoSo.Id > 0)
                    {
                        hoSo.TrangThaiHoSo = (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_TRA_LAI;
                        await _hoSoRepos.UpdateAsync(hoSo);

                        if (hoSo.HoSoXuLyId_Active.HasValue)
                        {
                            var hosoxl = _hoSoXuLyRepos.FirstOrDefault(hoSo.HoSoXuLyId_Active.Value);
                            hosoxl.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.DOANH_NGHIEP;
                            hosoxl.DonViGui = (int)CommonENum.DON_VI_XU_LY.MOT_CUA_TIEP_NHAN;
                            hosoxl.NguoiGuiId = _session.UserId;
                            hosoxl.NguoiXuLyId = hoSo.CreatorUserId;
                            hosoxl.YKienGui = input.LyDoTuChoi;
                            hosoxl.HoSoIsDat = null;
                            hosoxl.NgayGui = DateTime.Now;

                            #region Lưu lịch sử
                            var _history = new XHoSoXuLyHistory();
                            _history.HoSoXuLyId = hosoxl.Id;
                            _history.ThuTucId = hoSo.ThuTucId;
                            _history.HoSoId = hoSo.Id;
                            _history.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.MOT_CUA_TIEP_NHAN;
                            _history.DonViKeTiep = (int)CommonENum.DON_VI_XU_LY.DOANH_NGHIEP;
                            _history.NguoiXuLyId = _session.UserId;
                            _history.NoiDungYKien = input.LyDoTuChoi;
                            _history.ActionEnum = (int)CommonENum.FORM_FUNCTION.MOT_CUA_PHAN_CONG_HO_SO;

                            var _historyId = await _hoSoXuLyHistoryRepos.InsertOrUpdateAndGetIdAsync(_history);
                            hosoxl.HoSoXuLyHistoryId_Active = null;
                            #endregion

                            await _hoSoXuLyRepos.UpdateAsync(hosoxl);
                        }
                        count++;
                    }
                }
                return count;
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                return 0;
            }
        }

        #endregion

        #region Phòng ban phân công

        public dynamic GetThongKePhanCong(List<ItemDto<long>> chuyenViens)
        {
            try
            {
                var listCV = new List<ItemDto<long>>();
                foreach (var item in chuyenViens)
                {
                    var obj = listCV.Find(p => p.Id == item.Id);
                    if (obj == null)
                    {
                        listCV.Add(item);
                    }
                }

                var _list = from cv in listCV
                            select new
                            {
                                ChuyenVienId = cv.Id,
                                TenChuyenVien = cv.Name,
                                SumChuyenVienThuLy = (from p in _hoSoXuLyRepos.GetAll()
                                                      where (p.ChuyenVienThuLyId == cv.Id)
                                                      && (p.DonViXuLy != null ? p.DonViXuLy.Value : 0) != (int)CommonENum.DON_VI_XU_LY.PHONG_BAN_PHAN_CONG
                                                      && (p.DonViXuLy != null ? p.DonViXuLy.Value : 0) != (int)CommonENum.DON_VI_XU_LY.DOANH_NGHIEP
                                                      select 1).Count(),
                                SumChuyenVienPhoiHop = (from p in _hoSoXuLyRepos.GetAll()
                                                        where (p.ChuyenVienPhoiHopId == cv.Id)
                                                        && (p.DonViXuLy != null ? p.DonViXuLy.Value : 0) != (int)CommonENum.DON_VI_XU_LY.PHONG_BAN_PHAN_CONG
                                                        && (p.DonViXuLy != null ? p.DonViXuLy.Value : 0) != (int)CommonENum.DON_VI_XU_LY.DOANH_NGHIEP
                                                        select 1).Count()
                            };
                return _list.ToArray();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                return null;
            }
        }

        public async Task<dynamic> InitPhanCongCanBo(InitPhanCong99InputDto input)
        {
            try
            {
                //Phòng Ban Của User: input.PhongBanId
                var listTruongPhong = await (
                                        from u in _userRepos.GetAll()
                                        where u.PhongBanId == input.PhongBanId && u.RoleLevel == (int)CommonENum.ROLE_LEVEL.TRUONG_PHONG && u.IsDeleted != true && u.IsActive == true
                                        select new ItemDto<long>
                                        {
                                            Id = u.Id,
                                            Name = u.Surname + " " + u.Name
                                        }
                                      ).ToListAsync();

                long? truongPhongId = null;
                if (input.RoleLevel == (int)CommonENum.ROLE_LEVEL.TRUONG_PHONG)
                {
                    truongPhongId = input.UserId;
                }

                //Phòng Ban Của User: input.PhongBanId
                var listPhoPhong = await (
                                        from u in _userRepos.GetAll()
                                        where u.PhongBanId == input.PhongBanId && u.RoleLevel == (int)CommonENum.ROLE_LEVEL.PHO_PHONG && u.IsDeleted != true && u.IsActive == true
                                        select new ItemDto<long>
                                        {
                                            Id = u.Id,
                                            Name = u.Surname + " " + u.Name
                                        }
                                      ).ToListAsync();

                var listChuyenVien = await (
                                      from u in _userRepos.GetAll()
                                      where u.PhongBanId == input.PhongBanId && u.RoleLevel == (int)CommonENum.ROLE_LEVEL.CHUYEN_VIEN
                                      orderby u.Name
                                      select new ItemDto<long>
                                      {
                                          Id = u.Id,
                                          Name = u.Surname + " " + u.Name
                                      }
                                  ).ToListAsync();

                var res = new
                {
                    truongPhongId,
                    listTruongPhong,
                    listPhoPhong,
                    listChuyenVien,
                    listThongKePhanCong = GetThongKePhanCong(listChuyenVien)
                };

                return res;
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                return null;
            }
        }

        public async Task<int> PhanCongThamDinh(PhanCongThamDinh99InputDto input)
        {
            try
            {
                int count = 0;
                foreach (var _id in input.ArrHoSoId)
                {
                    var hosoxl = _hoSoXuLyRepos.FirstOrDefault(_id);
                    if (hosoxl.Id > 0)
                    {
                        var hoSo = _hoSoRepos.FirstOrDefault(hosoxl.HoSoId);
                        
                        #region Lưu lịch sử
                        var _history = new XHoSoXuLyHistory();
                        _history.HoSoXuLyId = hosoxl.Id;
                        _history.ThuTucId = hosoxl.ThuTucId;
                        _history.HoSoId = hosoxl.HoSoId;
                        _history.IsHoSoBS = hosoxl.IsHoSoBS;
                        _history.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.PHONG_BAN_PHAN_CONG;
                        _history.NguoiXuLyId = _session.UserId;
                        _history.ActionEnum = (int)CommonENum.FORM_FUNCTION.PHONG_BAN_PHAN_CONG;
                        if (hosoxl.ChuyenVienThuLyId != null && hosoxl.ChuyenVienThuLyDaDuyet == null)
                        {
                            _history.ActionEnum = (int)CommonENum.FORM_FUNCTION.PHAN_CONG_LAI_HO_SO_CHUA_XU_LY;
                        }
                       
                        _history.ChuyenVienThuLyId = input.ChuyenVienThuLyId;
                        _history.ChuyenVienPhoiHopId = input.ChuyenVienPhoiHopId;

                        _history.IsChuyenNhanh = input.IsChuyenNhanh;
                        await _hoSoXuLyHistoryRepos.InsertOrUpdateAsync(_history);
                        #endregion

                        hosoxl.HoSoXuLyHistoryId_Active = null;
                        //Update trạng thái hồ sơ
                        hosoxl.TruongPhongId = input.TruongPhongId;
                        hosoxl.ChuyenVienThuLyId = input.ChuyenVienThuLyId;
                        hosoxl.PhoPhongId = input.PhoPhongId;
                        hosoxl.ChuyenVienPhoiHopId = input.ChuyenVienPhoiHopId;
                     
                        hosoxl.NguoiXuLyId = input.ChuyenVienThuLyId;
                        hosoxl.DonViGui = (int)CommonENum.DON_VI_XU_LY.PHONG_BAN_PHAN_CONG;
                        hosoxl.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET;

                        hosoxl.NguoiGuiId = _session.UserId;
                        hosoxl.YKienGui = null;
                        hosoxl.NgayGui = DateTime.Now;
                        await _hoSoXuLyRepos.UpdateAsync(hosoxl);

                        count++;
                    }
                }

                return count;
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