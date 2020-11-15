using System.Linq;
using Abp.Authorization;
using newPSG.PMS.Authorization;
using Abp;

namespace newPSG.PMS.Tenants.Dashboard
{
    [AbpAuthorize(AppPermissions.Pages_Tenant_Dashboard)]
    public class TenantDashboardAppService : PMSAppServiceBase, ITenantDashboardAppService
    {

    }
}