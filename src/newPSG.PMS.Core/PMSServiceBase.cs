using Abp;

namespace newPSG.PMS
{
    /// <summary>
    /// This class can be used as a base class for services in this application.
    /// It has some useful objects property-injected and has some basic methods most of services may need to.
    /// It's suitable for non domain nor application service classes.
    /// For domain services inherit <see cref="PMSDomainServiceBase"/>.
    /// For application services inherit PMSAppServiceBase.
    /// </summary>
    public abstract class PMSServiceBase : AbpServiceBase
    {
        protected PMSServiceBase()
        {
            LocalizationSourceName = PMSConsts.LocalizationSourceName;
        }
    }
}