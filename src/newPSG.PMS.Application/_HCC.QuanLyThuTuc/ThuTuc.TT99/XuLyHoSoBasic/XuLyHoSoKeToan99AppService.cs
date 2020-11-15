using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using newPSG.PMS.Authorization.Users;
using newPSG.PMS.Dto;
using newPSG.PMS.Editions;
using newPSG.PMS.EntityDB;
using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using newPSG.PMS.MultiTenancy;
using System.Data.Entity;
using Abp.Linq.Extensions;

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
using newPSG.PMS.Common;
#endregion

namespace newPSG.PMS.Services
{
    #region INTERFACE
    public interface IXuLyHoSoKeToan99AppService : IApplicationService
    {
        #region Xác nhận thanh toán
        Task<PagedResultDto<ThanhToanDto>> GetListThanhToanChuyenKhoanPaging(ThanhToanChuyenKhoanKeToan99InputDto input);
        Task<dynamic> LoadXacNhanThanhToan(LoadXacNhanKeToan99InputDto input);
        Task<int> KeToanDuyet_Chuyen(LuuXacNhanKeToan99InputDto input);
        #endregion
    }
    #endregion

    #region MAIN
    [AbpAuthorize]
    public class XuLyHoSoKeToan99AppService : PMSAppServiceBase, IXuLyHoSoKeToan99AppService
    {
        private readonly IRepository<DoanhNghiep, long> _doanhNghiepRepos;
        private readonly IRepository<XHoSo, long> _hoSoRepos;
        private readonly IRepository<XHoSoXuLyHistory, long> _hoSoXuLyHistoryRepos;
        private readonly IRepository<LoaiHoSo> _loaiHoSoRepos;
        private readonly IRepository<ThanhToan, long> _thanhToanRepos;
        private readonly IAbpSession _session;

        private readonly ICustomTennantAppService _customTennantAppService;
        private readonly IXuLyHoSoDoanhNghiep99AppService _xuLyHoSoDoanhNghiepAppService;

        public XuLyHoSoKeToan99AppService(IRepository<DoanhNghiep, long> doanhNghiepRepos,
                                          IRepository<XHoSo, long> hoSoRepos,
                                          IRepository<XHoSoXuLyHistory, long> hoSoXuLyHistoryRepos,
                                          IRepository<LoaiHoSo> loaiHoSoRepos,
                                          IRepository<ThanhToan, long> thanhToanRepos,
                                          IAbpSession session,

                                          ICustomTennantAppService customTennantAppService,
                                          IXuLyHoSoDoanhNghiep99AppService xuLyHoSoDoanhNghiepAppService)
        {
            _doanhNghiepRepos = doanhNghiepRepos;
            _hoSoRepos = hoSoRepos;
            _hoSoXuLyHistoryRepos = hoSoXuLyHistoryRepos;
            _loaiHoSoRepos = loaiHoSoRepos;
            _thanhToanRepos = thanhToanRepos;
            _session = session;

            _customTennantAppService = customTennantAppService;
            _xuLyHoSoDoanhNghiepAppService = xuLyHoSoDoanhNghiepAppService;
        }

        #region Xác nhận thanh toán
        public async Task<PagedResultDto<ThanhToanDto>> GetListThanhToanChuyenKhoanPaging(ThanhToanChuyenKhoanKeToan99InputDto input)
        {
            try
            {
                var query = (from hoso in _hoSoRepos.GetAll()
                             join tt in _thanhToanRepos.GetAll() on hoso.ThanhToanId_Active equals tt.Id
                             join dnds in _doanhNghiepRepos.GetAll() on hoso.DoanhNghiepId equals dnds.Id into dntb
                             from dn in dntb.DefaultIfEmpty()
                             where (hoso.PId == null && tt.KenhThanhToan == (int)CommonENum.KENH_THANH_TOAN.HINH_THUC_CHUYEN_KHOAN)
                             && (string.IsNullOrEmpty(input.Filter) || hoso.MaHoSo.LocDauLowerCaseDB().Contains(input.Filter.LocDauLowerCaseDB()) || hoso.SoDangKy.Contains(input.Filter.LocDauLowerCaseDB())
                                 || hoso.TenDoanhNghiep.LocDauLowerCaseDB().Contains(input.Filter.LocDauLowerCaseDB()) || tt.MaGiaoDich.Contains(input.Filter.LocDauLowerCaseDB()) || tt.MaDonHang.Contains(input.Filter.LocDauLowerCaseDB()))
                             && (!input.LoaiHoSoId.HasValue || hoso.LoaiHoSoId == input.LoaiHoSoId.Value)
                             && (!input.TinhId.HasValue || hoso.TinhId == input.TinhId.Value)
                             select new ThanhToanDto
                             {
                                 Id = hoso.Id,
                                 DoanhNghiepId = tt.DoanhNghiepId,
                                 MaHoSo = hoso.MaHoSo,
                                 SoDangKy = hoso.SoDangKy,
                                 LoaiHoSoId = hoso.LoaiHoSoId,
                                 StrLoaiHoSo = hoso.TenLoaiHoSo,
                                 HoSoId = tt.HoSoId,
                                 GhiChu = tt.GhiChu,
                                 KenhThanhToan = tt.KenhThanhToan,
                                 PhiDaNop = tt.PhiDaNop,
                                 PhiXacNhan = tt.PhiXacNhan,
                                 SoTaiKhoanNop = tt.SoTaiKhoanNop,
                                 SoTaiKhoanHuong = tt.SoTaiKhoanHuong,
                                 MaGiaoDich = tt.MaGiaoDich,
                                 MaDonHang = tt.MaDonHang,
                                 NgayGiaoDich = tt.NgayGiaoDich,
                                 TrangThaiKeToan = tt.TrangThaiKeToan,
                                 TrangThaiNganHang = tt.TrangThaiNganHang,
                                 TenDoanhNghiep = dn.TenDoanhNghiep,
                                 TenantId = tt.TenantId,
                                 IsChiCuc = hoso.IsChiCuc,
                                 ChiCucId = hoso.ChiCucId,
                                 TrangThaiHoSo = hoso.TrangThaiHoSo,
                                 TinhId = (dn != null) ? dn.TinhId : 0,
                                 StrTinh = (dn != null) ? dn.Tinh : string.Empty,
                                 ThanhToanId_Active = hoso.ThanhToanId_Active
                             });

                if (input.FormCase == (int)CommonENum.FORM_CASE_XAC_NHAN_THANH_TOAN.HO_SO_CHO_XAC_NHAN_THANH_TOAN)
                {
                    query = query.Where(
                       p => ((p.TrangThaiHoSo != null ? p.TrangThaiHoSo.Value : 0) == (int)CommonENum.TRANG_THAI_HO_SO.CHO_KE_TOAN_XAC_NHAN) 
                       && p.TrangThaiKeToan == (int)CommonENum.TRANG_THAI_KE_TOAN.KE_TOAN_CHUA_XAC_NHAN
                   );
                }
                else if (input.FormCase == (int)CommonENum.FORM_CASE_XAC_NHAN_THANH_TOAN.HO_SO_DA_XAC_NHAN_THANH_TOAN_THANH_CONG)
                {
                    query = query.Where(
                       p => p.TrangThaiKeToan == (int)CommonENum.TRANG_THAI_KE_TOAN.KE_TOAN_XAC_NHAN_THANH_CONG
                   );
                }
                else if (input.FormCase == (int)CommonENum.FORM_CASE_XAC_NHAN_THANH_TOAN.HO_SO_DA_XAC_NHAN_THANH_TOAN_THAT_BAI)
                {
                    query = query.Where(
                       p => ((p.TrangThaiHoSo != null ? p.TrangThaiHoSo.Value : 0) == (int)CommonENum.TRANG_THAI_HO_SO.THANH_TOAN_THAT_BAI) 
                       && p.TrangThaiKeToan == (int)CommonENum.TRANG_THAI_KE_TOAN.KE_TOAN_XAC_NHAN_KHONG_THANH_CONG
                   );
                }

                var _total = await query.CountAsync();
                var dataGrids = await query
                    .OrderBy(x => x.NgayGiaoDich)
                    .PageBy(input)
                   .ToListAsync();

                return new PagedResultDto<ThanhToanDto>(_total, dataGrids);
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                return null;
            }
        }

        public async Task<dynamic> LoadXacNhanThanhToan(LoadXacNhanKeToan99InputDto input)
        {
            try
            {
                var thanhtoan = await _thanhToanRepos.FirstOrDefaultAsync(input.ThanhToanId);
                if (thanhtoan != null && thanhtoan.Id > 0)
                {

                    var res = new
                    {
                        thanhToan = thanhtoan
                    };
                    return res;
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
            }
            return null;
        }

        public async Task<int> KeToanDuyet_Chuyen(LuuXacNhanKeToan99InputDto input)
        {
            try
            {
                if (input.HoSoId > 0 && input.ThanhToanId > 0)
                {
                    var thanhtoan = await _thanhToanRepos.FirstOrDefaultAsync(input.ThanhToanId);
                    if (thanhtoan != null)
                    {
                        if (input.XacNhanThanhToan == 1)
                        {
                            thanhtoan.TrangThaiNganHang = (int)CommonENum.TRANG_THAI_GIAO_DICH.GIAO_DICH_THANH_CONG;
                            thanhtoan.TrangThaiKeToan = (int)CommonENum.TRANG_THAI_KE_TOAN.KE_TOAN_XAC_NHAN_THANH_CONG;
                            thanhtoan.NgayXacNhanThanhToan = DateTime.Now;
                            thanhtoan.PhiXacNhan = input.PhiXacNhan;
                            await _thanhToanRepos.UpdateAsync(thanhtoan);
                            var hoso = _hoSoRepos.Get(input.HoSoId);
                            if (hoso != null)
                            {
                                hoso.TrangThaiHoSo = (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_HO_SO_MOI;
                                await _hoSoRepos.UpdateAsync(hoso);
                                await _xuLyHoSoDoanhNghiepAppService.NopHoSoMoi(hoso, false, true);
                            }
                        }
                        else
                        {
                            var hoso = _hoSoRepos.Get(input.HoSoId);
                            if (hoso == null)
                            {
                                return 0;
                            }

                            hoso.TrangThaiHoSo = (int)CommonENum.TRANG_THAI_HO_SO.THANH_TOAN_THAT_BAI;
                            await _hoSoRepos.UpdateAsync(hoso);

                            thanhtoan.LyDoHuyThanhToan = input.YKienXacNhan;
                            thanhtoan.TrangThaiNganHang = (int)CommonENum.TRANG_THAI_GIAO_DICH.GIAO_DICH_KHONG_THANH_CONG;
                            thanhtoan.TrangThaiKeToan = (int)CommonENum.TRANG_THAI_KE_TOAN.KE_TOAN_XAC_NHAN_KHONG_THANH_CONG;
                            await _thanhToanRepos.UpdateAsync(thanhtoan);

                            #region Lưu lịch sử
                            var _history = new XHoSoXuLyHistory();
                            _history.ThuTucId = hoso.ThuTucId;
                            _history.HoSoId = input.HoSoId;
                            _history.HoSoXuLyId = 0;
                            _history.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.KE_TOAN;
                            _history.NoiDungYKien = input.YKienXacNhan;
                            _history.NguoiXuLyId = _session.UserId;
                            _history.ActionEnum = (int)CommonENum.FORM_FUNCTION.KE_TOAN_XAC_NHAN_THANH_TOAN;
                            await _hoSoXuLyHistoryRepos.InsertOrUpdateAndGetIdAsync(_history);
                            #endregion
                        }
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
    }
    #endregion
}