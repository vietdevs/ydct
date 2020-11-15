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
using newPSG.PMS.Configuration;
using newPSG.PMS.KeypayServices;
using newPSG.PMS.vn.keypay.webservices;

namespace newPSG.PMS.Services
{
    public interface IThanhToanKeyPayAppService : IApplicationService
    {
        Task<string> GetMERCHANT_TRANS_ID_FromSettings();
        Task<string> GetMerchantSecure_Hash_MD5_For_TennantId(int tenantId, Keypay keypay);
        Task<Keypay> GetUrlSendToKeypayMD5(int tenantId, string maGiaoDich, string maDonHang, long amount, string description);
        Task<string> QueryBillStatusV2_MD5(int TenantId, string Merchant_trans_id, string Good_code, string Trans_time);
        //Task<string> ConfirmTransResult(int TenantId, string merchant_trans_id, string good_code, string trans_id, string trans_result);
    }

    [AbpAuthorize]
    public class ThanhToanKeyPayAppService : PMSAppServiceBase, IThanhToanKeyPayAppService
    {
        private readonly IRepository<DoanhNghiep, long> _doanhNghiepRepos;
        private readonly IRepository<LoaiHinhDoanhNghiep> _loaiHinhRepos;
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

        public ThanhToanKeyPayAppService(
            IRepository<DoanhNghiep, long> doanhNghiepRepos,
            IRepository<LoaiHinhDoanhNghiep> loaiHinhRepos,
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
            IUnitOfWorkManager unitOfWorkManager
            )
        {
            _doanhNghiepRepos = doanhNghiepRepos;
            _loaiHinhRepos = loaiHinhRepos;
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
        }
        public async Task<string> GetMERCHANT_TRANS_ID_FromSettings()
        {
            string maGiaoDichNew = "1";
            try
            {

                long ret = 1;
                string MERCHANT_TRANS_ID_MAX = await SettingManager.GetSettingValueForApplicationAsync(AppSettings.Payment.KEYPAY_MERCHANT_TRANS_ID_MAX);
                if (!string.IsNullOrEmpty(MERCHANT_TRANS_ID_MAX))
                {
                    ret = Convert.ToInt64(MERCHANT_TRANS_ID_MAX);
                    //maGiaoDich = ret.ToString().PadLeft(6, '0');
                    maGiaoDichNew = ret.ToString();
                    ret++;
                    await SettingManager.ChangeSettingForApplicationAsync(AppSettings.Payment.KEYPAY_MERCHANT_TRANS_ID_MAX, ret.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return maGiaoDichNew;
        }
        public async Task<string> GetMerchantSecure_Hash_MD5_For_TennantId(int tenantId, Keypay keypay)
        {
            var hash_MD5 = "";
            try
            {

                var KEYPAY_MERCHANT_KEY = await SettingManager.GetSettingValueForTenantAsync(AppSettings.Payment.KEYPAY_MERCHANT_KEY, tenantId);
                var KEYPAY_MERCHANT_CODE = await SettingManager.GetSettingValueForTenantAsync(AppSettings.Payment.KEYPAY_MERCHANT_CODE, tenantId);

                //xử lý so sánh chuối mã hóa
                keypay.Merchant_secure_key = KEYPAY_MERCHANT_KEY;
                keypay.Merchant_code = KEYPAY_MERCHANT_CODE;

                hash_MD5 = keypay.get_MerchantSecure_Hash_MD5();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return hash_MD5;
        }
        public async Task<Keypay> GetUrlSendToKeypayMD5(int tenantId, string maGiaoDich, string maDonHang, long amount, string description)
        {
            var KEYPAY_MERCHANT_KEY = await SettingManager.GetSettingValueForTenantAsync(AppSettings.Payment.KEYPAY_MERCHANT_KEY, tenantId);
            var KEYPAY_MERCHANT_CODE = await SettingManager.GetSettingValueForTenantAsync(AppSettings.Payment.KEYPAY_MERCHANT_CODE, tenantId);
            if (string.IsNullOrEmpty(KEYPAY_MERCHANT_KEY) || string.IsNullOrEmpty(KEYPAY_MERCHANT_CODE))
            {
                return null;
            }

            Keypay pay = new Keypay();
            pay.Keypayurl = Utility.KeyPaySetting.KPAY_URL();
            pay.Bank_code = string.Empty;
            pay.Command = Utility.KeyPaySetting.COMMAND_TYPE_PAY;
            pay.Country_Code = Utility.KeyPaySetting.COUNTRY_CODE;
            pay.Current_code = Utility.KeyPaySetting.CURRENT_CODE;
            pay.Current_local = Utility.KeyPaySetting.CURRENT_LOCAL;
            pay.Desc1 = string.Empty;
            pay.Desc2 = string.Empty;
            pay.Desc3 = string.Empty;
            pay.Desc4 = string.Empty;
            pay.Desc5 = string.Empty;
            pay.Internal_bank = Utility.KeyPaySetting.INTERNAL_BANK;
            pay.Return_url = Utility.KeyPaySetting.RETURN_URL();
            pay.Service_code = Utility.KeyPaySetting.SERVICE_CODE_MUA_HANG;
            pay.Ship_fee = "0";
            pay.Tax = "0";
            pay.Version = Utility.KeyPaySetting.VERSION;

            #region Parameter payment
            pay.Nest_code = amount.ToString();
            pay.Merchant_trans_id = maGiaoDich;
            pay.Xdescription = description;
            pay.Good_code = maDonHang;
            #endregion

            pay.Merchant_code = KEYPAY_MERCHANT_CODE;
            pay.Merchant_secure_key = KEYPAY_MERCHANT_KEY;
            pay.Secure_hash = pay.get_Secure_Hash_MD5();
            return pay;
        }
        public async Task<string> QueryBillStatusV2_MD5(int TenantId, string Merchant_trans_id, string Good_code, string Trans_time)
        {
            var KEYPAY_MERCHANT_KEY = await SettingManager.GetSettingValueForTenantAsync(AppSettings.Payment.KEYPAY_MERCHANT_KEY, TenantId);
            var KEYPAY_MERCHANT_CODE = await SettingManager.GetSettingValueForTenantAsync(AppSettings.Payment.KEYPAY_MERCHANT_CODE, TenantId);
            string _keyHash = KEYPAY_MERCHANT_CODE + Merchant_trans_id + Good_code + Trans_time + KEYPAY_MERCHANT_KEY;
            Secure_Hash_MD5 md5 = new Secure_Hash_MD5();
            string _Secure_hash = md5.GetMD5Hash(_keyHash);

            //Call service
            kpWebservices webservice = new kpWebservices();
            return webservice.QuerryBillStatusV2(KEYPAY_MERCHANT_CODE, Merchant_trans_id, Good_code, Trans_time, _Secure_hash);
        }

        /// <summary>
        /// Comfirm trạng thái giao dịch
        /// </summary>
        /// <param name="TenantId"></param>
        /// <param name="merchant_trans_id">Mã giao dịch</param>
        /// <param name="good_code">Mã đơn hàng</param>
        /// <param name="trans_id">Mã Giao dịch của Keypay</param>
        /// <param name="trans_result">Trạng thái xử lý 0: Thành công| 1 Thất bại</param>
        /// <returns>
        /// Thành công : yyy|00 |MD5(yyy + 00 + Merchant_secure_key)
        /// Thất bại : 011|xx |MD5(011 + xx + Merchant_secure_key)
        /// </returns>
        public async Task<string> ConfirmTransResult(int TenantId, string merchant_trans_id, string good_code, string trans_id, string trans_result)
        {
            var KEYPAY_MERCHANT_KEY = await SettingManager.GetSettingValueForTenantAsync(AppSettings.Payment.KEYPAY_MERCHANT_KEY, TenantId);
            var KEYPAY_MERCHANT_CODE = await SettingManager.GetSettingValueForTenantAsync(AppSettings.Payment.KEYPAY_MERCHANT_CODE, TenantId);
            string _keyHash = KEYPAY_MERCHANT_CODE + good_code + merchant_trans_id + trans_id + trans_result + KEYPAY_MERCHANT_KEY;
            Secure_Hash_MD5 md5 = new Secure_Hash_MD5();
            string _Secure_hash = md5.GetMD5Hash(_keyHash);

            //Call service
            kpWebservices webservice = new kpWebservices();
            return webservice.ConfirmTransResult(KEYPAY_MERCHANT_CODE, good_code, merchant_trans_id,trans_id,trans_result, _Secure_hash);
        }
    }
}
