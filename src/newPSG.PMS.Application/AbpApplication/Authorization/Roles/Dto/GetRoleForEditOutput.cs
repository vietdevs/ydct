using System.Collections.Generic;
using Abp.Application.Services.Dto;
using newPSG.PMS.Authorization.Permissions.Dto;

namespace newPSG.PMS.Authorization.Roles.Dto
{
    public class GetRoleForEditOutput
    {
        public RoleEditDto Role { get; set; }

        public List<FlatPermissionDto> Permissions { get; set; }

        public List<string> GrantedPermissionNames { get; set; }
    }
}