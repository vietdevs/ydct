using Abp.Zero.Ldap.Authentication;
using Abp.Zero.Ldap.Configuration;
using newPSG.PMS.Authorization.Users;
using newPSG.PMS.MultiTenancy;

namespace newPSG.PMS.Authorization.Ldap
{
    public class AppLdapAuthenticationSource : LdapAuthenticationSource<Tenant, User>
    {
        public AppLdapAuthenticationSource(ILdapSettings settings, IAbpZeroLdapModuleConfig ldapModuleConfig)
            : base(settings, ldapModuleConfig)
        {
        }
    }
}
