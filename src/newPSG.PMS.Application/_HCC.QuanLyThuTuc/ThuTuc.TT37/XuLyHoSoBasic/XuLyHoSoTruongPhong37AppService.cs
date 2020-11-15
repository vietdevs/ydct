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
    public interface IXuLyHoSoTruongPhong37AppService : IApplicationService
    {
        Task<dynamic> InitTruongPhongDuyet(InitTruongPhongDuyet37InputDto input);
        Task<dynamic> LoadTruongPhongDuyet(LoadTruongPhongDuyet37InputDto input);
        Task<int> TruongPhongDuyet(LuuTruongPhongDuyet37InputDto input);
        Task<int> TruongPhongDuyetThamDinh(LuuTruongPhongDuyet37InputDto input);
    }
    #endregion

    #region MAIN
    [AbpAuthorize]
    public class XuLyHoSoTruongPhong37AppService : PMSAppServiceBase, IXuLyHoSoTruongPhong37AppService
    {
        private readonly IRepository<XHoSo, long> _hoSoRepos;
        private readonly IRepository<XHoSoXuLy, long> _hoSoXuLyRepos;
        private readonly IRepository<XHoSoXuLyHistory, long> _hoSoXuLyHistoryRepos;
        private readonly IRepository<User, long> _userRepos;
        private readonly IAbpSession _session;

        public XuLyHoSoTruongPhong37AppService(IRepository<XHoSo, long> hoSoRepos,
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

        public async Task<dynamic> InitTruongPhongDuyet(InitTruongPhongDuyet37InputDto input)
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

        public async Task<dynamic> LoadTruongPhongDuyet(LoadTruongPhongDuyet37InputDto input)
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
                    //    _hoSoXuLyDto.BienBanThamDinh_ChuyenVienPhoiHop = new BienBanThamDinhPhoiHop37Dto()
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

        // duyệt thẩm xét hồ sơ bổ sung
        public async Task<int> TruongPhongDuyet(LuuTruongPhongDuyet37InputDto input)
        {
            try
            {
                if (input.HoSoXuLyId > 0)
                {
                    var hoso = await _hoSoRepos.GetAsync(input.HoSoId.Value);
                    var hsxl = await _hoSoXuLyRepos.FirstOrDefaultAsync(input.HoSoXuLyId);
                    if (hsxl != null)
                    {
                        if (input.IsTraLaiChuyenVien.HasValue && input.IsTraLaiChuyenVien.Value)
                        {
                            
                            hsxl.LyDoTraLai = input.NoiDungYKien;
                            hsxl.DonViGui = (int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG;
                            hsxl.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET;
                            hsxl.NguoiGuiId = _session.UserId;
                            hsxl.NguoiXuLyId = hsxl.ChuyenVienThuLyId;
                            hsxl.TruongPhongDaDuyet = null;
                            hsxl.TruongPhongNgayDuyet = DateTime.Now;
                            hoso.TrangThaiHoSo = (int)CommonENum.TRANG_THAI_HO_SO.HO_SO_THAM_XET_LAI;
                        }
                        else
                        {
                            hsxl.NoiDungCV = input.NoiDungCV;
                            hsxl.DonViGui = (int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG;
                            hsxl.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.LANH_DAO_CUC;
                            hsxl.NguoiGuiId = _session.UserId;
                            hsxl.NguoiXuLyId = hsxl.LanhDaoCucId;
                            hsxl.TruongPhongDaDuyet = true;
                            hsxl.TruongPhongNgayDuyet = DateTime.Now;

                            hoso.TrangThaiHoSo = (int)CommonENum.TRANG_THAI_HO_SO.SUA_DOI_BO_SUNG;
                        }



                        hsxl.HoSoXuLyHistoryId_Active = null;
                        hsxl.TruongPhongDaDuyet = true;

                        //Lưu hồ sơ
                        #region Lưu lịch sử
                        var _history = new XHoSoXuLyHistory();

                        if (hsxl.HoSoXuLyHistoryId_Active.HasValue)
                        {
                            _history = _hoSoXuLyHistoryRepos.Get(hsxl.HoSoXuLyHistoryId_Active.Value);
                        }

                        _history.ThuTucId = hsxl.ThuTucId;
                        _history.HoSoXuLyId = hsxl.Id;
                        _history.HoSoId = input.HoSoId;
                        _history.IsHoSoBS = hsxl.IsHoSoBS;
                        _history.NgayXuLy = DateTime.Now;
                        _history.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG;
                        _history.ActionEnum = (int)CommonENum.FORM_FUNCTION.TRUONG_PHONG_DUYET;
                        _history.NguoiXuLyId = _session.UserId;

                        var _historyId = await _hoSoXuLyHistoryRepos.InsertOrUpdateAndGetIdAsync(_history);
                        hsxl.HoSoXuLyHistoryId_Active = _historyId;

                        #endregion

                        await _hoSoRepos.UpdateAsync(hoso);
                        await _hoSoXuLyRepos.UpdateAsync(hsxl);


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

        // duyệt thẩm định hồ sơ bổ sung
        public async Task<int> TruongPhongDuyetThamDinh(LuuTruongPhongDuyet37InputDto input)
        {
            try
            {
                if (input.HoSoXuLyId > 0)
                {
                    var hoso = await _hoSoRepos.GetAsync(input.HoSoId.Value);
                    var hsxl = await _hoSoXuLyRepos.FirstOrDefaultAsync(input.HoSoXuLyId);
                    if (hsxl != null)
                    {
                        if (input.IsTraLaiChuyenVien.HasValue && input.IsTraLaiChuyenVien.Value)
                        {

                            hsxl.LyDoTraLai = input.NoiDungYKien;
                            hsxl.DonViGui = (int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG;
                            hsxl.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET_TONG_HOP;
                            hsxl.NguoiGuiId = _session.UserId;
                            hsxl.NguoiXuLyId = hsxl.ChuyenVienThuLyId;
                            hsxl.TruongPhongDaDuyet = null;
                            hsxl.TruongPhongNgayDuyet = DateTime.Now;
                            hoso.TrangThaiHoSo = (int)CommonENum.TRANG_THAI_HO_SO.HO_SO_THAM_DINH_LAI;
                        }
                        else
                        {
                            hsxl.NoiDungCV = input.NoiDungCV;
                            hsxl.DonViGui = (int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG;
                            hsxl.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.LANH_DAO_CUC;
                            hsxl.NguoiGuiId = _session.UserId;
                            hsxl.NguoiXuLyId = hsxl.LanhDaoCucId;
                            hsxl.TruongPhongDaDuyet = true;
                            hsxl.TruongPhongNgayDuyet = DateTime.Now;

                            hoso.TrangThaiHoSo = (int)CommonENum.TRANG_THAI_HO_SO.SUA_DOI_BO_SUNG;
                        }



                        hsxl.HoSoXuLyHistoryId_Active = null;
                        hsxl.TruongPhongDaDuyet = true;

                        //Lưu hồ sơ
                        #region Lưu lịch sử
                        var _history = new XHoSoXuLyHistory();

                        if (hsxl.HoSoXuLyHistoryId_Active.HasValue)
                        {
                            _history = _hoSoXuLyHistoryRepos.Get(hsxl.HoSoXuLyHistoryId_Active.Value);
                        }

                        _history.ThuTucId = hsxl.ThuTucId;
                        _history.NgayXuLy = DateTime.Now;
                        _history.HoSoXuLyId = hsxl.Id;
                        _history.HoSoId = input.HoSoId;
                        _history.IsHoSoBS = hsxl.IsHoSoBS;
                        _history.LuongXuLy = (int)CommonENum.LUONG_XU_LY_TT37.LUONG_THAM_DINH;
                        _history.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG;
                        _history.ActionEnum = (int)CommonENum.FORM_FUNCTION.TRUONG_PHONG_DUYET_THAM_DINH;
                        _history.NguoiXuLyId = _session.UserId;

                        var _historyId = await _hoSoXuLyHistoryRepos.InsertOrUpdateAndGetIdAsync(_history);
                        hsxl.HoSoXuLyHistoryId_Active = _historyId;

                        #endregion

                        await _hoSoRepos.UpdateAsync(hoso);
                        await _hoSoXuLyRepos.UpdateAsync(hsxl);


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