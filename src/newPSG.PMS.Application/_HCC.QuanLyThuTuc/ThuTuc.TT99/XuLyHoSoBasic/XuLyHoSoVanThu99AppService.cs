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
    public interface IXuLyHoSoVanThu99AppService : IApplicationService
    {
        Task<dynamic> LoadVanThuDuyet(VanThuXuLy99InputDto input);
        Task DongDau(VanThuXuLy99InputDto input);
        Task BaoCaoSaiSot(VanThuXuLy99InputDto input);
    }
    #endregion

    #region MAIN
    [AbpAuthorize]
    public class XuLyHoSoVanThu99AppService : PMSAppServiceBase, IXuLyHoSoVanThu99AppService
    {
        private readonly IRepository<XHoSo, long> _hoSoRepos;
        private readonly IRepository<XHoSoTepDinhKem, long> _hoSoTepDinhKemRepos;
        private readonly IRepository<XHoSoXuLy, long> _hoSoXuLyRepos;
        private readonly IRepository<XHoSoXuLyHistory, long> _hoSoXuLyHistoryRepos;
        private readonly IRepository<User, long> _userRepos;
        private readonly IAbpSession _session;

        public XuLyHoSoVanThu99AppService(IRepository<XHoSo, long> hoSoRepos,
                                          IRepository<XHoSoTepDinhKem, long> hoSoTepDinhKemRepos,
                                          IRepository<XHoSoXuLy, long> hoSoXuLyRepos,
                                          IRepository<XHoSoXuLyHistory, long> hoSoXuLyHistoryRepos,
                                          IRepository<User, long> userRepos,
                                          IAbpSession session)
        {
            _hoSoRepos = hoSoRepos;
            _hoSoTepDinhKemRepos = hoSoTepDinhKemRepos;
            _hoSoXuLyRepos = hoSoXuLyRepos;
            _hoSoXuLyHistoryRepos = hoSoXuLyHistoryRepos;
            _userRepos = userRepos;
            _session = session;
        }

        public async Task<dynamic> LoadVanThuDuyet(VanThuXuLy99InputDto input)
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

        public async Task DongDau(VanThuXuLy99InputDto input)
        {
            var vanThuId = _session.UserId;
            try
            {
                var hoSo = await _hoSoRepos.FirstOrDefaultAsync(x => x.Id == input.HoSoId);
                var hosoxl = await _hoSoXuLyRepos.FirstOrDefaultAsync(input.HoSoXuLyId);
                if (hosoxl != null && hoSo != null && hosoxl.Id > 0)
                {
                    if (hosoxl.HoSoIsDat == true)
                    {
                        hoSo.TrangThaiHoSo = (int)CommonENum.TRANG_THAI_HO_SO.DA_HOAN_TAT;
                        hoSo.NgayTraKetQua = DateTime.Now;
                    }
                    else 
                    {
                        hoSo.TrangThaiHoSo = (int)CommonENum.TRANG_THAI_HO_SO.SUA_DOI_BO_SUNG;
                        hoSo.IsHoSoBS = true;
                    }

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
                    hosoxl.GiayTiepNhanCA = input.GiayTiepNhanCA;
                    if (hosoxl.HoSoIsDat == true)
                    {
                        hoSo.GiayTiepNhan = input.GiayTiepNhanCA;
                        hoSo.GiayTiepNhanFull = input.GiayTiepNhanCA;
                    }
                    else
                    {
                        hoSo.GiayTiepNhan = null;
                        hoSo.GiayTiepNhanFull = null;
                    }
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

        public async Task BaoCaoSaiSot(VanThuXuLy99InputDto input)
        {
            try
            {
                var hosoxl = await _hoSoXuLyRepos.FirstOrDefaultAsync(input.HoSoXuLyId);
                if (hosoxl != null && hosoxl.Id > 0)
                {
                    #region Lưu lịch sử
                    var _history = new XHoSoXuLyHistory();
                    _history.ThuTucId = hosoxl.ThuTucId;
                    _history.HoSoXuLyId = hosoxl.Id;
                    _history.HoSoId = hosoxl.HoSoId;
                    _history.IsHoSoBS = hosoxl.IsHoSoBS;
                    _history.NguoiXuLyId = _session.UserId;
                    _history.NoiDungYKien = input.NoiDungYKien;
                    _history.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.VAN_THU;
                    _history.ActionEnum = (int)CommonENum.FORM_FUNCTION.VAN_THU_BAO_CAO_CONG_VAN_CO_SAI_SOT;
                    _history.HoSoIsDat = hosoxl.HoSoIsDat;
                    _history.HoSoIsDat_Pre = hosoxl.HoSoIsDat;
                    _history.TrangThaiCV = null;

                    await _hoSoXuLyHistoryRepos.InsertAndGetIdAsync(_history);
                    #endregion

                    hosoxl.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.LANH_DAO_CUC;
                    hosoxl.DonViGui = (int)CommonENum.DON_VI_XU_LY.VAN_THU;
                    hosoxl.NguoiGuiId = _session.UserId;
                    hosoxl.NguoiXuLyId = hosoxl.LanhDaoCucId;
                    hosoxl.NgayGui = DateTime.Now;
                    hosoxl.VanThuDaDuyet = true;
                    hosoxl.YKienGui = _history.NoiDungYKien;

                    await _hoSoXuLyRepos.UpdateAsync(hosoxl);
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
            }
        }
    }
    #endregion
}