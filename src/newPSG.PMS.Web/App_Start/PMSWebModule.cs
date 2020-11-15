using Abp;
using Abp.Configuration.Startup;
using Abp.Hangfire;
using Abp.Hangfire.Configuration;
using Abp.IO;
using Abp.Modules;
using Abp.Runtime.Caching.Redis;
using Abp.Web.Mvc;
using Abp.Web.SignalR;
using Abp.Zero.Configuration;
using Castle.MicroKernel.Registration;
using Hangfire;
using Hangfire.Common;
using Microsoft.Owin.Security;
using newPSG.PMS.HangFireScheduler;
using newPSG.PMS.Web.App.Startup;//SPA!
using newPSG.PMS.Web.Bundling;
using newPSG.PMS.Web.Navigation;
using newPSG.PMS.Web.Routing;
using newPSG.PMS.WebApi;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace newPSG.PMS.Web
{
    /// <summary>
    /// Web module of the application.
    /// This is the most top and entrance module that depends on others.
    /// </summary>
    [DependsOn(
        typeof(AbpWebMvcModule),
        typeof(AbpZeroOwinModule),
        typeof(PMSDataModule),
        typeof(PMSApplicationModule),
        typeof(PMSWebApiModule),
        typeof(AbpWebSignalRModule),
        typeof(AbpRedisCacheModule), //AbpRedisCacheModule dependency can be removed if not using Redis cache
        typeof(AbpHangfireModule))] //AbpHangfireModule dependency can be removed if not using Hangfire
    public class PMSWebModule : AbpModule
    {
        public override void PreInitialize()
        {
            //Use database for language management
            Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();

            //Configure navigation/menu
            Configuration.Navigation.Providers.Add<AppNavigationProvider>();//SPA!
            Configuration.Navigation.Providers.Add<FrontEndNavigationProvider>();

            Configuration.Modules.AbpWebCommon().MultiTenancy.DomainFormat = WebUrlService.WebSiteRootAddress;

            //Uncomment these lines to use HangFire as background job manager.
            Configuration.BackgroundJobs.UseHangfire(configuration =>
            {
                configuration.GlobalConfiguration.UseSqlServerStorage("Default");
            });

            //Uncomment this line to use Redis cache instead of in-memory cache.
            //Configuration.Caching.UseRedis();
        }

        public override void Initialize()
        {
            //Dependency Injection
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            IocManager.IocContainer.Register(
                Component
                    .For<IAuthenticationManager>()
                    .UsingFactoryMethod(() => HttpContext.Current.GetOwinContext().Authentication)
                    .LifestyleTransient()
                );

            //Areas
            AreaRegistration.RegisterAllAreas();

            //Routes
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            //Bundling
            BundleTable.Bundles.IgnoreList.Clear();
            CommonBundleConfig.RegisterBundles(BundleTable.Bundles);
            AppBundleConfig.RegisterBundles(BundleTable.Bundles);//SPA!
            FrontEndBundleConfig.RegisterBundles(BundleTable.Bundles);

            AppVar.DefaultConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["Default"].ConnectionString;

            // hangfire 
            var manager = new RecurringJobManager();
            manager.AddOrUpdate("HangFileSchedulerLienThongTuCongBo"
                , Job.FromExpression(() => new HangFileScheduler().HangFileSchedulerLienThongTuCongBo())
                , Cron.MinuteInterval(5));

            manager.AddOrUpdate("HangFileSchedulerLienThongDangKyCongBo"
              , Job.FromExpression(() => new HangFileScheduler().HangFileSchedulerLienThongDangKyCongBo())
              , Cron.MinuteInterval(5));

            manager.AddOrUpdate("HangFileSchedulerLienThongDangKyQuangCao"
              , Job.FromExpression(() => new HangFileScheduler().HangFileSchedulerLienThongDangKyQuangCao())
              , Cron.MinuteInterval(5));

            manager.AddOrUpdate("HangFileSchedulerLienThongCoSoDuDieuKien"
              , Job.FromExpression(() => new HangFileScheduler().HangFileSchedulerLienThongCoSoDuDieuKien())
              , Cron.MinuteInterval(5));

        }

        public override void PostInitialize()
        {
            var server = HttpContext.Current.Server;
            var appFolders = IocManager.Resolve<AppFolders>();

            appFolders.SampleProfileImagesFolder = server.MapPath("~/Common/Images/SampleProfilePics");
            appFolders.TempFileDownloadFolder = server.MapPath("~/Temp/Downloads");
            appFolders.TempFileBaoCaoFolder = server.MapPath("~/Temp/BaoCao");
            appFolders.WebLogsFolder = server.MapPath("~/App_Data/Logs");
            appFolders.PDFTemplate = server.MapPath("~/PDFTemplate");
            try { DirectoryHelper.CreateIfNotExists(appFolders.TempFileDownloadFolder); } catch { }
        }
    }
}