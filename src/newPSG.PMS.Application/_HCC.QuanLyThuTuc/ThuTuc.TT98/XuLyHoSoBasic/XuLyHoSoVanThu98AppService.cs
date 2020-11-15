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
    public interface IXuLyHoSoVanThu98AppService : IApplicationService
    {
        Task<dynamic> LoadVanThuDuyet(VanThuXuLy98InputDto input);
        Task VanThuDuyet_Chuyen(VanThuXuLy98InputDto input);
    }
    #endregion

    #region MAIN
    [AbpAuthorize]
    public class XuLyHoSoVanThu98AppService : PMSAppServiceBase, IXuLyHoSoVanThu98AppService
    {
        private readonly IRepository<XHoSo, long> _hoSoRepos;
        private readonly IRepository<XHoSoTepDinhKem, long> _hoSoTepDinhKemRepos;
        private readonly IRepository<XHoSoXuLy, long> _hoSoXuLyRepos;
        private readonly IRepository<XHoSoXuLyHistory, long> _hoSoXuLyHistoryRepos;
        private readonly IRepository<User, long> _userRepos;
        private readonly IAbpSession _session;

        public XuLyHoSoVanThu98AppService(IRepository<XHoSo, long> hoSoRepos,
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

        public async Task<dynamic> LoadVanThuDuyet(VanThuXuLy98InputDto input)
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

        public async Task VanThuDuyet_Chuyen(VanThuXuLy98InputDto input)
        {
            var vanThuId = _session.UserId;
            try
            {
                var hoSo = await _hoSoRepos.FirstOrDefaultAsync(x => x.Id == input.HoSoId);
                var hosoxl = await _hoSoXuLyRepos.FirstOrDefaultAsync(input.HoSoXuLyId);
                if (hosoxl != null && hoSo != null && hosoxl.Id > 0)
                {
                    if (input.TrangThaiXuLy == (int)CommonENum.KET_QUA_XU_LY.DAT)
                    {
                        hoSo.TrangThaiHoSo = (int)CommonENum.TRANG_THAI_HO_SO.DA_HOAN_TAT;
                        hoSo.NgayTraKetQua = DateTime.Now;
                    }
                    else if (input.TrangThaiXuLy == (int)CommonENum.KET_QUA_XU_LY.BO_SUNG)
                    {
                        hoSo.TrangThaiHoSo = (int)CommonENum.TRANG_THAI_HO_SO.SUA_DOI_BO_SUNG;
                        hoSo.IsHoSoBS = true;
                    }
                    else if (input.TrangThaiXuLy == (int)CommonENum.KET_QUA_XU_LY.KHONG_DAT)
                    {
                        hoSo.TrangThaiHoSo = (int)CommonENum.TRANG_THAI_HO_SO.TU_CHOI_CAP_PHEP;
                        hoSo.NgayTraKetQua = DateTime.Now;
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
                    if (input.TrangThaiXuLy == (int)CommonENum.KET_QUA_XU_LY.DAT)
                    {
                        hoSo.GiayTiepNhan = input.DuongDanTepCA;
                        hoSo.GiayTiepNhanFull = input.DuongDanTepCA;
                        hosoxl.GiayTiepNhanCA = input.DuongDanTepCA;
                        hosoxl.HoSoIsDat = true;
                    }
                    else
                    {
                        hoSo.GiayTiepNhan = null;
                        hoSo.GiayTiepNhanFull = null;
                        hosoxl.HoSoIsDat = false;
                    }
                    hosoxl.YKienGui = null;

                    await _hoSoRepos.UpdateAsync(hoSo);
                    await _hoSoXuLyRepos.UpdateAsync(hosoxl);

                    if (input.TrangThaiXuLy == (int)CommonENum.KET_QUA_XU_LY.BO_SUNG)
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

                    await _hoSoXuLyHistoryRepos.InsertAndGetIdAsync(_history);
                    #endregion
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