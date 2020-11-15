using Abp.Authorization;
using newPSG.PMS.Authorization.Roles;
using newPSG.PMS.Authorization.Users;
using newPSG.PMS.MultiTenancy;

namespace newPSG.PMS.Authorization
{
    /// <summary>
    /// Implements <see cref="PermissionChecker"/>.
    /// </summary>
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {

        }
    }
}
