using Abp.Dependency;
using Abp.Runtime.Session;
using Abp.Web.Mvc.Views;

namespace newPSG.PMS.Web.Views
{
    public abstract class PMSWebViewPageBase : PMSWebViewPageBase<dynamic>
    {
       
    }

    public abstract class PMSWebViewPageBase<TModel> : AbpWebViewPage<TModel>
    {
        public IAbpSession AbpSession { get; private set; }
        
        protected PMSWebViewPageBase()
        {
            AbpSession = IocManager.Instance.Resolve<IAbpSession>();
            LocalizationSourceName = PMSConsts.LocalizationSourceName;
        }
    }
}