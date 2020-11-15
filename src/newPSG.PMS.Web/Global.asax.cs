using System;
using System.Globalization;
using System.Web;
using Abp.Castle.Logging.Log4Net;
using Abp.Configuration;
using Abp.Localization;
using Abp.Logging;
using Abp.Timing;
using Abp.Extensions;
using Abp.Web;
using Castle.Facilities.Logging;
using System.Web.Mvc;
using System.Configuration;

namespace newPSG.PMS.Web
{
    public class MvcApplication : AbpWebApplication<PMSWebModule>
    {
        protected override void Application_Start(object sender, EventArgs e)
        {
            //Use UTC clock. Remove this to use local time for your application.
            Clock.Provider = ClockProviders.Local;
            //Log4Net configuration
            AbpBootstrapper.IocManager.IocContainer
                .AddFacility<LoggingFacility>(f => f.UseAbpLog4Net()
                    .WithConfig(Server.MapPath("log4net.config"))
                );
            

            base.Application_Start(sender, e);
        }

        protected override void Session_Start(object sender, EventArgs e)
        {
            RestoreUserLanguage();
            base.Session_Start(sender, e);
        }

        

        private void RestoreUserLanguage()
        {
            var settingManager = AbpBootstrapper.IocManager.Resolve<ISettingManager>();
            var defaultLanguage = settingManager.GetSettingValue(LocalizationSettingNames.DefaultLanguage);

            if (defaultLanguage.IsNullOrEmpty())
            {
                return;
            }

            try
            {
                CultureInfo.GetCultureInfo(defaultLanguage);
                Response.Cookies.Add(new HttpCookie("Abp.Localization.CultureName", defaultLanguage) { Expires = Clock.Now.AddYears(2) ,HttpOnly=true});
            }
            catch (CultureNotFoundException exception)
            {
                LogHelper.Logger.Warn(exception.Message, exception);
            }
        }

        /* Preventing client side cache */
        private static readonly DateTime CacheExpireDate = new DateTime(2000, 1, 1);
        protected override void Application_BeginRequest(object sender, EventArgs e)
        {
            MvcHandler.DisableMvcResponseHeader = true;
            base.Application_BeginRequest(sender, e);
            Response.Headers.Remove("X-Powered-By");
            Response.Headers.Remove("X-AspNet-Version");
            Response.Headers.Remove("X-AspNetMvc-Version");
            Response.Headers.Remove("Server");

            var cfg = (System.Web.Configuration.CompilationSection)ConfigurationManager.GetSection("system.web/compilation");
            if (cfg.Debug)
            {
                DisableClientCache();
            }
        }

        private void DisableClientCache()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(CacheExpireDate);
            Response.Cache.SetNoStore();
        }
    }
}
