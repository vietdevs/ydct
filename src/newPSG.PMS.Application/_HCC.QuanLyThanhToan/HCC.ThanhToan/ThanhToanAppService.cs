using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using newPSG.PMS.Common.Dto;
using newPSG.PMS.EntityDB;
using Abp.Runtime.Session;
using Microsoft.AspNet.Identity;
using newPSG.PMS.Authorization.Users;
using newPSG.PMS.DoanhNghiepInput;
using Abp.Collections.Extensions;
using Abp.Linq.Extensions;
using System;
using Abp.Authorization;
using newPSG.PMS.Authorization.Users.Dto;
using newPSG.PMS.Authorization.Roles;
using newPSG.PMS.Authorization;
using Abp.Domain.Uow;
using System.Configuration;
using newPSG.PMS.MultiTenancy;
using Abp.Extensions;
using System.Text.RegularExpressions;
using Abp.Application.Services;
using newPSG.PMS.Dto;
using newPSG.PMS.Web;
using Newtonsoft.Json;
using static newPSG.PMS.CommonENum;

namespace newPSG.PMS.Services
{
    public interface IThanhToanAppService : IApplicationService
    {
        Task<List<ThanhToanDto>> GetListThanhToanByHoSoId(int thuTucId = 0, long hoSoId = 0);
        Task<dynamic> QueryBildToKeypay(KeypayDto input);
        Task<dynamic> QueryBildToKeypayV2(long thanhToanId);
    }

    [AbpAuthorize]
    public class ThanhToanAppService : PMSAppServiceBase, IThanhToanAppService
    {
        private readonly IRepository<DoanhNghiep, long> _doanhNghiepRepos;
        private readonly IRepository<ThanhToan, long> _thanhToanRepos;
        private readonly IAbpSession _session;
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly IRepository<PhongBan> _phongBanRepos;
        private readonly IRepository<User, long> _userRepos;
        private readonly IRepository<Tinh> _tinhRepos;
        private readonly IRepository<Huyen, long> _huyenRepos;
        private readonly IRepository<Xa, long> _xaRepos;
        private readonly UserAppService _userService;
        private readonly TenantManager _tenantManager;

        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IXuLyHoSoDoanhNghiep99AppService _xuLyHoSoDoanhNghiep99AppService;
        #region Thủ Tục hành Chính
        //private readonly IXuLyHoSoDoanhNghiep01AppService _xuLyHoSoDoanhNghiep01AppService;
        //private readonly IXuLyHoSoDoanhNghiep02AppService _xuLyHoSoDoanhNghiep02AppService;
        //private readonly IXuLyHoSoDoanhNghiep03AppService _xuLyHoSoDoanhNghiep03AppService;
        //private readonly IXuLyHoSoDoanhNghiep04AppService _xuLyHoSoDoanhNghiep04AppService;
        //private readonly IXuLyHoSoDoanhNghiep05AppService _xuLyHoSoDoanhNghiep05AppService;
        //private readonly IXuLyHoSoDoanhNghiep06AppService _xuLyHoSoDoanhNghiep06AppService;
        //private readonly IXuLyHoSoDoanhNghiep07AppService _xuLyHoSoDoanhNghiep07AppService;
        //private readonly IXuLyHoSoDoanhNghiep08AppService _xuLyHoSoDoanhNghiep08AppService;
        //private readonly IXuLyHoSoDoanhNghiep09AppService _xuLyHoSoDoanhNghiep09AppService;
        //private readonly IXuLyHoSoDoanhNghiep10AppService _xuLyHoSoDoanhNghiep10AppService;
        #endregion

        private readonly IThanhToanKeyPayAppService _thanhToanKeyPayAppService;

        public ThanhToanAppService(
            IRepository<DoanhNghiep, long> doanhNghiepRepos,
            IRepository<ThanhToan, long> thanhToanRepos,
            IAbpSession session,
            UserManager userManager,
            RoleManager roleManager,
            IRepository<PhongBan> phongBanRepos,
            IRepository<User, long> userRepos,
            IRepository<Tinh> tinhRepos,
            IRepository<Huyen, long> huyenRepos,
            IRepository<Xa, long> xaRepos,
            UserAppService userService,
            TenantManager tenantManager,
            IUnitOfWorkManager unitOfWorkManager,
			IXuLyHoSoDoanhNghiep99AppService xuLyHoSoDoanhNghiep99AppService,
        #region Thủ Tục hành Chính
            //IXuLyHoSoDoanhNghiep01AppService xuLyHoSoDoanhNghiep01AppService,
            //IXuLyHoSoDoanhNghiep02AppService xuLyHoSoDoanhNghiep02AppService,
            //IXuLyHoSoDoanhNghiep03AppService xuLyHoSoDoanhNghiep03AppService,
            //IXuLyHoSoDoanhNghiep04AppService xuLyHoSoDoanhNghiep04AppService,
            //IXuLyHoSoDoanhNghiep05AppService xuLyHoSoDoanhNghiep05AppService,
            //IXuLyHoSoDoanhNghiep06AppService xuLyHoSoDoanhNghiep06AppService,
            //IXuLyHoSoDoanhNghiep07AppService xuLyHoSoDoanhNghiep07AppService,
            //IXuLyHoSoDoanhNghiep08AppService xuLyHoSoDoanhNghiep08AppService,
            //IXuLyHoSoDoanhNghiep09AppService xuLyHoSoDoanhNghiep09AppService,
            //IXuLyHoSoDoanhNghiep10AppService xuLyHoSoDoanhNghiep10AppService,
        #endregion
            IThanhToanKeyPayAppService thanhToanKeyPayAppService
            )
        {
            _doanhNghiepRepos = doanhNghiepRepos;
            _thanhToanRepos = thanhToanRepos;
            _session = session;
            _userManager = userManager;
            _roleManager = roleManager;
            _phongBanRepos = phongBanRepos;
            _userRepos = userRepos;
            _tinhRepos = tinhRepos;
            _huyenRepos = huyenRepos;
            _xaRepos = xaRepos;
            _userService = userService;
            _tenantManager = tenantManager;
            _unitOfWorkManager = unitOfWorkManager;
            _xuLyHoSoDoanhNghiep99AppService = xuLyHoSoDoanhNghiep99AppService;
            #region Thủ Tục hành Chính
            //_xuLyHoSoDoanhNghiep01AppService = xuLyHoSoDoanhNghiep01AppService;
            //_xuLyHoSoDoanhNghiep02AppService = xuLyHoSoDoanhNghiep02AppService;
            //_xuLyHoSoDoanhNghiep03AppService = xuLyHoSoDoanhNghiep03AppService;
            //_xuLyHoSoDoanhNghiep04AppService = xuLyHoSoDoanhNghiep04AppService;
            //_xuLyHoSoDoanhNghiep05AppService = xuLyHoSoDoanhNghiep05AppService;
            //_xuLyHoSoDoanhNghiep06AppService = xuLyHoSoDoanhNghiep06AppService;
            //_xuLyHoSoDoanhNghiep07AppService = xuLyHoSoDoanhNghiep07AppService;
            //_xuLyHoSoDoanhNghiep08AppService = xuLyHoSoDoanhNghiep08AppService;
            //_xuLyHoSoDoanhNghiep09AppService = xuLyHoSoDoanhNghiep09AppService;
            //_xuLyHoSoDoanhNghiep10AppService = xuLyHoSoDoanhNghiep10AppService;
            #endregion
            _thanhToanKeyPayAppService = thanhToanKeyPayAppService;
        }

        public async Task<List<ThanhToanDto>> GetListThanhToanByHoSoId(int thuTucId = 0, long hoSoId = 0)
        {
            try
            {
                var query = (from tt in _thanhToanRepos.GetAll()
                             where tt.PhanHeId == thuTucId && tt.HoSoId == hoSoId
                             select new ThanhToanDto
                             { 
                                 PhanHeId = tt.PhanHeId,
                                 Id = tt.Id,
                                 DoanhNghiepId = tt.DoanhNghiepId,
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
                                 TenantId = tt.TenantId,
                                 KetQuaGiaoDichJson = tt.KetQuaGiaoDichJson,
                                 LyDoHuyThanhToan = tt.LyDoHuyThanhToan,
                                 RequestJson = tt.RequestJson,
                                 ResponseJson = tt.ResponseJson,
                                 ArrHoSoId = tt.ArrHoSoId
                             });

                var listTT = await query
                    .OrderBy(x => x.NgayGiaoDich)
                    .ToListAsync();

                foreach (var item in listTT)
                {
                   
                    if (item.KenhThanhToan.HasValue)
                    {
                        var kenhThanhToan = (CommonENum.KENH_THANH_TOAN)item.KenhThanhToan;
                        item.StrKenhThanhToan = CommonENum.GetEnumDescription(kenhThanhToan);
                    }

                    var trangThai = (CommonENum.TRANG_THAI_GIAO_DICH)item.TrangThaiNganHang;
                    item.StrTrangThaiNganHang = CommonENum.GetEnumDescription(trangThai);

                    item.StrNgayGiaoDich = item.NgayGiaoDich.HasValue ? item.NgayGiaoDich.Value.ToString("yyyyMMdd") : string.Empty;
                }

                return listTT;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public async Task<dynamic> QueryBildToKeypay(KeypayDto input)
        {
            try
            {
                if (input != null && input.TenantId.HasValue && input.ThanhToanId.HasValue)
                {
                    var res = await _thanhToanKeyPayAppService.QueryBillStatusV2_MD5(input.TenantId.Value, input.Merchant_trans_id, input.Good_code, input.Trans_time);
                    var resSplit = res.Split('|');
                    string _statusCode = "";
                    if (resSplit.Length > 2)
                    {
                        _statusCode = resSplit[1];
                        if (_statusCode == CommonENum.KEYPAY_RESPONSE.THANH_CONG)
                        {
                            var maDonHangSplit = input.Good_code.Split('.');
                            long hoSoId = Convert.ToInt64(maDonHangSplit.Last());
                            var maThuTuc = maDonHangSplit.First();

                            //THU_TUC_99
                            if (maThuTuc == CommonENum.GetEnumDescription(THU_TUC_ID.THU_TUC_99))
                            {
                                //await _xuLyHoSoDoanhNghiep99AppService.NopHoSoMoi(hoSoId, true);
                            }
                            #region Thủ Tục hành Chính
                            //THU_TUC_01
                            if (maThuTuc == CommonENum.GetEnumDescription(THU_TUC_ID.THU_TUC_01))
                            {
                                //await _xuLyHoSoDoanhNghiep01AppService.NopHoSoMoi(hoSoId, true);
                            }
                            //THU_TUC_02
                            if (maThuTuc == CommonENum.GetEnumDescription(THU_TUC_ID.THU_TUC_02))
                            {
                                //await _xuLyHoSoDoanhNghiep02AppService.NopHoSoMoi(hoSoId, true);
                            }
                            //THU_TUC_03
                            if (maThuTuc == CommonENum.GetEnumDescription(THU_TUC_ID.THU_TUC_03))
                            {
                                //await _xuLyHoSoDoanhNghiep03AppService.NopHoSoMoi(hoSoId, true);
                            }
                            //THU_TUC_04
                            if (maThuTuc == CommonENum.GetEnumDescription(THU_TUC_ID.THU_TUC_04))
                            {
                                //await _xuLyHoSoDoanhNghiep04AppService.NopHoSoMoi(hoSoId, true);
                            }
                            //THU_TUC_05
                            if (maThuTuc == CommonENum.GetEnumDescription(THU_TUC_ID.THU_TUC_05))
                            {
                                //await _xuLyHoSoDoanhNghiep05AppService.NopHoSoMoi(hoSoId, true);
                            }
                            //THU_TUC_06
                            if (maThuTuc == CommonENum.GetEnumDescription(THU_TUC_ID.THU_TUC_06))
                            {
                                //await _xuLyHoSoDoanhNghiep06AppService.NopHoSoMoi(hoSoId, true);
                            }
                            //THU_TUC_07
                            if (maThuTuc == CommonENum.GetEnumDescription(THU_TUC_ID.THU_TUC_07))
                            {
                                //await _xuLyHoSoDoanhNghiep07AppService.NopHoSoMoi(hoSoId, true);
                            }
                            //THU_TUC_08
                            if (maThuTuc == CommonENum.GetEnumDescription(THU_TUC_ID.THU_TUC_08))
                            {
                                //await _xuLyHoSoDoanhNghiep08AppService.NopHoSoMoi(hoSoId, true);
                            }
                            //THU_TUC_09
                            if (maThuTuc == CommonENum.GetEnumDescription(THU_TUC_ID.THU_TUC_09))
                            {
                                //await _xuLyHoSoDoanhNghiep09AppService.NopHoSoMoi(hoSoId, true);
                            }
                            //THU_TUC_10
                            if (maThuTuc == CommonENum.GetEnumDescription(THU_TUC_ID.THU_TUC_10))
                            {
                                //await _xuLyHoSoDoanhNghiep10AppService.NopHoSoMoi(hoSoId, true);
                            }
                            #endregion
                        }
                    }
                    return new
                    {
                        Status = 1,
                        Data = res,
                        StatusCode = _statusCode
                    };
                }
                return new
                {
                    Status = 0,
                    Error = true
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<dynamic> QueryBildToKeypayV2(long thanhToanId)
        {
            try
            {
                var thanhToan = _thanhToanRepos.Get(thanhToanId);
                if (thanhToan != null && thanhToan.TenantId.HasValue && thanhToan.HoSoId.HasValue && thanhToan.PhanHeId.HasValue)
                {
                    var strNgayGiaoDich = thanhToan.NgayGiaoDich.HasValue ? thanhToan.NgayGiaoDich.Value.ToString("yyyyMMdd") : string.Empty;
                  
                    var res = await _thanhToanKeyPayAppService.QueryBillStatusV2_MD5(thanhToan.TenantId.Value, thanhToan.MaGiaoDich, thanhToan.MaDonHang, strNgayGiaoDich);
                    var resSplit = res.Split('|');
                    if (resSplit.Length > 2)
                    {
                        KeypayDto ketQuaGiaoDich = new KeypayDto
                        {
                            ThanhToanId = thanhToan.Id,
                            TenantId = thanhToan.TenantId.Value,
                            Merchant_trans_id = thanhToan.MaGiaoDich,
                            Trans_time = strNgayGiaoDich,
                            Good_code = thanhToan.MaDonHang,
                            Nest_code = thanhToan.PhiDaNop.HasValue ? thanhToan.PhiDaNop.ToString() : "0"
                        };
                        ketQuaGiaoDich.Response_code = resSplit[1];
                        if (ketQuaGiaoDich.Response_code == "99")
                        {
                            ketQuaGiaoDich.Trans_id = resSplit[2];
                            //THU_TUC_99
                            if (thanhToan.PhanHeId == (int)THU_TUC_ID.THU_TUC_99)
                            {
                                await _xuLyHoSoDoanhNghiep99AppService.NopHoSoMoi(thanhToan.HoSoId.Value, true);
                            }
                            #region Thủ Tục hành Chính
                            //THU_TUC_01
                            //else if (maThuTuc == CommonENum.GetEnumDescription(THU_TUC_ID.THU_TUC_1))
                            //{
                            //    await _xuLyHoSoDoanhNghiep01AppService.DoanhNghiepTuCongBo(hoSoId, true);
                            //}
                            //THU_TUC_02
                            else if (thanhToan.PhanHeId == (int)THU_TUC_ID.THU_TUC_02)
                            {
                                //await _xuLyHoSoDoanhNghiep02AppService.NopHoSoMoi(hoso);
                            }
                            //THU_TUC_03
                            else if (thanhToan.PhanHeId == (int)THU_TUC_ID.THU_TUC_03)
                            {
                                //await _xuLyHoSoDoanhNghiep03AppService.NopHoSoMoi(hoso);
                            }
                            //THU_TUC_04
                            else if (thanhToan.PhanHeId == (int)THU_TUC_ID.THU_TUC_04)
                            {
                                //await _xuLyHoSoDoanhNghiep04AppService.ChuyenHoSoSangThamDinh(hoso);
                            }
                            #endregion
                            thanhToan.TrangThaiNganHang = (int)CommonENum.TRANG_THAI_GIAO_DICH.GIAO_DICH_THANH_CONG;
                            thanhToan.KetQuaGiaoDichJson = JsonConvert.SerializeObject(ketQuaGiaoDich);
                            _thanhToanRepos.Update(thanhToan);
                        }

                        return new
                        {
                            Status = 1,
                            Data = res,
                            StatusCode = ketQuaGiaoDich.Response_code,
                            MsgReturn = ketQuaGiaoDich.genMsgReturn(ketQuaGiaoDich.Response_code)
                        };
                    }
                }
                return new
                {
                    Status = 0,
                    Error = true
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
