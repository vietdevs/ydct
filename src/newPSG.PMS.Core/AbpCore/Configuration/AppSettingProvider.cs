using System.Collections.Generic;
using System.Configuration;
using Abp.Configuration;
using Abp.Json;
using Abp.Zero.Configuration;
using newPSG.PMS.Security;

namespace newPSG.PMS.Configuration
{
    /// <summary>
    /// Defines settings for the application.
    /// See <see cref="AppSettings"/> for setting names.
    /// </summary>
    public class AppSettingProvider : SettingProvider
    {
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            context.Manager.GetSettingDefinition(AbpZeroSettingNames.UserManagement.TwoFactorLogin.IsEnabled).DefaultValue = false.ToString().ToLowerInvariant();

            var defaultPasswordComplexitySetting = new PasswordComplexitySetting
            {
                MinLength = 6,
                MaxLength = 10,
                UseNumbers = true,
                UseUpperCaseLetters = false,
                UseLowerCaseLetters = true,
                UsePunctuations = false,
            };

            return new[]
                   {
                       //Host settings
                        new SettingDefinition(AppSettings.TenantManagement.AllowSelfRegistration,ConfigurationManager.AppSettings[AppSettings.TenantManagement.UseCaptchaOnRegistration] ?? "true"),
                        new SettingDefinition(AppSettings.TenantManagement.IsNewRegisteredTenantActiveByDefault,ConfigurationManager.AppSettings[AppSettings.TenantManagement.IsNewRegisteredTenantActiveByDefault] ??"false"),
                        new SettingDefinition(AppSettings.TenantManagement.UseCaptchaOnRegistration,ConfigurationManager.AppSettings[AppSettings.TenantManagement.UseCaptchaOnRegistration] ?? "true"),
                        new SettingDefinition(AppSettings.TenantManagement.DefaultEdition,ConfigurationManager.AppSettings[AppSettings.TenantManagement.DefaultEdition] ?? ""),
                        new SettingDefinition(AppSettings.Security.PWComplexity, defaultPasswordComplexitySetting.ToJsonString(),scopes: SettingScopes.Application | SettingScopes.Tenant),

                        //Tenant settings
                        new SettingDefinition(AppSettings.UserManagement.AllowSelfRegistration, ConfigurationManager.AppSettings[AppSettings.UserManagement.AllowSelfRegistration] ?? "true", scopes: SettingScopes.Tenant),
                        new SettingDefinition(AppSettings.UserManagement.IsNewRegisteredUserActiveByDefault, ConfigurationManager.AppSettings[AppSettings.UserManagement.IsNewRegisteredUserActiveByDefault] ?? "false", scopes: SettingScopes.Tenant),
                        new SettingDefinition(AppSettings.UserManagement.UseCaptchaOnRegistration, ConfigurationManager.AppSettings[AppSettings.UserManagement.UseCaptchaOnRegistration] ?? "true", scopes: SettingScopes.Tenant),
                        //Payment
                        new SettingDefinition(AppSettings.Payment.KEYPAY_MERCHANT_KEY, ConfigurationManager.AppSettings[AppSettings.Payment.KEYPAY_MERCHANT_KEY] ?? "", scopes: SettingScopes.Tenant),
                        new SettingDefinition(AppSettings.Payment.KEYPAY_MERCHANT_CODE, ConfigurationManager.AppSettings[AppSettings.Payment.KEYPAY_MERCHANT_CODE] ?? "", scopes: SettingScopes.Tenant),
                        new SettingDefinition(AppSettings.Payment.KEYPAY_MERCHANT_TRANS_ID_MAX, ConfigurationManager.AppSettings[AppSettings.Payment.KEYPAY_MERCHANT_TRANS_ID_MAX] ?? "1", scopes: SettingScopes.Application),

                        //LienThong
                        new SettingDefinition(AppSettings.LienThongInfo.DOMAIN_LIEN_THONG, ConfigurationManager.AppSettings[AppSettings.LienThongInfo.DOMAIN_LIEN_THONG] ?? "", scopes: SettingScopes.Application),
                        new SettingDefinition(AppSettings.LienThongInfo.TENANT_LIEN_THONG, ConfigurationManager.AppSettings[AppSettings.LienThongInfo.TENANT_LIEN_THONG] ?? "", scopes: SettingScopes.Application),
                        new SettingDefinition(AppSettings.LienThongInfo.USER_LIEN_THONG, ConfigurationManager.AppSettings[AppSettings.LienThongInfo.USER_LIEN_THONG] ?? "", scopes: SettingScopes.Tenant),
                        new SettingDefinition(AppSettings.LienThongInfo.PASS_LIEN_THONG, ConfigurationManager.AppSettings[AppSettings.LienThongInfo.PASS_LIEN_THONG] ?? "", scopes: SettingScopes.Tenant),
                        new SettingDefinition(AppSettings.LienThongInfo.TOKEN_LIEN_THONG, ConfigurationManager.AppSettings[AppSettings.LienThongInfo.TOKEN_LIEN_THONG] ?? "", scopes: SettingScopes.Tenant)
                   };
        }
    }
}
