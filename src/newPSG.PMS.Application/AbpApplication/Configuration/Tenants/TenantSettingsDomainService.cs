using System.Globalization;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Extensions;
using newPSG.PMS.Configuration.Tenants.Dto;
using System;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.Runtime.Security;

namespace newPSG.PMS.Configuration.Tenants
{
    public interface ITenantSettingsDomainService : IDomainService
    {
        Task<LienThongInfoSettingDto> GetLienThongInfoSettingsAsync(int tenanId);

        bool IsAccountLienThong(int tenanId);
    }

    public class TenantSettingsDomainService : PMSAppServiceBase, ITenantSettingsDomainService
    {
        public TenantSettingsDomainService()
        {

        }

        [UnitOfWork]
        public async Task<LienThongInfoSettingDto> GetLienThongInfoSettingsAsync(int tenanId)
        {
            try
            {
                var _DomainLienThong = await SettingManager.GetSettingValueForApplicationAsync(AppSettings.LienThongInfo.DOMAIN_LIEN_THONG);
                var _TenantLienThong = await SettingManager.GetSettingValueForApplicationAsync(AppSettings.LienThongInfo.TENANT_LIEN_THONG);
                var _UserLienThong = await SettingManager.GetSettingValueForTenantAsync(AppSettings.LienThongInfo.USER_LIEN_THONG, tenanId);
                var _PassLienThongMaHoa = await SettingManager.GetSettingValueForTenantAsync(AppSettings.LienThongInfo.PASS_LIEN_THONG, tenanId);
                var _PassLienThong = SimpleStringCipher.Instance.Decrypt(_PassLienThongMaHoa);
                //var _TokenLienThong = await SettingManager.GetSettingValueForTenantAsync(AppSettings.LienThongInfo.TOKEN_LIEN_THONG, tenanId);

                var settings = new LienThongInfoSettingDto
                {
                    DomainLienThong = _DomainLienThong,
                    TenantLienThong = _TenantLienThong,
                    UserLienThong = _UserLienThong,
                    PassLienThong = _PassLienThong,
                    //TokenLienThong = _TokenLienThong
                };

                return settings;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return null;
            }
        }

        [UnitOfWork]
        public bool IsAccountLienThong(int tenanId)
        {
            try
            {
                bool isAccount = false;
                string _UserLienThong = SettingManager.GetSettingValueForTenant(AppSettings.LienThongInfo.USER_LIEN_THONG, tenanId);
                string _PassLienThongMaHoa = SettingManager.GetSettingValueForTenant(AppSettings.LienThongInfo.PASS_LIEN_THONG, tenanId);

                if (!string.IsNullOrEmpty(_UserLienThong) && !string.IsNullOrEmpty(_PassLienThongMaHoa))
                {
                    isAccount = true;
                }

                return isAccount;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return false;
            }
        }
    }
}