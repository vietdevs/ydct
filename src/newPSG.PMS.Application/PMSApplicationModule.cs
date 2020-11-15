using System.Reflection;
using Abp.AutoMapper;
using Abp.Modules;
using newPSG.PMS.Authorization;

namespace newPSG.PMS
{
    /// <summary>
    /// Application layer module of the application.
    /// </summary>
    [DependsOn(typeof(PMSCoreModule))]
    public class PMSApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            //Adding authorization providers
            Configuration.Authorization.Providers.Add<AppAuthorizationProvider>();

            //Adding custom AutoMapper mappings
            Configuration.Modules.AbpAutoMapper().Configurators.Add(mapper =>
            {
                CustomDtoMapper.CreateMappings(mapper);
            });
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
