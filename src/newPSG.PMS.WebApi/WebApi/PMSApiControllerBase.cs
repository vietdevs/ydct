using Abp.WebApi.Controllers;

namespace newPSG.PMS.WebApi
{
    public abstract class PMSApiControllerBase : AbpApiController
    {
        protected PMSApiControllerBase()
        {
            LocalizationSourceName = PMSConsts.LocalizationSourceName;
        }
    }
}