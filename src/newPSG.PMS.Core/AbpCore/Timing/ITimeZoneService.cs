using System.Threading.Tasks;
using Abp.Configuration;

namespace newPSG.PMS.Timing
{
    public interface ITimeZoneService
    {
        Task<string> GetDefaultTimezoneAsync(SettingScopes scope, int? tenantId);
    }
}
