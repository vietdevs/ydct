using System.Threading.Tasks;
using Abp.Application.Services;
using newPSG.PMS.Configuration.Tenants.Dto;

namespace newPSG.PMS.Configuration.Tenants
{
    public interface ITenantSettingsAppService : IApplicationService
    {
        Task<TenantSettingsEditDto> GetAllSettings();

        Task UpdateAllSettings(TenantSettingsEditDto input);

        Task ClearLogo();

        Task ClearCustomCss();
    }
}
