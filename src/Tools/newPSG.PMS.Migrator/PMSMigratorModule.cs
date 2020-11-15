using System.Data.Entity;
using System.Reflection;
using Abp.Events.Bus;
using Abp.Modules;
using Castle.MicroKernel.Registration;
using newPSG.PMS.EntityFramework;

namespace newPSG.PMS.Migrator
{
    [DependsOn(typeof(PMSDataModule))]
    public class PMSMigratorModule : AbpModule
    {
        public override void PreInitialize()
        {
            Database.SetInitializer<PMSDbContext>(null);

            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
            Configuration.ReplaceService(typeof(IEventBus), () =>
            {
                IocManager.IocContainer.Register(
                    Component.For<IEventBus>().Instance(NullEventBus.Instance)
                );
            });
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}