using System.Threading.Tasks;
using Abp.Application.Services;
using newPSG.PMS.Configuration.Host.Dto;

namespace newPSG.PMS.Configuration.Host
{
    public interface IHostSettingsAppService : IApplicationService
    {
        Task<HostSettingsEditDto> GetAllSettings();

        Task UpdateAllSettings(HostSettingsEditDto input);

        Task SendTestEmail(SendTestEmailInput input);
    }
}
