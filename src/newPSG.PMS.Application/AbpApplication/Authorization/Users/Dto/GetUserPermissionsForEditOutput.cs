using System.Collections.Generic;
using Abp.Application.Services.Dto;
using newPSG.PMS.Authorization.Permissions.Dto;

namespace newPSG.PMS.Authorization.Users.Dto
{
    public class GetUserPermissionsForEditOutput
    {
        public List<FlatPermissionDto> Permissions { get; set; }

        public List<string> GrantedPermissionNames { get; set; }
    }
}