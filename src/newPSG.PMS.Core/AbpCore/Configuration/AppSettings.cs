namespace newPSG.PMS.Configuration
{
    /// <summary>
    /// Defines string constants for setting names in the application.
    /// See <see cref="AppSettingProvider"/> for setting definitions.
    /// </summary>
    public static class AppSettings
    {
        public static class General
        {
            //no setting yet
        }

        public static class TenantManagement
        {
            public const string AllowSelfRegistration = "App.TenantManagement.AllowSelfRegistration";
            public const string IsNewRegisteredTenantActiveByDefault = "App.TenantManagement.IsNewRegisteredTenantActiveByDefault";
            public const string UseCaptchaOnRegistration = "App.TenantManagement.UseCaptchaOnRegistration";
            public const string DefaultEdition = "App.TenantManagement.DefaultEdition";
        }

        public static class UserManagement
        {
            public const string AllowSelfRegistration = "App.UserManagement.AllowSelfRegistration";
            public const string IsNewRegisteredUserActiveByDefault = "App.UserManagement.IsNewRegisteredUserActiveByDefault";
            public const string UseCaptchaOnRegistration = "App.UserManagement.UseCaptchaOnRegistration";
        }

        public static class Security
        {
            public const string PWComplexity = "App.Security.PasswordComplexity";
        }

        public static class Payment
        {
            public const string KEYPAY_MERCHANT_CODE = "App.Payment.KEYPAY_MERCHANT_CODE";
            public const string KEYPAY_MERCHANT_KEY = "App.Payment.KPAY_MERCHANT_KEY";
            public const string KEYPAY_MERCHANT_TRANS_ID_MAX = "App.Payment.KEYPAY_MERCHANT_TRANS_ID_MAX";
        }

        public static class LienThongInfo
        {
            public const string DOMAIN_LIEN_THONG = "App.LienThong.DOMAIN_LIEN_THONG";
            public const string TENANT_LIEN_THONG = "App.LienThong.TENANT_LIEN_THONG";
            public const string USER_LIEN_THONG = "App.LienThong.USER_LIEN_THONG";
            public const string PASS_LIEN_THONG = "App.LienThong.PASS_LIEN_THONG";
            public const string TOKEN_LIEN_THONG = "App.LienThong.TOKEN_LIEN_THONG";
        }
    }
}