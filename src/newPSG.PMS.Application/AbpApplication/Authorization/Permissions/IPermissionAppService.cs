using Abp.Application.Services;
using Abp.Application.Services.Dto;
using newPSG.PMS.Authorization.Permissions.Dto;

namespace newPSG.PMS.Authorization.Permissions
{
    public interface IPermissionAppService : IApplicationService
    {
        ListResultDto<FlatPermissionWithLevelDto> GetAllPermissions();
    }
}
