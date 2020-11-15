using System.Threading.Tasks;
using Abp.Application.Services;
using newPSG.PMS.Sessions.Dto;

namespace newPSG.PMS.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
        //object GetRoleCurrentUser();
        object GetAllChucVu();
    }
}
